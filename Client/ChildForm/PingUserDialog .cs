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
                // Khởi tạo kết nối nếu cần
                if (!await UserManager.InitializeAsync(AppState.CurrentUser))
                {
                    statusLabel.Text = "❌ Không kết nối được tới server (UserManager).";
                    return;
                }

                // Gửi lệnh GET_USER_AND_GROUPS và xử lý phản hồi
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

            statusLabel.Text = $"🔍 Kiểm tra {selected}...";

            // Bước 1: Kiểm tra user có tồn tại không
            var checkResp = await UtilityManager.SendSingleCommandAsync(AppState.CurrentUser, $"GET_USER|{selected}");
            if (!checkResp.StartsWith("USER|"))
            {
                statusLabel.Text = $"❌ Người dùng '{selected}' không tồn tại.";
                return;
            }

            var pingResp = await UtilityManager.SendSingleCommandAsync(AppState.CurrentUser, $"PING_USER|{selected}");

            if (pingResp.StartsWith("PONG_USER"))
            {
                statusLabel.Text = $"✅ {selected} đang online.";
            }
            else if (pingResp.StartsWith("PING_FAIL"))
            {
                statusLabel.Text = $"🔴 {selected} không online.";
            }
            else
            {
                statusLabel.Text = $"❌ Không phản hồi từ server (timeout hoặc lỗi).";
            }


            SelectedUser = selected;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
