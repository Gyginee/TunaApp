using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Models;
using Client.Utils;

namespace Client.ChildForm
{
    public partial class PingUserDialog : Form
    {
        public string SelectedUser { get; private set; }

        public PingUserDialog()
        {
            InitializeComponent();
            LoadUserList();
            listBoxUsers.SelectedIndexChanged += ListBoxUsers_SelectedIndexChanged;
        }

        private async void LoadUserList()
        {
            statusLabel.Text = "🔄 Đang tải danh sách người dùng...";

            try
            {
                if (!await UserManager.InitializeAsync(AppState.CurrentUser))
                {
                    statusLabel.Text = "❌ Không kết nối được tới server (UserManager).";
                    return;
                }

                var users = await UserManager.GetOnlineUsersAsync(AppState.CurrentUser);
                users = users.Where(u => u != AppState.CurrentUser).ToArray();

                if (users.Length > 0)
                {
                    listBoxUsers.Items.AddRange(users);
                    statusLabel.Text = $"📋 Đã tải {users.Length} người dùng.";
                }
                else
                {
                    statusLabel.Text = "⚠️ Không có người dùng online.";
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"❌ Lỗi khi tải user: {ex.Message}";
            }
        }

        private async void ListBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = listBoxUsers.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selected))
            {
                statusLabel.Text = "🔸 Vui lòng chọn người dùng.";
                return;
            }

            statusLabel.Text = $"📡 Đang ping {selected}...";

            try
            {
                string response = await UtilityManager.SendSingleCommandAsync(AppState.CurrentUser, $"PING_USER_IP|{selected}");

                if (response.StartsWith("PING_USER_IP_RESULT|"))
                {
                    string[] parts = response.Split('|');

                    if (parts.Length >= 4 && parts[1] != "FAIL")
                    {
                        string username = parts[1];
                        string ip = parts[2];
                        string pingResult = parts[3];

                        statusLabel.Text = $"📍 {username} @ {ip} — {pingResult}";
                    }
                    else
                    {
                        statusLabel.Text = "❌ Không tìm thấy người dùng hoặc IP.";
                    }
                }
                else
                {
                    statusLabel.Text = "❌ Phản hồi không hợp lệ từ server.";
                }

                SelectedUser = selected;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"❌ Lỗi khi ping: {ex.Message}";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
