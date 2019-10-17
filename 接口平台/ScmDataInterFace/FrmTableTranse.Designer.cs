namespace ScmDataInterFace
{
    partial class FrmTableTranse
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgv_task = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.b_addtable = new System.Windows.Forms.Button();
            this.b_close = new System.Windows.Forms.Button();
            this.b_save = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.t_taskname = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.t_task = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Task_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Task_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TheirTableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TheirSql = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OurTableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OurTable_desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OurSql = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.interfaceDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.direction = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.IncrementInsert = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DeleteNotDrop = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DropTable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.del = new System.Windows.Forms.DataGridViewButtonColumn();
            this.GroupSql = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupField = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_task)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dgv_task, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(901, 576);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dgv_task
            // 
            this.dgv_task.AllowUserToAddRows = false;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            this.dgv_task.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_task.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_task.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.Task_id,
            this.Task_Name,
            this.TheirTableName,
            this.TheirSql,
            this.OurTableName,
            this.OurTable_desc,
            this.OurSql,
            this.interfaceDesc,
            this.direction,
            this.IncrementInsert,
            this.DeleteNotDrop,
            this.DropTable,
            this.del,
            this.GroupSql,
            this.GroupField});
            this.dgv_task.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_task.Location = new System.Drawing.Point(4, 117);
            this.dgv_task.Name = "dgv_task";
            this.dgv_task.RowTemplate.Height = 23;
            this.dgv_task.Size = new System.Drawing.Size(893, 414);
            this.dgv_task.TabIndex = 0;
            this.dgv_task.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgv_task_CellBeginEdit);
            this.dgv_task.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_task_CellClick);
            this.dgv_task.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_task_CellDoubleClick);
            this.dgv_task.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_task_CellEndEdit);
            this.dgv_task.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_task_CellValueChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.b_addtable);
            this.panel2.Controls.Add(this.b_close);
            this.panel2.Controls.Add(this.b_save);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 538);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(893, 34);
            this.panel2.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(233, 8);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "下移";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(152, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "上移";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // b_addtable
            // 
            this.b_addtable.Location = new System.Drawing.Point(9, 8);
            this.b_addtable.Name = "b_addtable";
            this.b_addtable.Size = new System.Drawing.Size(75, 23);
            this.b_addtable.TabIndex = 2;
            this.b_addtable.Text = "添加表";
            this.b_addtable.UseVisualStyleBackColor = true;
            this.b_addtable.Click += new System.EventHandler(this.b_addtable_Click);
            // 
            // b_close
            // 
            this.b_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_close.Location = new System.Drawing.Point(809, 8);
            this.b_close.Name = "b_close";
            this.b_close.Size = new System.Drawing.Size(75, 23);
            this.b_close.TabIndex = 1;
            this.b_close.Text = "关闭";
            this.b_close.UseVisualStyleBackColor = true;
            this.b_close.Visible = false;
            this.b_close.Click += new System.EventHandler(this.b_close_Click);
            // 
            // b_save
            // 
            this.b_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_save.Location = new System.Drawing.Point(727, 8);
            this.b_save.Name = "b_save";
            this.b_save.Size = new System.Drawing.Size(75, 23);
            this.b_save.TabIndex = 0;
            this.b_save.Text = "保存";
            this.b_save.UseVisualStyleBackColor = true;
            this.b_save.Click += new System.EventHandler(this.b_save_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(893, 54);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("宋体", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(155, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(583, 36);
            this.label1.TabIndex = 0;
            this.label1.Text = "双方数据导入导出管理";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.t_taskname);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.t_task);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(4, 65);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(893, 24);
            this.panel3.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label4.Location = new System.Drawing.Point(517, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(212, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "注：任务将以在表格中的先后顺序执行";
            // 
            // t_taskname
            // 
            this.t_taskname.Location = new System.Drawing.Point(336, 3);
            this.t_taskname.Name = "t_taskname";
            this.t_taskname.Size = new System.Drawing.Size(153, 21);
            this.t_taskname.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(269, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "任务名称：";
            // 
            // t_task
            // 
            this.t_task.Enabled = false;
            this.t_task.Location = new System.Drawing.Point(74, 0);
            this.t_task.Name = "t_task";
            this.t_task.Size = new System.Drawing.Size(153, 21);
            this.t_task.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "任务编号：";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(4, 96);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(893, 14);
            this.panel4.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Maroon;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(893, 14);
            this.label5.TabIndex = 12;
            this.label5.Text = "鼠标双击进行编辑";
            // 
            // id
            // 
            this.id.DataPropertyName = "id";
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Visible = false;
            // 
            // Task_id
            // 
            this.Task_id.DataPropertyName = "Task_id";
            this.Task_id.HeaderText = "任务编号";
            this.Task_id.Name = "Task_id";
            this.Task_id.Visible = false;
            // 
            // Task_Name
            // 
            this.Task_Name.DataPropertyName = "Task_Name";
            this.Task_Name.HeaderText = "任务名称";
            this.Task_Name.Name = "Task_Name";
            this.Task_Name.Visible = false;
            // 
            // TheirTableName
            // 
            this.TheirTableName.DataPropertyName = "TheirTableName";
            this.TheirTableName.HeaderText = "对方表名";
            this.TheirTableName.Name = "TheirTableName";
            this.TheirTableName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TheirTableName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TheirTableName.Width = 180;
            // 
            // TheirSql
            // 
            this.TheirSql.DataPropertyName = "TheirSql";
            this.TheirSql.HeaderText = "对方语句";
            this.TheirSql.Name = "TheirSql";
            this.TheirSql.Visible = false;
            // 
            // OurTableName
            // 
            this.OurTableName.DataPropertyName = "OurTableName";
            this.OurTableName.HeaderText = "我方表名";
            this.OurTableName.Name = "OurTableName";
            this.OurTableName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.OurTableName.Width = 180;
            // 
            // OurTable_desc
            // 
            this.OurTable_desc.DataPropertyName = "OurTable_desc";
            this.OurTable_desc.HeaderText = "说明";
            this.OurTable_desc.Name = "OurTable_desc";
            this.OurTable_desc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.OurTable_desc.Width = 90;
            // 
            // OurSql
            // 
            this.OurSql.DataPropertyName = "OurSql";
            this.OurSql.HeaderText = "我方语句";
            this.OurSql.Name = "OurSql";
            this.OurSql.Visible = false;
            // 
            // interfaceDesc
            // 
            this.interfaceDesc.DataPropertyName = "interfaceDesc";
            this.interfaceDesc.HeaderText = "界面描述";
            this.interfaceDesc.Name = "interfaceDesc";
            this.interfaceDesc.Visible = false;
            // 
            // direction
            // 
            this.direction.DataPropertyName = "direction";
            this.direction.HeaderText = "传递方向";
            this.direction.Items.AddRange(new object[] {
            "0",
            "1"});
            this.direction.Name = "direction";
            this.direction.Width = 150;
            // 
            // IncrementInsert
            // 
            this.IncrementInsert.DataPropertyName = "IncrementInsert";
            this.IncrementInsert.FalseValue = "N";
            this.IncrementInsert.HeaderText = "增量同步";
            this.IncrementInsert.Name = "IncrementInsert";
            this.IncrementInsert.TrueValue = "Y";
            this.IncrementInsert.Width = 50;
            // 
            // DeleteNotDrop
            // 
            this.DeleteNotDrop.DataPropertyName = "DeleteNotDrop";
            this.DeleteNotDrop.FalseValue = "N";
            this.DeleteNotDrop.HeaderText = "清空表";
            this.DeleteNotDrop.Name = "DeleteNotDrop";
            this.DeleteNotDrop.TrueValue = "Y";
            this.DeleteNotDrop.Width = 50;
            // 
            // DropTable
            // 
            this.DropTable.DataPropertyName = "DropTable";
            this.DropTable.FalseValue = "N";
            this.DropTable.HeaderText = "删除表";
            this.DropTable.Name = "DropTable";
            this.DropTable.TrueValue = "Y";
            this.DropTable.Width = 50;
            // 
            // del
            // 
            this.del.DataPropertyName = "del";
            this.del.HeaderText = "删除";
            this.del.Name = "del";
            this.del.Width = 90;
            // 
            // GroupSql
            // 
            this.GroupSql.DataPropertyName = "GroupSql";
            this.GroupSql.HeaderText = "GroupSql";
            this.GroupSql.Name = "GroupSql";
            this.GroupSql.Visible = false;
            // 
            // GroupField
            // 
            this.GroupField.DataPropertyName = "GroupField";
            this.GroupField.HeaderText = "GroupField";
            this.GroupField.Name = "GroupField";
            this.GroupField.Visible = false;
            // 
            // FrmTableTranse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.ClientSize = new System.Drawing.Size(901, 576);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FrmTableTranse";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据导入/导出";
            this.Load += new System.EventHandler(this.FrmTableTranse_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_task)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgv_task;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button b_addtable;
        private System.Windows.Forms.Button b_close;
        private System.Windows.Forms.Button b_save;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox t_task;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox t_taskname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Task_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Task_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn TheirTableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TheirSql;
        private System.Windows.Forms.DataGridViewTextBoxColumn OurTableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OurTable_desc;
        private System.Windows.Forms.DataGridViewTextBoxColumn OurSql;
        private System.Windows.Forms.DataGridViewTextBoxColumn interfaceDesc;
        private System.Windows.Forms.DataGridViewComboBoxColumn direction;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IncrementInsert;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DeleteNotDrop;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DropTable;
        private System.Windows.Forms.DataGridViewButtonColumn del;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupSql;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupField;
    }
}