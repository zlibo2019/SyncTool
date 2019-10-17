namespace ScmDataInterFace
{
    partial class FrmExecTask
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmExecTask));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.b_cancel = new System.Windows.Forms.Button();
            this.b_ok = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbTheir = new System.Windows.Forms.RadioButton();
            this.rbOur = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.t_sql = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cmbMutexName = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbIsMutex = new System.Windows.Forms.CheckBox();
            this.cb_task = new ScmDataInterFace.UserCombobox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.b_cancel);
            this.panel3.Controls.Add(this.b_ok);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // b_cancel
            // 
            resources.ApplyResources(this.b_cancel, "b_cancel");
            this.b_cancel.Name = "b_cancel";
            this.b_cancel.UseVisualStyleBackColor = true;
            this.b_cancel.Click += new System.EventHandler(this.b_cancel_Click);
            // 
            // b_ok
            // 
            resources.ApplyResources(this.b_ok, "b_ok");
            this.b_ok.Name = "b_ok";
            this.b_ok.UseVisualStyleBackColor = true;
            this.b_ok.Click += new System.EventHandler(this.b_ok_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbTheir);
            this.panel2.Controls.Add(this.rbOur);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.t_sql);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // rbTheir
            // 
            resources.ApplyResources(this.rbTheir, "rbTheir");
            this.rbTheir.Name = "rbTheir";
            this.rbTheir.UseVisualStyleBackColor = true;
            // 
            // rbOur
            // 
            resources.ApplyResources(this.rbOur, "rbOur");
            this.rbOur.Checked = true;
            this.rbOur.Name = "rbOur";
            this.rbOur.TabStop = true;
            this.rbOur.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // t_sql
            // 
            resources.ApplyResources(this.t_sql, "t_sql");
            this.t_sql.Name = "t_sql";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.cbIsMutex);
            this.panel1.Controls.Add(this.cb_task);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.cmbMutexName);
            this.panel4.Controls.Add(this.label3);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // cmbMutexName
            // 
            this.cmbMutexName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMutexName.FormattingEnabled = true;
            this.cmbMutexName.Items.AddRange(new object[] {
            resources.GetString("cmbMutexName.Items"),
            resources.GetString("cmbMutexName.Items1"),
            resources.GetString("cmbMutexName.Items2"),
            resources.GetString("cmbMutexName.Items3"),
            resources.GetString("cmbMutexName.Items4"),
            resources.GetString("cmbMutexName.Items5"),
            resources.GetString("cmbMutexName.Items6"),
            resources.GetString("cmbMutexName.Items7"),
            resources.GetString("cmbMutexName.Items8"),
            resources.GetString("cmbMutexName.Items9"),
            resources.GetString("cmbMutexName.Items10"),
            resources.GetString("cmbMutexName.Items11"),
            resources.GetString("cmbMutexName.Items12"),
            resources.GetString("cmbMutexName.Items13")});
            resources.ApplyResources(this.cmbMutexName, "cmbMutexName");
            this.cmbMutexName.Name = "cmbMutexName";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cbIsMutex
            // 
            resources.ApplyResources(this.cbIsMutex, "cbIsMutex");
            this.cbIsMutex.Name = "cbIsMutex";
            this.cbIsMutex.UseVisualStyleBackColor = true;
            this.cbIsMutex.CheckedChanged += new System.EventHandler(this.cbIsMutex_CheckedChanged);
            // 
            // cb_task
            // 
            this.cb_task.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cb_task.FormattingEnabled = true;
            resources.ApplyResources(this.cb_task, "cb_task");
            this.cb_task.Name = "cb_task";
            this.cb_task.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cb_task_KeyDown);
            this.cb_task.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cb_task_KeyPress);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FrmExecTask
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FrmExecTask";
            this.Load += new System.EventHandler(this.FrmExecTask_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private UserCombobox cb_task;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button b_ok;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox t_sql;
        private System.Windows.Forms.Button b_cancel;
        private System.Windows.Forms.RadioButton rbTheir;
        private System.Windows.Forms.RadioButton rbOur;
        private System.Windows.Forms.CheckBox cbIsMutex;
        private System.Windows.Forms.ComboBox cmbMutexName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel4;
    }
}