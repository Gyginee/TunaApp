using Client.Models;
using Client.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class GroupCreationDialog : Form
    {
        private readonly Action<string> _onGroupJoined;

        public GroupCreationDialog(Action<string> onGroupJoined)
        {
            InitializeComponent();
            _onGroupJoined = onGroupJoined;
        }

        private async void CreateGroupBtn_Click(object sender, EventArgs e)
        {
            string groupName = groupNameBox.Text;
            string members = AppState.CurrentUser;

            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(members))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            if (!InformationManager.IsConnected)
            {
                bool connected = await InformationManager.InitializeAsync(AppState.CurrentUser);
                if (!connected)
                {
                    MessageBox.Show("Không thể kết nối tới server.");
                    return;
                }
            }

            string command = $"CREATE_GROUP|{groupName}|{members}";
            await InformationManager.SendCommandAsync(command);
            MessageBox.Show("Đã gửi yêu cầu tạo nhóm.");

            _onGroupJoined?.Invoke(groupName); // <<< callback gọi về form Chat
            Close();
        }

        private async void joinGroupButton_Click(object sender, EventArgs e)
        {
            string groupName = groupNameBox.Text;
            if (string.IsNullOrWhiteSpace(groupName)) return;

            if (!InformationManager.IsConnected)
            {
                bool connected = await InformationManager.InitializeAsync(AppState.CurrentUser);
                if (!connected)
                {
                    MessageBox.Show("Không thể kết nối server.");
                    return;
                }
            }

            string command = $"JOIN_GROUP|{groupName}|{AppState.CurrentUser}";
            await InformationManager.SendCommandAsync(command);

            _onGroupJoined?.Invoke(groupName); // <<< callback luôn khi join
            Close();
        }


    }

}
