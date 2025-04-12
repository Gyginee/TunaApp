using Client.Models;
using Client.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
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

            sendFileButton.Click += SendFileButton_Click;
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
            Panel bubblePanel = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(flowPanelLayout.Width - 20, 0),
                Padding = new Padding(5),
                Margin = new Padding(5),
                BackColor = Color.Transparent
            };
            
            Label senderLabel = new Label
            {
                Text = $"{message.Sender} - {message.GetFormattedTime()}",
                AutoSize = true,
                Dock = DockStyle.Top,
                Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            
            bubblePanel.Controls.Add(senderLabel);
            
            if (message.IsImage)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(message.FileData))
                    {
                        Image img = Image.FromStream(ms);
                        PictureBox pictureBox = new PictureBox
                        {
                            Image = new Bitmap(img),
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Width = 200,
                            Height = 200,
                            Margin = new Padding(5),
                            BackColor = Color.White
                        };
                        
                        bubblePanel.Controls.Add(pictureBox);
                        
                        pictureBox.Click += (s, e) => 
                        {
                            using (Form imageForm = new Form())
                            {
                                imageForm.StartPosition = FormStartPosition.CenterScreen;
                                imageForm.Size = new Size(800, 600);
                                imageForm.Text = $"Ảnh từ {message.Sender}";
                                
                                PictureBox fullImage = new PictureBox
                                {
                                    Dock = DockStyle.Fill,
                                    Image = new Bitmap(img),
                                    SizeMode = PictureBoxSizeMode.Zoom
                                };
                                
                                imageForm.Controls.Add(fullImage);
                                imageForm.ShowDialog();
                            }
                        };
                    }
                }
                catch (Exception ex)
                {
                    Label errorLabel = new Label
                    {
                        Text = $"Không thể hiển thị ảnh: {ex.Message}",
                        AutoSize = true,
                        ForeColor = Color.Red
                    };
                    bubblePanel.Controls.Add(errorLabel);
                }
            }
            else if (message.IsFile)
            {
                Panel filePanel = new Panel
                {
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    BackColor = Color.LightYellow,
                    Padding = new Padding(5)
                };
                
                Label fileNameLabel = new Label
                {
                    Text = $"File: {message.FileName}",
                    AutoSize = true,
                    Dock = DockStyle.Top,
                    Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular)
                };
                
                Button downloadButton = new Button
                {
                    Text = "Tải xuống",
                    AutoSize = true,
                    Dock = DockStyle.Bottom,
                    Tag = message.FileData
                };
                
                downloadButton.Click += async (s, e) =>
                {
                    await DownloadFile(message.FileName, (byte[])((Button)s).Tag);
                };
                
                filePanel.Controls.Add(fileNameLabel);
                filePanel.Controls.Add(downloadButton);
                bubblePanel.Controls.Add(filePanel);
            }
            else
            {
                Label contentLabel = new Label
                {
                    Text = message.Content,
                    AutoSize = true,
                    MaximumSize = new Size(flowPanelLayout.Width - 60, 0),
                    Padding = new Padding(10),
                    BackColor = isSender ? Color.LightBlue : Color.LightGray,
                    ForeColor = Color.Black
                };
                
                bubblePanel.Controls.Add(contentLabel);
            }
            
            if (isSender)
            {
                bubblePanel.Anchor = AnchorStyles.Right;
                senderLabel.TextAlign = ContentAlignment.MiddleRight;
            }
            else
            {
                bubblePanel.Anchor = AnchorStyles.Left;
                senderLabel.TextAlign = ContentAlignment.MiddleLeft;
            }
            
            return bubblePanel;
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

            AddMessageToChat($"Bạn đã tham gia nhóm [{groupName}]", true);
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
                        break;

                    case "GROUP_MESSAGE":
                        if (parts.Length >= 4)
                        {
                            string group = parts[2];
                            string content = parts[3];
                            entry = new MessageEntry
                            {
                                Sender = sender,
                                Content = content,
                                Timestamp = DateTime.Now,
                                GroupName = group
                            };
                            CacheMessage(group, entry);
                            if (group == _currentChatTarget)
                                flowPanelLayout.Controls.Add(CreateChatBubble(entry, sender == AppState.CurrentUser));
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

    case "GROUP_FILE":
        if (parts.Length >= 5)
        {
            string group = parts[2];
            string fileName = parts[3];
            byte[] fileData = Convert.FromBase64String(parts[4]);
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
                flowPanelLayout.Controls.Add(CreateChatBubble(entry, sender == AppState.CurrentUser));
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
            
            // Thêm nhóm vào danh sách nếu chưa có
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

    case "USER_CONNECTED":
        UpdateUserList($"USERS|{parts[1]}");
        break;

    case "USER_DISCONNECTED":
        RemoveUserFromList(parts[1]);
        break;
}
}
}

private void CacheMessage(string target, MessageEntry entry)
{
    if (!_messageHistory.ContainsKey(target))
        _messageHistory[target] = new List<MessageEntry>();
    
    // Kiểm tra xem tin nhắn đã tồn tại chưa để tránh hiển thị trùng lặp
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
    
    // Làm mới giao diện
    flowPanelLayout.Controls.Clear();
    
    // Hiển thị lại tin nhắn cũ từ cache
    if (_messageHistory.ContainsKey(newTarget))
    {
        foreach (var message in _messageHistory[newTarget])
        {
            bool isSender = message.Sender == AppState.CurrentUser;
            flowPanelLayout.Controls.Add(CreateChatBubble(message, isSender));
        }
    }
    
    // Cuộn xuống tin nhắn mới nhất nếu có
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
                var groups = parts[2].Split(',').Where(g => !string.IsNullOrWhiteSpace(g)); // tránh group rỗng

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

        await Task.Delay(5000); // delay giữa mỗi lần fetch
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
        e.Cancel = true; // Tạm hoãn đóng form
        this.Hide(); // Ẩn form hiện tại
        Menu.Instance.Show(); // Hiển thị lại form Menu
    }
    else
    {
        base.OnFormClosing(e);
    }
}

// Lớp MessageEntry để lưu trữ mọi loại tin nhắn
public class MessageEntry
{
    public string Sender { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsImage { get; set; }
    public bool IsFile { get; set; }
    public string FileName { get; set; }
    public byte[] FileData { get; set; }
    public string GroupName { get; set; } // Nếu là tin nhắn nhóm
    
    public string GetFormattedTime()
    {
        return Timestamp.ToString("HH:mm:ss");
    }
    
    // Tạo key duy nhất cho tin nhắn để tránh hiển thị trùng lặp
    public string GetUniqueKey()
    {
        string type = IsImage ? "IMAGE" : (IsFile ? "FILE" : "TEXT");
        string contentHash = string.Empty;
        
        if (IsImage || IsFile)
        {
            using (SHA256 sha = SHA256.Create())
            {
                contentHash = Convert.ToBase64String(sha.ComputeHash(FileData)).Substring(0, 10);
            }
        }
        else
        {
            contentHash = Content;
        }
        
        return $"{type}|{Sender}|{Timestamp.Ticks}|{contentHash}";
    }
}

