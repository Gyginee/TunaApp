using Client.Models; // Đảm bảo namespace này đúng
using System;
using System.IO;
using System.Windows.Forms;
// using System.Drawing; // Thêm nếu cần cho Font hoặc màu sắc

namespace Client.ChildForm
{
    public partial class EmailDetailForm : Form
    {
        private readonly EmailInfo _email;

        // Constructor nhận EmailInfo
        public EmailDetailForm(EmailInfo email)
        {
            InitializeComponent();
            _email = email ?? throw new ArgumentNullException(nameof(email));

            // *** Sửa: Thiết lập DisplayMember cho ListBox ***
            lstAttachments.DisplayMember = "FileName"; // Chỉ định thuộc tính FileName của AttachmentInfo để hiển thị
            lstAttachments.ValueMember = null; // Không cần ValueMember ở đây

            // Gán sự kiện
            this.Load += EmailDetailForm_Load;
            lstAttachments.SelectedIndexChanged += LstAttachments_SelectedIndexChanged;
            btnSaveAttachment.Click += BtnSaveAttachment_Click;
        }

        private void EmailDetailForm_Load(object sender, EventArgs e)
        {
            if (_email == null) return;

            // --- Điền thông tin Header ---
            txtFrom.Text = _email.Sender;
            txtTo.Text = _email.To ?? "";
            txtCc.Text = _email.Cc ?? "";
            txtSubject.Text = _email.Subject;
            txtDate.Text = _email.ReceivedDate.ToString("dd/MM/yyyy HH:mm:ss");
            this.Text = $"Thư: {_email.Subject}";

            // --- Xử lý Nội dung (Body) ---
            // Giữ nguyên như cũ, dùng RichTextBox cho text
            // Cân nhắc dùng WebBrowser/WebView2 nếu IsHtmlBody là true
            if (_email.IsHtmlBody)
            {
                // Nếu dùng RichTextBox, bạn có thể cố gắng hiển thị HTML cơ bản
                // nhưng kết quả có thể không như ý.
                // Thay vào đó, bạn có thể hiển thị thông báo là nội dung HTML
                // hoặc dùng WebBrowser/WebView2.
                // Ví dụ đơn giản:
                rtbBody.Text = $"Nội dung email dạng HTML:\n\n{_email.Body}";
                // Hoặc nếu bạn có WebBrowser control tên là webBrowserBody:
                // webBrowserBody.DocumentText = _email.Body;
            }
            else
            {
                rtbBody.Text = _email.Body;
            }


            // --- Xử lý File đính kèm ---
            if (_email.Attachments != null && _email.Attachments.Count > 0)
            {
                pnAttachments.Visible = true;
                lstAttachments.Items.Clear();
                foreach (var attachment in _email.Attachments)
                {
                    // *** Sửa: Thêm trực tiếp object AttachmentInfo vào ListBox ***
                    lstAttachments.Items.Add(attachment);
                }
            }
            else
            {
                pnAttachments.Visible = false;
            }
            UpdateButtonStates();
        }

        private void LstAttachments_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            btnSaveAttachment.Enabled = lstAttachments.SelectedItem != null;
        }

        private void BtnSaveAttachment_Click(object sender, EventArgs e)
        {
            // *** Sửa: Ép kiểu SelectedItem thành AttachmentInfo ***
            if (lstAttachments.SelectedItem is AttachmentInfo attachment)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = attachment.FileName;
                    // Có thể thêm Filter dựa trên ContentType nếu có
                    // Ví dụ: if (!string.IsNullOrEmpty(attachment.ContentType)) saveFileDialog.Filter = $"{attachment.ContentType}|{attachment.ContentType}|All files (*.*)|*.*";
                    saveFileDialog.Filter = "All files (*.*)|*.*";

                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK) // Thêm 'this' để dialog mở tương đối với form hiện tại
                    {
                        try
                        {
                            File.WriteAllBytes(saveFileDialog.FileName, attachment.Content);
                            MessageBox.Show(this, $"Đã lưu file '{attachment.FileName}' thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (IOException ioEx) // Bắt lỗi IO cụ thể
                        {
                            MessageBox.Show(this, $"Lỗi IO khi lưu file: {ioEx.Message}\nĐường dẫn: {saveFileDialog.FileName}", "Lỗi Lưu File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex) // Bắt lỗi chung khác
                        {
                            MessageBox.Show(this, $"Lỗi không xác định khi lưu file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else if (lstAttachments.SelectedIndex != -1)
            {
                // Trường hợp item được chọn nhưng không phải AttachmentInfo (lỗi logic)
                MessageBox.Show(this, "Không thể lấy thông tin file đính kèm đã chọn.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}