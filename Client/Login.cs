using Client.Models;
using Client.Utils;
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Login : Form
    {
        private static Login _instance;
        public Login()
        {
            InitializeComponent();


        }
        public static Login Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new Login();
                }
                return _instance;
            }
        }


        private async void loginBox_Click(object sender, EventArgs e)
        {
            await ProcessAuthCommand("LOGIN");

        }

        private async void signBox_Click(object sender, EventArgs e)
        {
            await ProcessAuthCommand("REGISTER");
        }

        private async Task ProcessAuthCommand(string commandType)
        {
            try
            {
                var username = userBox.Text.Trim();
                var password = passBox.Text.Trim();

                if (!ValidateCredentials(username, password)) return;

                // ✅ GỌI INIT Ở ĐÂY
                var connected = await ConnectionManager.InitializeAsync();
                if (!connected)
                {
                    ShowError("Không thể kết nối đến server.");
                    return;
                }

                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await ConnectionManager.SendCommandAsync($"{commandType}|{username}|{password}");
                    var response = await ConnectionManager.ReadResponseAsync();
                    ProcessAuthResponse(commandType, response);
                }
            }
            catch (OperationCanceledException)
            {
                ShowError("Timeout kết nối server");
            }
            catch (Exception ex)
            {
                ShowError($"{commandType} error: {ex.Message}");
            }
        }


        private void ProcessAuthResponse(string commandType, string response)
        {
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    if (response == null)
                    {
                        ShowError("Không nhận được phản hồi từ server");
                        return;
                    }

                    var parts = response.Split('|');
                    if (parts.Length == 0)
                    {
                        ShowError("Phản hồi không hợp lệ từ server");
                        return;
                    }

                    var isSuccess = (parts[0].Trim().Equals("LOGIN_SUCCESS", StringComparison.OrdinalIgnoreCase) || parts[0].Trim().Equals("REGISTER_SUCCESS", StringComparison.OrdinalIgnoreCase)) ;
                    if (!isSuccess)
                    {
                        var errorMessage = parts.Length > 1 ? parts[1] : "Lỗi không xác định";
                        MessageBox.Show(errorMessage, "LỖI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (commandType == "LOGIN" && isSuccess)
                    {
                        AppState.CurrentUser = userBox.Text.Trim();
                        this.Hide();
                        Menu.Instance.Show();
                    }
                    else
                    {
                        var message = isSuccess
                            ? "Thao tác thành công!"
                            : (parts.Length > 1 ? parts[1] : "Lỗi không xác định");

                        MessageBox.Show(message,
                            isSuccess ? "THÔNG BÁO" : "LỖI",
                            MessageBoxButtons.OK,
                            isSuccess ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"{commandType} lỗi: {ex.Message}");
                }
            });
        }

        private bool ValidateCredentials(string username, string password)
        {
            var regex = new Regex("^[a-zA-Z0-9_]{4,20}$");
            var isValid = regex.IsMatch(username) && regex.IsMatch(password);

            if (!isValid)
            {
                ShowError("Tài khoản/mật khẩu phải từ 4-20 ký tự và không chứa ký tự đặc biệt");
            }

            return isValid;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "LỖI", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
    }
}
