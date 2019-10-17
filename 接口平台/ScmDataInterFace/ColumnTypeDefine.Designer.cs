namespace ScmDataInterFace
{
    partial class ColumnTypeDefine
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv_columntype = new System.Windows.Forms.DataGridView();
            this.columntype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDecimal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.b_ok = new System.Windows.Forms.Button();
            this.b_cancel = new System.Windows.Forms.Button();
            this.message = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_columntype)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_columntype
            // 
            this.dgv_columntype.AllowUserToAddRows = false;
            this.dgv_columntype.AllowUserToDeleteRows = false;
            this.dgv_columntype.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_columntype.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_columntype.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_columntype.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columntype,
            this.ColumnLength,
            this.ColumnDecimal});
            this.dgv_columntype.Location = new System.Drawing.Point(2, 2);
            this.dgv_columntype.MultiSelect = false;
            this.dgv_columntype.Name = "dgv_columntype";
            this.dgv_columntype.RowHeadersWidth = 24;
            this.dgv_columntype.RowTemplate.Height = 23;
            this.dgv_columntype.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_columntype.Size = new System.Drawing.Size(354, 284);
            this.dgv_columntype.TabIndex = 0;
            this.dgv_columntype.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgv_columntype_CellValidating);
            this.dgv_columntype.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_columntype_CellEnter);
            // 
            // columntype
            // 
            this.columntype.HeaderText = "类型";
            this.columntype.Name = "columntype";
            this.columntype.ReadOnly = true;
            this.columntype.Width = 150;
            // 
            // ColumnLength
            // 
            dataGridViewCellStyle2.Format = "N2";
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnLength.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnLength.HeaderText = "长度";
            this.ColumnLength.Name = "ColumnLength";
            this.ColumnLength.Width = 80;
            // 
            // ColumnDecimal
            // 
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnDecimal.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnDecimal.HeaderText = "小数";
            this.ColumnDecimal.Name = "ColumnDecimal";
            this.ColumnDecimal.Width = 60;
            // 
            // b_ok
            // 
            this.b_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_ok.Location = new System.Drawing.Point(179, 292);
            this.b_ok.Name = "b_ok";
            this.b_ok.Size = new System.Drawing.Size(75, 23);
            this.b_ok.TabIndex = 1;
            this.b_ok.Text = "确认";
            this.b_ok.UseVisualStyleBackColor = true;
            this.b_ok.Click += new System.EventHandler(this.b_ok_Click);
            // 
            // b_cancel
            // 
            this.b_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.b_cancel.Location = new System.Drawing.Point(271, 292);
            this.b_cancel.Name = "b_cancel";
            this.b_cancel.Size = new System.Drawing.Size(75, 23);
            this.b_cancel.TabIndex = 2;
            this.b_cancel.Text = "取消";
            this.b_cancel.UseVisualStyleBackColor = true;
            // 
            // message
            // 
            this.message.AutoSize = true;
            this.message.ForeColor = System.Drawing.Color.Red;
            this.message.Location = new System.Drawing.Point(7, 296);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(0, 12);
            this.message.TabIndex = 3;
            // 
            // ColumnTypeDefine
            // 
            this.AcceptButton = this.b_ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.CancelButton = this.b_cancel;
            this.ClientSize = new System.Drawing.Size(358, 318);
            this.Controls.Add(this.message);
            this.Controls.Add(this.b_cancel);
            this.Controls.Add(this.b_ok);
            this.Controls.Add(this.dgv_columntype);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColumnTypeDefine";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "列类型定义";
            this.Load += new System.EventHandler(this.ColumnTypeDefine_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_columntype)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_columntype;
        private System.Windows.Forms.Button b_ok;
        private System.Windows.Forms.Button b_cancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn columntype;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDecimal;
        private System.Windows.Forms.Label message;
    }
}