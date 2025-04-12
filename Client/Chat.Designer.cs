namespace Client
{
    partial class Chat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            messageTextBox = new TextBox();
            sendButton = new Button();
            emojiButton = new Button();
            userListView = new ListView();
            sendFileButton = new Button();
            createGroupButton = new Button();
            flowPanelLayout = new FlowLayoutPanel();
            userLabel = new Label();
            SuspendLayout();
            // 
            // messageTextBox
            // 
            messageTextBox.Font = new Font("Open Sans", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            messageTextBox.Location = new Point(129, 408);
            messageTextBox.Multiline = true;
            messageTextBox.Name = "messageTextBox";
            messageTextBox.PlaceholderText = "Nhập tin nhắn...";
            messageTextBox.Size = new Size(529, 52);
            messageTextBox.TabIndex = 1;
            // 
            // sendButton
            // 
            sendButton.Font = new Font("Open Sans", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sendButton.Location = new Point(701, 408);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(80, 52);
            sendButton.TabIndex = 2;
            sendButton.Text = "Gửi";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += SendButton_Click;
            // 
            // emojiButton
            // 
            emojiButton.Location = new Point(664, 407);
            emojiButton.Name = "emojiButton";
            emojiButton.Size = new Size(30, 23);
            emojiButton.TabIndex = 3;
            emojiButton.Text = "😊";
            emojiButton.UseVisualStyleBackColor = true;
            // 
            // userListView
            // 
            userListView.Location = new Point(6, 12);
            userListView.Name = "userListView";
            userListView.Size = new Size(118, 391);
            userListView.TabIndex = 4;
            userListView.UseCompatibleStateImageBehavior = false;
            userListView.View = View.List;
            // 
            // sendFileButton
            // 
            sendFileButton.Location = new Point(664, 437);
            sendFileButton.Name = "sendFileButton";
            sendFileButton.Size = new Size(30, 23);
            sendFileButton.TabIndex = 5;
            sendFileButton.Text = "📁";
            sendFileButton.UseVisualStyleBackColor = true;
            sendFileButton.Click += SendFileButton_Click;
            // 
            // createGroupButton
            // 
            createGroupButton.Font = new Font("Open Sans Medium", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            createGroupButton.Location = new Point(6, 409);
            createGroupButton.Name = "createGroupButton";
            createGroupButton.Size = new Size(117, 51);
            createGroupButton.TabIndex = 6;
            createGroupButton.Text = "Nhóm";
            createGroupButton.UseVisualStyleBackColor = true;
            createGroupButton.Click += CreateGroupButton_Click;
            // 
            // flowPanelLayout
            // 
            flowPanelLayout.Location = new Point(129, 12);
            flowPanelLayout.Name = "flowPanelLayout";
            flowPanelLayout.Size = new Size(651, 391);
            flowPanelLayout.TabIndex = 7;
            // 
            // userLabel
            // 
            userLabel.AutoSize = true;
            userLabel.Font = new Font("Open Sans Light", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            userLabel.Location = new Point(6, 467);
            userLabel.Name = "userLabel";
            userLabel.Size = new Size(44, 23);
            userLabel.TabIndex = 8;
            userLabel.Text = "User";
            // 
            // Chat
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(793, 493);
            Controls.Add(userLabel);
            Controls.Add(flowPanelLayout);
            Controls.Add(createGroupButton);
            Controls.Add(sendFileButton);
            Controls.Add(userListView);
            Controls.Add(emojiButton);
            Controls.Add(sendButton);
            Controls.Add(messageTextBox);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Chat";
            Text = "Chat";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox messageTextBox;
        private Button sendButton;
        private Button emojiButton;
        private ListView userListView;
        private Button sendFileButton;
        private Button createGroupButton;
        private FlowLayoutPanel flowPanelLayout;
        private Label userLabel;
    }
}