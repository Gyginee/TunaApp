using Client.ChildForm;
using Client.Models;
using Client.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace Client
{
    public partial class Menu : Form
    {
        private TcpClient _client;
        private string _username;
        private static Menu _instance;

        private Menu()
        {
            InitializeComponent();
            LoadUserData();
        }

        public static Menu Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new Menu();
                }
                return _instance;
            }
        }

        private void LoadUserData()
        {
            if (!string.IsNullOrEmpty(AppState.CurrentUser))
            {
                _username = AppState.CurrentUser;
                userLabel.Text = $"Xin chào {_username}";
            }
        }

        private void chatFormBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Chat().Show();
        }

        private void mailFormBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Mail().Show();
        }

        private void goldFormBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Gold().Show();
        }

        private void webFormBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Web().Show();
        }

        private void hostFormBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Host().Show();
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                await PerformLogoutAndExitAsync();
            }
            else
            {
                base.OnFormClosing(e);
            }
        }

        private async Task PerformLogoutAndExitAsync()
        {
            try
            {
                if (ConnectionManager.IsConnected)
                {
                    await ConnectionManager.SendCommandAsync("LOGOUT");
                    await Task.Delay(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[LOGOUT EXCEPTION] " + ex.Message);
            }
            finally
            {
                AppState.CurrentUser = null;

                MessageManager.Close();
                InformationManager.Close();
                ConnectionManager.Close();

                this.Hide();
                Login.Instance.Show();
            }
        }

        private void pingBtn_Click(object sender, EventArgs e)
        {
            using (var dialog = new PingUserDialog())
            {
                dialog.ShowDialog();
            }
        }


    }

}
