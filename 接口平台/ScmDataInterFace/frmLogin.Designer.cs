namespace ScmDataInterFace
{
    partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_Password = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_Load = new System.Windows.Forms.Button();
            this.btn_quit = new System.Windows.Forms.Button();
            this.cmb_login = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tb_Password
            // 
            resources.ApplyResources(this.tb_Password, "tb_Password");
            this.tb_Password.Name = "tb_Password";
            this.tb_Password.Tag = "1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label5.Name = "label5";
            // 
            // btn_Load
            // 
            resources.ApplyResources(this.btn_Load, "btn_Load");
            this.btn_Load.Name = "btn_Load";
            this.btn_Load.UseVisualStyleBackColor = true;
            this.btn_Load.Click += new System.EventHandler(this.btn_Load_Click);
            // 
            // btn_quit
            // 
            resources.ApplyResources(this.btn_quit, "btn_quit");
            this.btn_quit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_quit.Name = "btn_quit";
            this.btn_quit.UseVisualStyleBackColor = true;
            this.btn_quit.Click += new System.EventHandler(this.btn_quit_Click);
            // 
            // cmb_login
            // 
            resources.ApplyResources(this.cmb_login, "cmb_login");
            this.cmb_login.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_login.FormattingEnabled = true;
            this.cmb_login.Items.AddRange(new object[] {
            resources.GetString("cmb_login.Items"),
            resources.GetString("cmb_login.Items1")});
            this.cmb_login.Name = "cmb_login";
            this.cmb_login.Tag = "0";
            this.cmb_login.SelectedIndexChanged += new System.EventHandler(this.cmb_login_SelectedIndexChanged);
            // 
            // frmLogin
            // 
            this.AcceptButton = this.btn_Load;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_quit;
            this.Controls.Add(this.cmb_login);
            this.Controls.Add(this.btn_quit);
            this.Controls.Add(this.btn_Load);
            this.Controls.Add(this.tb_Password);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogin";
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_Password;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_Load;
        private System.Windows.Forms.Button btn_quit;
        private System.Windows.Forms.ComboBox cmb_login;
    }
}