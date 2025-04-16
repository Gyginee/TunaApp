using Client.Models;
using Client.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Client
{
    public partial class Chat : Form
    {
        private string _currentChatTarget;
        private bool _isGroupChat;
        private List<string> _groupMembers = new List<string>();
        private Dictionary<string, List<MessageEntry>> _messageHistory = new();
        private HashSet<string> _renderedMessageKeys = new HashSet<string>();
        private Dictionary<string, int> userPresenceTracker = new Dictionary<string, int>();
        private Dictionary<string, List<Control>> _messageCache = new Dictionary<string, List<Control>>();

        public Chat()
        {
            InitializeComponent();
            InitializeChatComponents();
            Task.Run(BackgroundUserGroupFetcher);
            Task.Run(ListenForMessages);
            Task.Run(OneTimeInitialMessageLoad);
        }

        private void InitializeChatComponents()
        {
            userLabel.Text = AppState.CurrentUser;
            flowPanelLayout.AutoScroll = true;
            flowPanelLayout.FlowDirection = FlowDirection.TopDown;
            flowPanelLayout.WrapContents = false;
            flowPanelLayout.BackColor = Color.WhiteSmoke;

            InitializeEmojiButton();

            userListView.ContextMenuStrip = new ContextMenuStrip();
            userListView.ContextMenuStrip.Items.Clear();
            userListView.ContextMenuStrip.Items.Add("Trò chuyện", null, StartChatTarget);

            Task.Run(async () =>
            {
                if (!InformationManager.IsConnected)
                    await InformationManager.InitializeAsync(AppState.CurrentUser);

                await InformationManager.SendCommandAsync("GET_USER_AND_GROUPS|" + AppState.CurrentUser);
                string response = await InformationManager.ReadResponseAsync();

                var parts = response.Split('|');
                if (parts.Length >= 3)
                {
                    var users = parts[1].Split(',').Where(u => u != AppState.CurrentUser);
                    var groups = parts[2].Split(',');

                    Invoke(new Action(() =>
                    {
                        UpdateOnlineUsers(users);
                        UpdateGroupList(groups);
                    }));
                }
            });

        }

        private void UpdateOnlineUsers(IEnumerable<string> users)
        {
            foreach (var user in users)
            {
                if (!userPresenceTracker.ContainsKey(user))
                {
                    userPresenceTracker[user] = 0;
                    userListView.Items.Add(new ListViewItem(user));
                }
                else
                {
                    userPresenceTracker[user] = 0;
                }
            }

            foreach (var trackedUser in userPresenceTracker.Keys.ToList())
            {
                if (!users.Contains(trackedUser))
                {
                    userPresenceTracker[trackedUser]++;
                    if (userPresenceTracker[trackedUser] >= 3)
                    {
                        var itemToRemove = userListView.Items.Cast<ListViewItem>().FirstOrDefault(item => item.Text == trackedUser);
                        if (itemToRemove != null)
                        {
                            userListView.Items.Remove(itemToRemove);
                        }
                        userPresenceTracker.Remove(trackedUser);
                    }
                }
            }
        }

        private void UpdateGroupList(IEnumerable<string> groups)
        {
            foreach (var group in groups)
            {
                string groupName = $"[Group] {group}";
                if (!userListView.Items.Cast<ListViewItem>().Any(item => item.Text == groupName))
                {
                    userListView.Items.Add(new ListViewItem(groupName));
                }
            }
        }

        private void RemoveUserFromList(string username)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(RemoveUserFromList), username);
                return;
            }

            var itemToRemove = userListView.Items.Cast<ListViewItem>().FirstOrDefault(item => item.Text == username);
            if (itemToRemove != null)
            {
                userListView.Items.Remove(itemToRemove);
                userPresenceTracker.Remove(username);
            }
        }

        private void UpdateUserList(string serverResponse)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateUserList), serverResponse);
                return;
            }

            var users = serverResponse.Split('|').Skip(1).Where(user => user != AppState.CurrentUser);

            foreach (var user in users)
            {
                if (!userPresenceTracker.ContainsKey(user))
                {
                    userPresenceTracker[user] = 0;
                    userListView.Items.Add(new ListViewItem(user));
                }
                else
                {
                    userPresenceTracker[user] = 0;
                }
            }

            foreach (var trackedUser in userPresenceTracker.Keys.ToList())
            {
                if (!users.Contains(trackedUser))
                {
                    userPresenceTracker[trackedUser]++;
                    if (userPresenceTracker[trackedUser] >= 3)
                    {
                        var itemToRemove = userListView.Items.Cast<ListViewItem>().FirstOrDefault(item => item.Text == trackedUser);
                        if (itemToRemove != null)
                        {
                            userListView.Items.Remove(itemToRemove);
                        }
                        userPresenceTracker.Remove(trackedUser);
                    }
                }
            }
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var message = messageTextBox.Text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Vui lòng nhập nội dung tin nhắn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            await SendMessage(message);
            messageTextBox.Clear();
        }

        private async Task SendMessage(string message)
        {
            if (string.IsNullOrEmpty(_currentChatTarget))
            {
                MessageBox.Show("Vui lòng chọn người/nhóm để gửi tin nhắn!");
                return;
            }
            try
            {
                string command = _isGroupChat ?
                    $"GROUP_MESSAGE|{_currentChatTarget}|{message}" :
                    $"MESSAGE|{_currentChatTarget}|{message}";

                if (!MessageManager.IsConnected)
                {
                    bool connected = await MessageManager.InitializeAsync(AppState.CurrentUser);
                    if (!connected)
                    {
                        MessageBox.Show("Không thể kết nối tới server (tin nhắn).");
                        return;
                    }
                }

                await MessageManager.SendCommandAsync(command);

                var entry = new MessageEntry
                {
                    Sender = AppState.CurrentUser,
                    Content = message,
                    Timestamp = DateTime.Now,
                    GroupName = _isGroupChat ? _currentChatTarget : null
                };
                CacheMessage(_currentChatTarget, entry);
                if (_currentChatTarget != null)
                    flowPanelLayout.Controls.Add(CreateChatBubble(entry, true));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }


        private Control CreateChatBubble(MessageEntry message, bool isSender)
        {
            Color senderBubbleColor = Color.FromArgb(225, 245, 254);
            Color receiverBubbleColor = Color.FromArgb(241, 241, 241);
            Color infoTextColor = Color.Gray;
            Color contentTextColor = Color.Black;
            Font contentFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Font infoFont = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            int bubbleMaxWidthPercentage = 85;
            Padding bubblePadding = new Padding(10);
            Padding contentPadding = new Padding(0, 5, 0, 0);
            Padding infoPadding = new Padding(0, 0, 0, 5);
            Size fileButtonSize = new Size(50, 28); 
            Size imagePreviewSize = new Size(250, 250);

            int availableWidth = 600; 
            if (flowPanelLayout != null && flowPanelLayout.ClientSize.Width > 50)
            {
                availableWidth = flowPanelLayout.ClientSize.Width - flowPanelLayout.Padding.Horizontal;
            }
            int bubbleMaxWidth = (int)(availableWidth * (bubbleMaxWidthPercentage / 100.0));
            int contentMaxWidth = Math.Max(10, bubbleMaxWidth - bubblePadding.Horizontal); 

            Panel bubblePanel = new Panel
            {
                MaximumSize = new Size(bubbleMaxWidth, 0),
                MinimumSize = new Size(60, 30), 
                Padding = bubblePadding,
                BackColor = isSender ? senderBubbleColor : receiverBubbleColor,
            };

            Label infoLabel = new Label
            {
                Text = $"{message.Sender} - {message.GetFormattedTime()}",
                Font = infoFont,
                ForeColor = infoTextColor,
                BackColor = Color.Transparent,
                MaximumSize = new Size(contentMaxWidth, 0),
                Dock = DockStyle.Top,
                Padding = infoPadding,
                TextAlign = isSender ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft,
                AutoSize = true 
            };
            bubblePanel.Controls.Add(infoLabel);

            Control contentControl = null;

            try
            {
                if (message.IsImage && message.FileData != null)
                {
                    Panel imageContainer = new Panel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.Transparent };
                    Bitmap originalBitmap = null;
                    try
                    { 
                        using (MemoryStream ms = new MemoryStream(message.FileData)) { originalBitmap = new Bitmap(ms); }
                        Image previewImage = Utility.ResizeImage(originalBitmap, imagePreviewSize.Width*2, imagePreviewSize.Height);

                        PictureBox pictureBox = new PictureBox
                        { 
                            Image = previewImage,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Size = previewImage.Size,
                            MaximumSize = new Size(contentMaxWidth, imagePreviewSize.Height),
                            Cursor = Cursors.Hand,
                            Dock = DockStyle.Top,
                            BackColor = Color.WhiteSmoke,
                            Tag = originalBitmap
                        };
                        imageContainer.Controls.Add(pictureBox);
                        Button downloadButton = new Button
                        { 
                            Text = "Tải xuống",
                            AutoSize = true,
                            FlatStyle = FlatStyle.System,
                            Dock = DockStyle.Top,
                            Margin = new Padding(0, 5, 0, 0),
                            Tag = message.FileData
                        };
                        downloadButton.Click += async (s, e) => { await DownloadFile($"image_{DateTime.Now.Ticks}.jpg", (byte[])((Button)s).Tag); };
                        imageContainer.Controls.Add(downloadButton);
                        imageContainer.Controls.SetChildIndex(downloadButton, 0);
                        pictureBox.Click += (s, e) => { 
                            Bitmap bmpToShow = (Bitmap)((PictureBox)s).Tag; if (bmpToShow == null) return;
                            using (Form imageForm = new Form())
                            { 
                                imageForm.StartPosition = FormStartPosition.CenterScreen;
                                imageForm.Size = new Size(Math.Min(Screen.FromControl(bubblePanel).WorkingArea.Width - 100, bmpToShow.Width + 20),
                                                          Math.Min(Screen.FromControl(bubblePanel).WorkingArea.Height - 100, bmpToShow.Height + 40));
                                imageForm.Text = $"Ảnh từ {message.Sender}"; imageForm.BackColor = Color.Black;
                                PictureBox fullImage = new PictureBox { Dock = DockStyle.Fill, Image = bmpToShow, SizeMode = PictureBoxSizeMode.Zoom };
                                imageForm.Controls.Add(fullImage); imageForm.ShowDialog();
                            }
                        };
                        contentControl = imageContainer;
                    }
                    catch (Exception imgEx)
                    {
                        originalBitmap?.Dispose();
                        Label errorLabel = new Label { Text = $"Lỗi ảnh: {imgEx.Message}", AutoSize = true, MaximumSize = new Size(contentMaxWidth, 0), ForeColor = Color.Red, BackColor = Color.Transparent, Dock = DockStyle.Top, Padding = contentPadding };
                        contentControl = errorLabel;
                    }
                }
                else if (message.IsFile && message.FileData != null && !string.IsNullOrEmpty(message.FileName))
                {
                    FlowLayoutPanel fileFlowPanel = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.LeftToRight, 
                        WrapContents = false, 
                        AutoSize = true,      
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        MaximumSize = new Size(contentMaxWidth, 0), 
                        BackColor = Color.FromArgb(230, 230, 230),
                        Padding = new Padding(5), 
                        Margin = new Padding(0, 5, 0, 0), 
                        Dock = DockStyle.Top 
                    };

                    PictureBox fileIcon = new PictureBox
                    {
                        Image = SystemIcons.Information.ToBitmap(), 
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size(20, 20), 
                        Margin = new Padding(0, 0, 5, 0) 
                    };
                    fileFlowPanel.Controls.Add(fileIcon);


                    Label fileNameLabel = new Label
                    {
                        Text = message.FileName, 
                        Font = contentFont,
                        ForeColor = contentTextColor,
                        BackColor = Color.Transparent,
                        UseMnemonic = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        MaximumSize = new Size(contentMaxWidth - fileIcon.Width - fileButtonSize.Width - 25, 0),
                        Height = fileButtonSize.Height, 
                        Margin = new Padding(0, (fileButtonSize.Height - contentFont.Height) / 2, 5, 0)
                    };
                    fileFlowPanel.Controls.Add(fileNameLabel);

                    Button downloadButton = new Button
                    {
                        Text = "Tải",
                        Size = fileButtonSize,
                        MinimumSize = fileButtonSize, 
                        FlatStyle = FlatStyle.System,
                        Margin = new Padding(5, 0, 0, 0), 
                        Tag = message.FileData
                    };
                    downloadButton.Click += async (s, e) =>
                    {
                        await DownloadFile(message.FileName, (byte[])((Button)s).Tag);
                    };
                    fileFlowPanel.Controls.Add(downloadButton);


                    contentControl = fileFlowPanel; 
                }
                else
                {
                    Label contentLabel = new Label
                    {
                        Text = message.Content ?? string.Empty, 
                        Font = contentFont,
                        ForeColor = contentTextColor,
                        BackColor = Color.Transparent,
                        AutoSize = true,
                        MaximumSize = new Size(contentMaxWidth, 0),
                        Dock = DockStyle.Top,
                        Padding = contentPadding,
                        TextAlign = ContentAlignment.MiddleLeft
                    };
                    contentControl = contentLabel;
                }
            }
            catch (Exception ex) 
            {
                System.Diagnostics.Debug.WriteLine($"Error creating bubble content: {ex}"); 
                Label errorLabel = new Label { Text = $"Lỗi hiển thị: {ex.Message}", AutoSize = true, MaximumSize = new Size(contentMaxWidth, 0), ForeColor = Color.Red, BackColor = Color.Transparent, Dock = DockStyle.Top, Padding = contentPadding };
                contentControl = errorLabel;
            }

            if (contentControl != null)
            {
                bubblePanel.Controls.Add(contentControl);
                bubblePanel.Controls.SetChildIndex(infoLabel, bubblePanel.Controls.Count - 1);
                if (contentControl != infoLabel) 
                {
                    bubblePanel.Controls.SetChildIndex(contentControl, bubblePanel.Controls.Count - 2);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Warning: contentControl is null for message from {message.Sender}");
                Label fallbackError = new Label { Text = "Không thể hiển thị nội dung", AutoSize = true, MaximumSize = new Size(contentMaxWidth, 0), ForeColor = Color.OrangeRed, BackColor = Color.Transparent, Dock = DockStyle.Top, Padding = contentPadding };
                bubblePanel.Controls.Add(fallbackError);
                bubblePanel.Controls.SetChildIndex(infoLabel, bubblePanel.Controls.Count - 1);
                bubblePanel.Controls.SetChildIndex(fallbackError, bubblePanel.Controls.Count - 2);
            }


            bubblePanel.SuspendLayout(); 

            int totalHeight = bubblePanel.Padding.Vertical;
            int maxChildWidth = 0;
            foreach (Control ctrl in bubblePanel.Controls)
            {
                if (ctrl.Visible)
                {
                    Size preferredSize = ctrl.GetPreferredSize(new Size(contentMaxWidth, 0));
                    totalHeight += preferredSize.Height + ctrl.Margin.Vertical;
                    maxChildWidth = Math.Max(maxChildWidth, preferredSize.Width + ctrl.Margin.Horizontal);
                }
            }
            totalHeight = Math.Max(bubblePanel.MinimumSize.Height, totalHeight); 
            int finalWidth = Math.Max(bubblePanel.MinimumSize.Width, Math.Min(bubbleMaxWidth, maxChildWidth + bubblePanel.Padding.Horizontal));

            bubblePanel.Size = new Size(finalWidth, totalHeight);

            bubblePanel.ResumeLayout(true); 

            int horizontalMargin = 10;
            if (flowPanelLayout != null && flowPanelLayout.ClientSize.Width > bubblePanel.Width + 20)
            {
                horizontalMargin = flowPanelLayout.ClientSize.Width - bubblePanel.Width - flowPanelLayout.Padding.Horizontal - 10;
            }
            bubblePanel.Margin = isSender ? new Padding(Math.Max(10, horizontalMargin), 5, 5, 5)
                                         : new Padding(5, 5, Math.Max(10, horizontalMargin), 5);

            bubblePanel.Disposed += (sender, e) => {
                PictureBox pb = FindControlRecursive<PictureBox>(bubblePanel);
                if (pb != null)
                {
                    (pb.Tag as IDisposable)?.Dispose(); pb.Tag = null;
                    pb.Image?.Dispose(); pb.Image = null;
                }
            };

            return bubblePanel;
        }

        private T FindControlRecursive<T>(Control container) where T : Control
    {
        foreach (Control c in container.Controls)
        {
            if (c is T)
            {
                return (T)c;
            }
            T found = FindControlRecursive<T>(c);
            if (found != null)
            {
                return found;
            }
        }
        return null; 
    }


    public static class Utility
    {
        public static Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            if (image == null) return null;

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY); 

            if (ratio >= 1.0 && maxWidth >= image.Width && maxHeight >= image.Height)
            {
                return new Bitmap(image);
            }

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            if (newWidth <= 0 || newHeight <= 0) 
            {
                return new Bitmap(image);
            }


            var newImage = new Bitmap(newWidth, newHeight, image.PixelFormat); 

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }
}


        private void StartChatTarget(object sender, EventArgs e)
        {
            if (userListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một người dùng hoặc nhóm!");
                return;
            }

            string selected = userListView.SelectedItems[0].Text;
            bool isGroup = selected.StartsWith("[Group] ");

            string target = isGroup ? selected.Replace("[Group] ", "") : selected;
            ChangeChatTarget(target, isGroup);

            var entry = new MessageEntry
            {
                Sender = "System",
                Content = isGroup
                    ? $"Bạn đang trò chuyện nhóm [{target}]"
                    : $"Bạn đang trò chuyện với {target}",
                Timestamp = DateTime.Now,
                GroupName = isGroup ? target : null
            };

            CacheMessage(target, entry);
            flowPanelLayout.Controls.Add(CreateChatBubble(entry, true));

            flowPanelLayout.ScrollControlIntoView(flowPanelLayout.Controls[flowPanelLayout.Controls.Count - 1]);
        }

        private void InitializeEmojiButton()
        {
            emojiButton.Click += (s, e) =>
            {
                var emojiForm = new EmojiForm();
                emojiForm.EmojiSelected += (emoji) =>
                {
                    messageTextBox.Text += emoji;
                };
                emojiForm.StartPosition = FormStartPosition.CenterParent;
                emojiForm.ShowDialog();
            };
        }

        private void CreateGroupButton_Click(object sender, EventArgs e)
        {
            GroupCreationDialog groupDialog = new GroupCreationDialog(OnGroupJoined);
            groupDialog.ShowDialog();
        }

        private async void OnGroupJoined(string groupName)
        {
            if (InformationManager.IsConnected)
            {
                await InformationManager.SendCommandAsync("GET_USER_AND_GROUPS|" + AppState.CurrentUser);
                string response = await InformationManager.ReadResponseAsync();

                var parts = response.Split('|');
                if (parts.Length >= 3)
                {
                    var users = parts[1].Split(',').Where(u => u != AppState.CurrentUser);
                    var groups = parts[2].Split(',');

                    UpdateOnlineUsers(users);
                    UpdateGroupList(groups);
                }
            }

            var entry = new MessageEntry
            {
                Sender = "System",
                Content = $"Bạn đã tham gia nhóm [{groupName}]",
                Timestamp = DateTime.Now,
                GroupName = groupName 
            };

            CacheMessage(groupName, entry);

            flowPanelLayout.Controls.Add(CreateChatBubble(entry, true));

            flowPanelLayout.ScrollControlIntoView(flowPanelLayout.Controls[flowPanelLayout.Controls.Count - 1]);
        }

        private void ProcessMessages(string response)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(ProcessMessages), response);
                return;
            }

            var messages = response.Split(new[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var msg in messages)
            {
                var parts = msg.Split('|');
                if (parts.Length < 2) continue;

                string messageType = parts[0];
                string sender = parts[1];
                MessageEntry entry = null;

                switch (messageType)
                {
                    case "MESSAGE":
                        if (parts.Length >= 3)
                        {
                            string content = parts[2];
                            if (sender != AppState.CurrentUser) 
                            {
                                entry = new MessageEntry
                                {
                                    Sender = sender,
                                    Content = content,
                                    Timestamp = DateTime.Now
                                };
                                CacheMessage(sender, entry);
                                if (sender == _currentChatTarget)
                                    flowPanelLayout.Controls.Add(CreateChatBubble(entry, false));
                            }
                        }
                        break;

                    case "GROUP_MESSAGE":
                        if (parts.Length >= 4)
                        {
                            string groupName = parts[2];
                            string content = parts[3];

                            if (sender != AppState.CurrentUser) 
                            {
                                entry = new MessageEntry
                                {
                                    Sender = sender,
                                    Content = content,
                                    Timestamp = DateTime.Now,
                                    GroupName = groupName
                                };
                                CacheMessage(groupName, entry);
                                if (groupName == _currentChatTarget)
                                    flowPanelLayout.Controls.Add(CreateChatBubble(entry, false));
                            }
                        }
                        break;

                    case "IMAGE":
                        if (parts.Length >= 3)
                        {
                            byte[] data = Convert.FromBase64String(parts[2]);
                            entry = new MessageEntry
                            {
                                Sender = sender,
                                Timestamp = DateTime.Now,
                                IsImage = true,
                                FileData = data
                            };
                            CacheMessage(sender, entry);
                            if (sender == _currentChatTarget)
                                flowPanelLayout.Controls.Add(CreateChatBubble(entry, false));
                        }
                        break;

                    case "GROUP_IMAGE":
                        if (parts.Length >= 4)
                        {
                            string group = parts[2];
                            byte[] data = Convert.FromBase64String(parts[3]);
                            if (sender != AppState.CurrentUser)
                            {
                                entry = new MessageEntry
                                {
                                    Sender = sender,
                                    Timestamp = DateTime.Now,
                                    IsImage = true,
                                    FileData = data,
                                    GroupName = group
                                };
                                CacheMessage(group, entry);
                                if (group == _currentChatTarget)
                                    flowPanelLayout.Controls.Add(CreateChatBubble(entry, false));
                            }
                        }
                        break;

                    case "FILE":
                        if (parts.Length >= 4)
                        {
                            string fileName = parts[2];
                            byte[] fileData = Convert.FromBase64String(parts[3]);
                            if (sender != AppState.CurrentUser)
                            {
                                entry = new MessageEntry
                                {
                                    Sender = sender,
                                    Timestamp = DateTime.Now,
                                    IsFile = true,
                                    FileName = fileName,
                                    FileData = fileData
                                };
                                CacheMessage(sender, entry);
                                if (sender == _currentChatTarget)
                                    flowPanelLayout.Controls.Add(CreateChatBubble(entry, false));
                            }
                        }
                        break;

                    case "GROUP_FILE":
                        if (parts.Length >= 5)
                        {
                            string group = parts[2];
                            string fileName = parts[3];
                            byte[] fileData = Convert.FromBase64String(parts[4]);
                            if (sender != AppState.CurrentUser)
                            {
                                entry = new MessageEntry
                                {
                                    Sender = sender,
                                    Timestamp = DateTime.Now,
                                    IsFile = true,
                                    FileName = fileName,
                                    FileData = fileData,
                                    GroupName = group
                                };
                                CacheMessage(group, entry);
                                if (group == _currentChatTarget)
                                    flowPanelLayout.Controls.Add(CreateChatBubble(entry, false));
                            }
                        }
                        break;

                    case "GROUP_CREATED":
                        if (parts.Length >= 3)
                        {
                            string groupName = parts[1];
                            string members = parts[2];
                            _groupMembers = members.Split(',').ToList();
                            entry = new MessageEntry
                            {
                                Sender = "System",
                                Content = $"Nhóm [{groupName}] đã được tạo với thành viên: {string.Join(", ", _groupMembers)}",
                                Timestamp = DateTime.Now,
                                GroupName = groupName
                            };
                            CacheMessage(groupName, entry);

                            string groupDisplay = $"[Group] {groupName}";
                            if (!userListView.Items.Cast<ListViewItem>().Any(item => item.Text == groupDisplay))
                            {
                                userListView.Items.Add(new ListViewItem(groupDisplay));
                            }

                            if (groupName == _currentChatTarget)
                                flowPanelLayout.Controls.Add(CreateChatBubble(entry, false));
                        }
                        break;

                    case "GROUP_JOINED":
                        if (parts.Length >= 3)
                        {
                            string newUser = parts[1];
                            string groupName = parts[2];
                            string groupDisplay = $"[Group] {groupName}";

                            string joinText = newUser == AppState.CurrentUser
                                ? $"Bạn đã tham gia nhóm [{groupName}]"
                                : $"{newUser} đã tham gia nhóm [{groupName}]";

                            entry = new MessageEntry
                            {
                                Sender = "System",
                                Content = joinText,
                                Timestamp = DateTime.Now,
                                GroupName = groupName
                            };
                            CacheMessage(groupName, entry);

                            if (groupName == _currentChatTarget)
                                flowPanelLayout.Controls.Add(CreateChatBubble(entry, newUser == AppState.CurrentUser));

                            if (newUser == AppState.CurrentUser &&
                                !userListView.Items.Cast<ListViewItem>().Any(item => item.Text == groupDisplay))
                            {
                                userListView.Items.Add(new ListViewItem(groupDisplay));
                            }
                        }
                        break;

                    case "JOIN_FAIL":
                        if (parts.Length >= 2)
                        {
                            MessageBox.Show($"Không thể tham gia nhóm: {parts[1]}", "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case "USER_CONNECTED":
                        UpdateUserList($"USERS|{parts[1]}");
                        break;

                    case "USER_DISCONNECTED":
                        RemoveUserFromList(parts[1]);
                        break;
                }
            }
        }


        private async void SendFileButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentChatTarget))
            {
                MessageBox.Show("Vui lòng chọn người/nhóm để gửi tệp!");
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "All Files (*.*)|*.*|Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg; *.jpeg; *.png; *.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    string fileName = Path.GetFileName(filePath);

                    try
                    {
                        string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };
                        if (imageExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                        {
                            await SendImage(filePath);
                        }
                        else
                        {
                            await SendFile(filePath, fileName);  
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi gửi tệp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task SendFile(string filePath, string fileName)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64File = Convert.ToBase64String(fileBytes);

            string command = _isGroupChat
                ? $"GROUP_FILE|{_currentChatTarget}|{fileName}|{base64File}"
                : $"FILE|{_currentChatTarget}|{fileName}|{base64File}";

            if (!MessageManager.IsConnected)
            {
                bool connected = await MessageManager.InitializeAsync(AppState.CurrentUser);
                if (!connected)
                {
                    MessageBox.Show("Không thể kết nối tới server (tin nhắn).");
                    return;
                }
            }

            await MessageManager.SendCommandAsync(command);

            var entry = new MessageEntry
            {
                Sender = AppState.CurrentUser,
                Timestamp = DateTime.Now,
                IsFile = true,
                FileName = fileName,
                FileData = fileBytes
            };
            CacheMessage(_currentChatTarget, entry);
            if (_currentChatTarget != null)
                flowPanelLayout.Controls.Add(CreateChatBubble(entry, true));
        }


        private async Task SendImage(string filePath)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(filePath);
                string base64Image = Convert.ToBase64String(imageBytes);

                string command = _isGroupChat
                    ? $"GROUP_IMAGE|{_currentChatTarget}|{base64Image}"
                    : $"IMAGE|{_currentChatTarget}|{base64Image}";

                if (!MessageManager.IsConnected)
                {
                    bool connected = await MessageManager.InitializeAsync(AppState.CurrentUser);
                    if (!connected)
                    {
                        MessageBox.Show("Không thể kết nối tới server (tin nhắn).");
                        return;
                    }
                }

                await MessageManager.SendCommandAsync(command);

                var entry = new MessageEntry
                {
                    Sender = AppState.CurrentUser,
                    Timestamp = DateTime.Now,
                    IsImage = true,
                    FileData = imageBytes
                };
                CacheMessage(_currentChatTarget, entry);
                if (_currentChatTarget != null)
                    flowPanelLayout.Controls.Add(CreateChatBubble(entry, true));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi hình ảnh: {ex.Message}");
            }
        }

        private void CacheMessage(string target, MessageEntry entry)
        {
            if (!_messageHistory.ContainsKey(target))
                _messageHistory[target] = new List<MessageEntry>();

            string messageKey = entry.GetUniqueKey();
            if (!_renderedMessageKeys.Contains(messageKey))
            {
                _messageHistory[target].Add(entry);
                _renderedMessageKeys.Add(messageKey);
            }
        }

        private async Task DownloadFile(string fileName, byte[] fileData)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = fileName;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    await File.WriteAllBytesAsync(sfd.FileName, fileData);
                    MessageBox.Show("Tải xuống thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ChangeChatTarget(string newTarget, bool isGroup)
        {
            _renderedMessageKeys.Clear();
            _currentChatTarget = newTarget;
            _isGroupChat = isGroup;

            flowPanelLayout.Controls.Clear();

            if (_messageHistory.ContainsKey(newTarget))
            {
                foreach (var message in _messageHistory[newTarget])
                {
                    bool isSender = message.Sender == AppState.CurrentUser;
                    flowPanelLayout.Controls.Add(CreateChatBubble(message, isSender));
                }
            }

            if (flowPanelLayout.Controls.Count > 0)
            {
                flowPanelLayout.ScrollControlIntoView(flowPanelLayout.Controls[flowPanelLayout.Controls.Count - 1]);
            }
        }

        private async Task BackgroundUserGroupFetcher()
        {
            while (true)
            {
                if (!InformationManager.IsConnected)
                {
                    await Task.Delay(5000);
                    continue;
                }

                try
                {
                    await InformationManager.SendCommandAsync("GET_USER_AND_GROUPS|" + AppState.CurrentUser);
                    string response = await InformationManager.ReadResponseAsync();

                    var parts = response.Split('|');
                    if (parts.Length >= 3)
                    {
                        var users = parts[1].Split(',').Where(u => u != AppState.CurrentUser);
                        var groups = parts[2].Split(',').Where(g => !string.IsNullOrWhiteSpace(g)); 

                        Invoke(new Action(() =>
                        {
                            UpdateOnlineUsers(users);
                            UpdateGroupList(groups);
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"User/group fetch error: {ex.Message}");
                }

                await Task.Delay(1000); 
            }
        }

        private async Task ListenForMessages()
        {
            while (true)
            {
                try
                {
                    if (!MessageManager.IsConnected)
                    {
                        bool connected = await MessageManager.InitializeAsync(AppState.CurrentUser);
                        if (!connected)
                        {
                            await Task.Delay(1000);
                            continue;
                        }
                    }

                    string line = await MessageManager.ReadResponseAsync();
                    if (!string.IsNullOrEmpty(line))
                    {
                        ProcessMessages(line);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Receive Error] {ex.Message}");
                    await Task.Delay(1000);
                }
            }
        }

        private async Task OneTimeInitialMessageLoad()
        {
            try
            {
                if (!MessageManager.IsConnected)
                {
                    bool connected = await MessageManager.InitializeAsync(AppState.CurrentUser);
                    if (!connected)
                    {
                        MessageBox.Show("Không thể kết nối tới server (tin nhắn).");
                        return;
                    }
                }

                await MessageManager.SendCommandAsync("GET_MESSAGES");
                var response = await MessageManager.ReadResponseAsync();
                ProcessMessages(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initial message load error: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; 
                this.Hide(); 
                Menu.Instance.Show(); 
            }
            else
            {
                base.OnFormClosing(e);
            }
        }

    }
}

