namespace ScmDataInterFace
{
    partial class Form4
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form4));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.t_jiqima = new System.Windows.Forms.TextBox();
            this.t_zhucema = new System.Windows.Forms.TextBox();
            this.b_zhuce = new System.Windows.Forms.Button();
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
            // t_jiqima
            // 
            resources.ApplyResources(this.t_jiqima, "t_jiqima");
            this.t_jiqima.Name = "t_jiqima";
            this.t_jiqima.ReadOnly = true;
            // 
            // t_zhucema
            // 
            resources.ApplyResources(this.t_zhucema, "t_zhucema");
            this.t_zhucema.Name = "t_zhucema";
            // 
            // b_zhuce
            // 
            resources.ApplyResources(this.b_zhuce, "b_zhuce");
            this.b_zhuce.Name = "b_zhuce";
            this.b_zhuce.UseVisualStyleBackColor = true;
            this.b_zhuce.Click += new System.EventHandler(this.b_zhuce_Click);
            // 
            // Form4
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.b_zhuce);
            this.Controls.Add(this.t_zhucema);
            this.Controls.Add(this.t_jiqima);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form4";
            this.Load += new System.EventHandler(this.Form4_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox t_jiqima;
        private System.Windows.Forms.TextBox t_zhucema;
        private System.Windows.Forms.Button b_zhuce;
    }
}