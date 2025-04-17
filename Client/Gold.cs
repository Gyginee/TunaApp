using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Models;
using Client.Utils;
using Newtonsoft.Json;

namespace Client
{
    public partial class Gold : Form
    {
        public Gold()
        {
            InitializeComponent(); 

            InitializeManualComponents();

            SetupUI();

            refreshButton.Click += RefreshButton_Click;
            this.Load += Gold_Load;
        }

        private void InitializeManualComponents()
        {
            if (this.refreshButton == null)
            {
                this.refreshButton = new System.Windows.Forms.Button();
            }
            if (this.goldData == null)
            {
                this.goldData = new System.Windows.Forms.DataGridView();
                ((System.ComponentModel.ISupportInitialize)(this.goldData)).BeginInit();
            }
            if (this.statusStrip == null)
            {
                this.statusStrip = new System.Windows.Forms.StatusStrip();
            }
            if (this.statusLabel == null)
            {
                this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            }

            this.Controls.Add(this.goldData); 
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.statusStrip);
            this.statusStrip.Items.Add(this.statusLabel);

            if (this.goldData != null)
            {
                ((System.ComponentModel.ISupportInitialize)(this.goldData)).EndInit();
            }
        }

        private void SetupUI()
        {
            this.Text = "Giá Vàng SJC";
            this.BackColor = Color.WhiteSmoke; 
            this.Padding = new Padding(10); 
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.MinimumSize = new Size(450, 350); 

            refreshButton.Text = "Làm mới";
            refreshButton.FlatStyle = FlatStyle.Flat; 
            refreshButton.FlatAppearance.BorderSize = 0; 
            refreshButton.BackColor = Color.FromArgb(0, 122, 204); 
            refreshButton.ForeColor = Color.White; 
            refreshButton.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            refreshButton.Size = new Size(100, 32);
            refreshButton.Cursor = Cursors.Hand;
            refreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right; 
            refreshButton.Location = new Point(this.ClientSize.Width - refreshButton.Width - this.Padding.Right, this.Padding.Top); 

            goldData.Dock = DockStyle.None; 
            goldData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; 
            goldData.Location = new Point(this.Padding.Left, refreshButton.Bottom + 10); 
            goldData.Size = new Size(this.ClientSize.Width - this.Padding.Horizontal,
                                     this.ClientSize.Height - refreshButton.Bottom - this.Padding.Bottom - statusStrip.Height - 15); 

            goldData.AllowUserToAddRows = false;
            goldData.AllowUserToDeleteRows = false;
            goldData.AllowUserToResizeRows = false; 
            goldData.ReadOnly = true;
            goldData.RowHeadersVisible = false; 
            goldData.BorderStyle = BorderStyle.None; 
            goldData.BackgroundColor = Color.White; 
            goldData.GridColor = Color.Gainsboro; 
            goldData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; 
            goldData.SelectionMode = DataGridViewSelectionMode.FullRowSelect; 
            goldData.MultiSelect = false;

            goldData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240); 
            goldData.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            goldData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            goldData.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 
            goldData.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; 
            goldData.ColumnHeadersHeight = 35;
            goldData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing; 
            goldData.EnableHeadersVisualStyles = false; 

            goldData.DefaultCellStyle.BackColor = Color.White;
            goldData.DefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30); 
            goldData.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            goldData.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 250); 
            goldData.DefaultCellStyle.SelectionForeColor = Color.Black;
            goldData.DefaultCellStyle.Padding = new Padding(5); 
            goldData.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; 

            goldData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248); 

            DataGridViewCellStyle numberStyle = new DataGridViewCellStyle(goldData.DefaultCellStyle);
            numberStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            statusStrip.BackColor = Color.FromArgb(230, 230, 230); 
            statusStrip.SizingGrip = false; 

            statusLabel.ForeColor = Color.DimGray;
            statusLabel.Text = "Sẵn sàng.";
        }


        private async void Gold_Load(object sender, EventArgs e)
        {
            await Task.Delay(200);
            await RefreshDataAsync();
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await RefreshDataAsync();
        }

        private async Task RefreshDataAsync()
        {
            refreshButton.Enabled = false; 
            statusLabel.Text = "🔄 Đang tải dữ liệu giá vàng...";
            goldData.DataSource = null; 
            goldData.Rows.Clear(); 
            goldData.Columns.Clear(); 

            try
            {
                string response = await UtilityManager.SendSingleCommandAsync(AppState.CurrentUser, "GOLD");

                if (string.IsNullOrWhiteSpace(response))
                {
                    statusLabel.Text = "❌ Lỗi: Server không phản hồi.";
                    MessageBox.Show("Server không phản hồi hoặc kết nối có vấn đề.", "Lỗi Mạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (response.StartsWith("GOLD_JSON|"))
                {
                    string json = response.Substring("GOLD_JSON|".Length);

                    try
                    {
                        var checkError = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                        if (checkError != null && checkError.ContainsKey("error"))
                        {
                            statusLabel.Text = $"⚠️ Lỗi từ server: {checkError["error"]}";
                            MessageBox.Show($"Lỗi từ server:\n{checkError["error"]}", "Lỗi Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    catch
                    {
                    }


                    var rows = JsonConvert.DeserializeObject<List<GoldRow>>(json);
                    if (rows == null || rows.Count == 0)
                    {
                        statusLabel.Text = "ℹ️ Không có dữ liệu giá vàng để hiển thị.";
                        MessageBox.Show("Không nhận được dữ liệu giá vàng từ server.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    goldData.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "type",
                        HeaderText = "Loại vàng",
                        DataPropertyName = "type", 
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, 
                        FillWeight = 40 
                    });

                    goldData.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "branch",
                        HeaderText = "Chi nhánh",
                        DataPropertyName = "branch",
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                        FillWeight = 30
                    });
                    DataGridViewCellStyle numberStyle = new DataGridViewCellStyle(goldData.DefaultCellStyle);
                    numberStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    numberStyle.Format = "N0"; 

                    goldData.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "buy",
                        HeaderText = "Giá Mua (VNĐ)",
                        DataPropertyName = "buy",
                        DefaultCellStyle = numberStyle, 
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                        FillWeight = 30
                    });
                    goldData.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = "sell",
                        HeaderText = "Giá Bán (VNĐ)",
                        DataPropertyName = "sell",
                        DefaultCellStyle = numberStyle, 
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                        FillWeight = 30
                    });



                    goldData.DataSource = rows;



                    statusLabel.Text = $"✅ Cập nhật thành công lúc {DateTime.Now:HH:mm:ss}";
                }
                else
                {
                    statusLabel.Text = "❌ Lỗi: Phản hồi không hợp lệ từ server.";
                    MessageBox.Show("Dữ liệu nhận được từ server không đúng định dạng:\n" + response, "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (JsonException jsonEx)
            {
                statusLabel.Text = "❌ Lỗi: Không thể xử lý dữ liệu JSON.";
                MessageBox.Show("Lỗi khi phân tích dữ liệu JSON từ server:\n" + jsonEx.Message, "Lỗi JSON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "❌ Lỗi: " + ex.Message;
                MessageBox.Show("Đã xảy ra lỗi không mong muốn:\n" + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                refreshButton.Enabled = true; 
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                if (Menu.Instance != null)
                {
                    Menu.Instance.Show();
                }
                else
                {
                }
            }
            else
            {
                base.OnFormClosing(e);
            }
        }
    }
}
