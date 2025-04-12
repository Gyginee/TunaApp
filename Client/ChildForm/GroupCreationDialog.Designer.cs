namespace Client
{
    partial class GroupCreationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupCreationDialog));
            createGroupBtn = new Button();
            joinGroupBtn = new Button();
            groupNameBox = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // createGroupBtn
            // 
            createGroupBtn.Location = new Point(12, 78);
            createGroupBtn.Name = "createGroupBtn";
            createGroupBtn.Size = new Size(205, 65);
            createGroupBtn.TabIndex = 1;
            createGroupBtn.Text = "Tạo nhóm mới";
            createGroupBtn.UseVisualStyleBackColor = true;
            createGroupBtn.Click += CreateGroupBtn_Click;
            // 
            // joinGroupBtn
            // 
            joinGroupBtn.Location = new Point(12, 160);
            joinGroupBtn.Name = "joinGroupBtn";
            joinGroupBtn.Size = new Size(205, 65);
            joinGroupBtn.TabIndex = 2;
            joinGroupBtn.Text = "Tham gia nhóm";
            joinGroupBtn.UseVisualStyleBackColor = true;
            joinGroupBtn.Click += joinGroupButton_Click;
            // 
            // groupNameBox
            // 
            groupNameBox.Location = new Point(12, 38);
            groupNameBox.Name = "groupNameBox";
            groupNameBox.PlaceholderText = "Nhập tên nhóm...";
            groupNameBox.Size = new Size(205, 23);
            groupNameBox.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 20);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 4;
            label1.Text = "Tên nhóm";
            // 
            // GroupCreationDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(230, 245);
            Controls.Add(label1);
            Controls.Add(groupNameBox);
            Controls.Add(joinGroupBtn);
            Controls.Add(createGroupBtn);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "GroupCreationDialog";
            Text = "Nhóm chat";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button createGroupBtn;
        private Button joinGroupBtn;
        private TextBox groupNameBox;
        private Label label1;
    }
}