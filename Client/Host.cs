using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Server.Handlers;
namespace Client
{
    public partial class Host : Form
    {

        public Host()
        {
            InitializeComponent();

            startBtn.Click += StartBtn_Click;
            stopBtn.Click += StopBtn_Click;

            Server.Handlers.HostHandlers.LogCallback = AppendLog;
        }

        private void AppendLog(string message)
        {
            if (logTextBox.InvokeRequired)
            {
                logTextBox.Invoke(new Action<string>(AppendLog), message);
                return;
            }

            logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        }
        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(portNumber.Text, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Port không hợp lệ.");
                return;
            }

            Server.Handlers.HostHandlers.LogCallback = (msg) =>
            {
                if (InvokeRequired)
                    Invoke(new Action(() => logTextBox.AppendText(msg + "\r\n")));
                else
                    logTextBox.AppendText(msg + "\r\n");
            };

            Server.Handlers.HostHandlers.StartWebHosting(port);
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            Server.Handlers.HostHandlers.StopWebHosting();
        }


        protected override async void OnFormClosing(FormClosingEventArgs e)
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
