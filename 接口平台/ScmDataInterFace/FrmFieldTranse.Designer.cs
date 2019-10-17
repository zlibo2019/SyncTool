namespace ScmDataInterFace
{
    partial class FrmFieldTranse
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.b_close = new System.Windows.Forms.Button();
            this.b_save = new System.Windows.Forms.Button();
            this.dgv_Fields = new System.Windows.Forms.DataGridView();
            this.TField_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field_Desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CanNull = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IsTrans = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.convertCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.t_OurTableName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.t_TheirTableName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.t_group = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.t_where = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Fields)).BeginInit();
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
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.dgv_Fields, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(919, 627);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.b_close);
            this.panel2.Controls.Add(this.b_save);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 589);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(911, 34);
            this.panel2.TabIndex = 2;
            // 
            // b_close
            // 
            this.b_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_close.Location = new System.Drawing.Point(823, 8);
            this.b_close.Name = "b_close";
            this.b_close.Size = new System.Drawing.Size(75, 23);
            this.b_close.TabIndex = 1;
            this.b_close.Text = "关闭";
            this.b_close.UseVisualStyleBackColor = true;
            this.b_close.Click += new System.EventHandler(this.b_close_Click);
            // 
            // b_save
            // 
            this.b_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_save.Location = new System.Drawing.Point(741, 8);
            this.b_save.Name = "b_save";
            this.b_save.Size = new System.Drawing.Size(75, 23);
            this.b_save.TabIndex = 0;
            this.b_save.Text = "保存";
            this.b_save.UseVisualStyleBackColor = true;
            this.b_save.Click += new System.EventHandler(this.b_save_Click);
            // 
            // dgv_Fields
            // 
            this.dgv_Fields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Fields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TField_Name,
            this.Field_Name,
            this.Field_Desc,
            this.Field_Type,
            this.CanNull,
            this.IsTrans,
            this.convertCol});
            this.dgv_Fields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Fields.Location = new System.Drawing.Point(4, 96);
            this.dgv_Fields.Name = "dgv_Fields";
            this.dgv_Fields.RowTemplate.Height = 23;
            this.dgv_Fields.Size = new System.Drawing.Size(911, 365);
            this.dgv_Fields.TabIndex = 0;
            this.dgv_Fields.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgv_Fields_RowsAdded);
            // 
            // TField_Name
            // 
            this.TField_Name.DataPropertyName = "TField_Name";
            this.TField_Name.HeaderText = "对方字段名";
            this.TField_Name.Name = "TField_Name";
            // 
            // Field_Name
            // 
            this.Field_Name.DataPropertyName = "Field_Name";
            this.Field_Name.HeaderText = "我方字段名";
            this.Field_Name.Name = "Field_Name";
            // 
            // Field_Desc
            // 
            this.Field_Desc.DataPropertyName = "Field_Desc";
            this.Field_Desc.HeaderText = "我方字段描述";
            this.Field_Desc.Name = "Field_Desc";
            // 
            // Field_Type
            // 
            this.Field_Type.DataPropertyName = "Field_Type";
            this.Field_Type.HeaderText = "我方字段类型";
            this.Field_Type.Name = "Field_Type";
            this.Field_Type.Width = 120;
            // 
            // CanNull
            // 
            this.CanNull.DataPropertyName = "CanNull";
            this.CanNull.FalseValue = "N";
            this.CanNull.HeaderText = "我方字段可为空";
            this.CanNull.IndeterminateValue = "";
            this.CanNull.Name = "CanNull";
            this.CanNull.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CanNull.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.CanNull.TrueValue = "Y";
            // 
            // IsTrans
            // 
            this.IsTrans.DataPropertyName = "IsTrans";
            this.IsTrans.FalseValue = "N";
            this.IsTrans.HeaderText = "是否传递";
            this.IsTrans.IndeterminateValue = "";
            this.IsTrans.Name = "IsTrans";
            this.IsTrans.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.IsTrans.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsTrans.TrueValue = "Y";
            this.IsTrans.Width = 80;
            // 
            // convertCol
            // 
            this.convertCol.DataPropertyName = "convertCol";
            this.convertCol.HeaderText = "转换";
            this.convertCol.Name = "convertCol";
            this.convertCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.convertCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.convertCol.Width = 400;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(911, 54);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("宋体", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(181, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(601, 36);
            this.label1.TabIndex = 0;
            this.label1.Text = "双方列映射和转换";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button2);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.t_OurTableName);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.t_TheirTableName);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(4, 65);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(911, 24);
            this.panel3.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(608, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "导入";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(527, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "导出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // t_OurTableName
            // 
            this.t_OurTableName.Enabled = false;
            this.t_OurTableName.Location = new System.Drawing.Point(336, 2);
            this.t_OurTableName.Name = "t_OurTableName";
            this.t_OurTableName.Size = new System.Drawing.Size(153, 21);
            this.t_OurTableName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(269, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "我方表名：";
            // 
            // t_TheirTableName
            // 
            this.t_TheirTableName.Enabled = false;
            this.t_TheirTableName.Location = new System.Drawing.Point(74, 2);
            this.t_TheirTableName.Name = "t_TheirTableName";
            this.t_TheirTableName.Size = new System.Drawing.Size(153, 21);
            this.t_TheirTableName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "对方表名：";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.t_group);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.t_where);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(4, 468);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(911, 114);
            this.panel4.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Location = new System.Drawing.Point(467, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(401, 24);
            this.label6.TabIndex = 5;
            this.label6.Text = "注：若输入分组内容，则导入导出时数据将分组放入内存，以节省内存空间\r\n这里只需要输入字段，多个字段之间用逗号隔开，如：xh,bh";
            // 
            // t_group
            // 
            this.t_group.Location = new System.Drawing.Point(469, 19);
            this.t_group.Multiline = true;
            this.t_group.Name = "t_group";
            this.t_group.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.t_group.Size = new System.Drawing.Size(418, 58);
            this.t_group.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(467, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "源数据分组字段：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label5.Location = new System.Drawing.Point(9, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(389, 24);
            this.label5.TabIndex = 2;
            this.label5.Text = "注：若需要对源数据进行筛选，则输入筛选内容，如：Where xh > 10000\r\nand xh < 20000";
            // 
            // t_where
            // 
            this.t_where.Location = new System.Drawing.Point(11, 19);
            this.t_where.Multiline = true;
            this.t_where.Name = "t_where";
            this.t_where.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.t_where.Size = new System.Drawing.Size(418, 58);
            this.t_where.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "源数据筛选条件：";
            // 
            // FrmFieldTranse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.ClientSize = new System.Drawing.Size(919, 627);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FrmFieldTranse";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "列映射和转换";
            this.Load += new System.EventHandler(this.FrmFieldTranse_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Fields)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button b_close;
        private System.Windows.Forms.Button b_save;
        private System.Windows.Forms.DataGridView dgv_Fields;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox t_OurTableName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox t_TheirTableName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox t_where;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox t_group;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn TField_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field_Desc;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field_Type;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CanNull;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsTrans;
        private System.Windows.Forms.DataGridViewTextBoxColumn convertCol;
    }
}