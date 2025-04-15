using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace Client
{
    public partial class Web : Form
    {
        private WebView2 webView;

        public Web()
        {
            InitializeComponent();
            addressBox.KeyDown += addressBox_KeyDown;
           

            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            // Thêm WebView2 vào Panel chứa web
            webPanel.Controls.Add(webView);

            // Cấu hình WebView2
            //webView.NavigationCompleted += WebView_NavigationCompleted;

            // Khởi động WebView2
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await webView.EnsureCoreWebView2Async();
            webView.Source = new Uri("https://www.google.com");
        }

        private void addressBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                goBtn.PerformClick(); // Gọi sự kiện click của nút "Go"
                e.SuppressKeyPress = true; // Ngăn Enter gây tiếng "beep"
            }
        }

        private void goBtn_Click(object sender, EventArgs e)
        {
            string input = addressBox.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ web!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!input.Contains(".") || Uri.CheckHostName(input) == UriHostNameType.Unknown)
            {
                // Nếu không phải domain, thì search Google
                string query = Uri.EscapeDataString(input);
                webView.Source = new Uri($"https://www.google.com/search?q={query}");
            }
            else
            {
                // Nếu là domain (và không có http/https), thêm vào
                if (!input.StartsWith("http://") && !input.StartsWith("https://"))
                {
                    input = "https://" + input;
                }

                try
                {
                    webView.Source = new Uri(input);
                }
                catch
                {
                    MessageBox.Show("Địa chỉ không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            MessageBox.Show("Trang đã tải xong: " + webView.Source, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
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
    }
}
