namespace ScmDataInterFace
{
    partial class EditForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgv_columns = new System.Windows.Forms.DataGridView();
            this.Table_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field_Desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsPK = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CanNull = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ShowIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ISIDENTITY = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tb_TableName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_TableDesc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ErrorText = new System.Windows.Forms.Label();
            this.b_cancel = new System.Windows.Forms.Button();
            this.b_ok = new System.Windows.Forms.Button();
            this.eP1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.cb_CompareTable = new ScmDataInterFace.UserCombobox();
            this.cb_DatabaseTable = new ScmDataInterFace.UserCombobox();
            this.ucbTablePosition = new ScmDataInterFace.UserCombobox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_columns)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eP1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dgv_columns, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(661, 347);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dgv_columns
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.LightCyan;
            this.dgv_columns.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgv_columns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_columns.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.dgv_columns.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.LightBlue;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_columns.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgv_columns.ColumnHeadersHeight = 40;
            this.dgv_columns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Table_Name,
            this.Field_Name,
            this.Field_Desc,
            this.Field_Type,
            this.IsPK,
            this.CanNull,
            this.ShowIndex,
            this.ISIDENTITY});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.AliceBlue;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.LightPink;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_columns.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgv_columns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_columns.EnableHeadersVisualStyles = false;
            this.dgv_columns.GridColor = System.Drawing.Color.LightSkyBlue;
            this.dgv_columns.Location = new System.Drawing.Point(5, 73);
            this.dgv_columns.MultiSelect = false;
            this.dgv_columns.Name = "dgv_columns";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.PowderBlue;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightPink;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_columns.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgv_columns.RowHeadersWidth = 20;
            this.dgv_columns.RowTemplate.Height = 32;
            this.dgv_columns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_columns.Size = new System.Drawing.Size(651, 235);
            this.dgv_columns.TabIndex = 1;
            this.dgv_columns.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_columns_CellClick);
            this.dgv_columns.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_columns_CellEnter);
            this.dgv_columns.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgv_columns_CellValidating);
            this.dgv_columns.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgv_columns_KeyDown);
            // 
            // Table_Name
            // 
            this.Table_Name.DataPropertyName = "Table_Name";
            this.Table_Name.HeaderText = "表名";
            this.Table_Name.Name = "Table_Name";
            this.Table_Name.Visible = false;
            // 
            // Field_Name
            // 
            this.Field_Name.DataPropertyName = "Field_Name";
            this.Field_Name.FillWeight = 31.60509F;
            this.Field_Name.HeaderText = "列名";
            this.Field_Name.Name = "Field_Name";
            // 
            // Field_Desc
            // 
            this.Field_Desc.DataPropertyName = "Field_Desc";
            this.Field_Desc.FillWeight = 47.1304F;
            this.Field_Desc.HeaderText = "中文列名";
            this.Field_Desc.Name = "Field_Desc";
            // 
            // Field_Type
            // 
            this.Field_Type.DataPropertyName = "Field_Type";
            this.Field_Type.FillWeight = 31.60509F;
            this.Field_Type.HeaderText = "类型定义";
            this.Field_Type.Name = "Field_Type";
            this.Field_Type.ReadOnly = true;
            // 
            // IsPK
            // 
            this.IsPK.DataPropertyName = "IsPK";
            this.IsPK.FalseValue = "N";
            this.IsPK.FillWeight = 25F;
            this.IsPK.HeaderText = "主键";
            this.IsPK.Name = "IsPK";
            this.IsPK.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.IsPK.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsPK.TrueValue = "Y";
            // 
            // CanNull
            // 
            this.CanNull.DataPropertyName = "CanNull";
            this.CanNull.FalseValue = "N";
            this.CanNull.FillWeight = 25F;
            this.CanNull.HeaderText = "允许空";
            this.CanNull.Name = "CanNull";
            this.CanNull.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CanNull.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.CanNull.TrueValue = "Y";
            // 
            // ShowIndex
            // 
            this.ShowIndex.DataPropertyName = "ShowIndex";
            this.ShowIndex.HeaderText = "显示顺序";
            this.ShowIndex.Name = "ShowIndex";
            this.ShowIndex.Visible = false;
            // 
            // ISIDENTITY
            // 
            this.ISIDENTITY.DataPropertyName = "ISIDENTITY";
            this.ISIDENTITY.FalseValue = "N";
            this.ISIDENTITY.FillWeight = 25F;
            this.ISIDENTITY.HeaderText = "自增列";
            this.ISIDENTITY.Name = "ISIDENTITY";
            this.ISIDENTITY.TrueValue = "Y";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cb_CompareTable);
            this.panel1.Controls.Add(this.cb_DatabaseTable);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ucbTablePosition);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(651, 26);
            this.panel1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(426, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "对应正式表:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(175, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "数据库现有表:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "表位置:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tb_TableName);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tb_TableDesc);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(5, 39);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(651, 26);
            this.panel2.TabIndex = 1;
            // 
            // tb_TableName
            // 
            this.tb_TableName.AutoCompleteCustomSource.AddRange(new string[] {
            "td_",
            "kq_",
            "xf_",
            "mj_",
            "sq_"});
            this.tb_TableName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tb_TableName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tb_TableName.Location = new System.Drawing.Point(71, 2);
            this.tb_TableName.Name = "tb_TableName";
            this.tb_TableName.Size = new System.Drawing.Size(208, 21);
            this.tb_TableName.TabIndex = 0;
            this.tb_TableName.Validating += new System.ComponentModel.CancelEventHandler(this.tb_TableName_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "数据表名:";
            // 
            // tb_TableDesc
            // 
            this.tb_TableDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_TableDesc.Location = new System.Drawing.Point(382, 3);
            this.tb_TableDesc.Name = "tb_TableDesc";
            this.tb_TableDesc.Size = new System.Drawing.Size(264, 21);
            this.tb_TableDesc.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(329, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "表描述:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ErrorText);
            this.panel3.Controls.Add(this.b_cancel);
            this.panel3.Controls.Add(this.b_ok);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(5, 316);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(651, 26);
            this.panel3.TabIndex = 4;
            // 
            // ErrorText
            // 
            this.ErrorText.AutoSize = true;
            this.ErrorText.ForeColor = System.Drawing.Color.Red;
            this.ErrorText.Location = new System.Drawing.Point(6, 8);
            this.ErrorText.Name = "ErrorText";
            this.ErrorText.Size = new System.Drawing.Size(0, 12);
            this.ErrorText.TabIndex = 2;
            // 
            // b_cancel
            // 
            this.b_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.b_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.b_cancel.Location = new System.Drawing.Point(559, 3);
            this.b_cancel.Name = "b_cancel";
            this.b_cancel.Size = new System.Drawing.Size(75, 23);
            this.b_cancel.TabIndex = 1;
            this.b_cancel.Text = "取消";
            this.b_cancel.UseVisualStyleBackColor = true;
            this.b_cancel.Click += new System.EventHandler(this.b_cancel_Click);
            // 
            // b_ok
            // 
            this.b_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.b_ok.Location = new System.Drawing.Point(466, 3);
            this.b_ok.Name = "b_ok";
            this.b_ok.Size = new System.Drawing.Size(75, 23);
            this.b_ok.TabIndex = 0;
            this.b_ok.Text = "确认";
            this.b_ok.UseVisualStyleBackColor = true;
            this.b_ok.Click += new System.EventHandler(this.b_ok_Click);
            // 
            // eP1
            // 
            this.eP1.ContainerControl = this;
            // 
            // cb_CompareTable
            // 
            this.cb_CompareTable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cb_CompareTable.FormattingEnabled = true;
            this.cb_CompareTable.Location = new System.Drawing.Point(500, 3);
            this.cb_CompareTable.Name = "cb_CompareTable";
            this.cb_CompareTable.Size = new System.Drawing.Size(146, 22);
            this.cb_CompareTable.TabIndex = 7;
            this.cb_CompareTable.SelectedIndexChanged += new System.EventHandler(this.cb_Formtype_ID_SelectedIndexChanged);
            this.cb_CompareTable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cb_CompareTable_KeyDown);
            this.cb_CompareTable.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cb_CompareTable_KeyPress);
            // 
            // cb_DatabaseTable
            // 
            this.cb_DatabaseTable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cb_DatabaseTable.FormattingEnabled = true;
            this.cb_DatabaseTable.Location = new System.Drawing.Point(259, 3);
            this.cb_DatabaseTable.Name = "cb_DatabaseTable";
            this.cb_DatabaseTable.Size = new System.Drawing.Size(161, 22);
            this.cb_DatabaseTable.TabIndex = 9;
            this.cb_DatabaseTable.SelectedIndexChanged += new System.EventHandler(this.cb_DatabaseTable_SelectedIndexChanged);
            this.cb_DatabaseTable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cb_CompareTable_KeyDown);
            this.cb_DatabaseTable.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cb_CompareTable_KeyPress);
            // 
            // ucbTablePosition
            // 
            this.ucbTablePosition.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ucbTablePosition.FormattingEnabled = true;
            this.ucbTablePosition.Items.AddRange(new object[] {
            "我方",
            "对方"});
            this.ucbTablePosition.Location = new System.Drawing.Point(60, 1);
            this.ucbTablePosition.Name = "ucbTablePosition";
            this.ucbTablePosition.Size = new System.Drawing.Size(98, 22);
            this.ucbTablePosition.TabIndex = 8;
            this.ucbTablePosition.Tag = "0";
            this.ucbTablePosition.SelectedIndexChanged += new System.EventHandler(this.userCombobox1_SelectedIndexChanged);
            // 
            // EditForm
            // 
            this.AcceptButton = this.b_ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.CancelButton = this.b_cancel;
            this.ClientSize = new System.Drawing.Size(661, 347);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "编辑表单";
            this.Load += new System.EventHandler(this.EditFormType_Load);
            this.Leave += new System.EventHandler(this.EditForm_Leave);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_columns)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eP1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgv_columns;
        private System.Windows.Forms.TextBox tb_TableDesc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_TableName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button b_cancel;
        private System.Windows.Forms.Button b_ok;
        private System.Windows.Forms.ErrorProvider eP1;
        private System.Windows.Forms.Label ErrorText;
        private System.Windows.Forms.Label label4;
        private UserCombobox cb_CompareTable;
        private UserCombobox cb_DatabaseTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Table_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field_Desc;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field_Type;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsPK;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CanNull;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShowIndex;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ISIDENTITY;
        private UserCombobox ucbTablePosition;
        private System.Windows.Forms.Label label5;
    }
}