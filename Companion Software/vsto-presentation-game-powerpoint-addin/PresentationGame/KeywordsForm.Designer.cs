namespace PresentationGame
{
    partial class KeywordsForm
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddKeyword = new System.Windows.Forms.Button();
            this.btnEditKeyword = new System.Windows.Forms.Button();
            this.btnRemoveKeyword = new System.Windows.Forms.Button();
            this.lstKeywords = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.btnAddKeyword);
            this.flowLayoutPanel1.Controls.Add(this.btnEditKeyword);
            this.flowLayoutPanel1.Controls.Add(this.btnRemoveKeyword);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 325);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(583, 48);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnAddKeyword
            // 
            this.btnAddKeyword.AutoSize = true;
            this.btnAddKeyword.Location = new System.Drawing.Point(3, 3);
            this.btnAddKeyword.Name = "btnAddKeyword";
            this.btnAddKeyword.Size = new System.Drawing.Size(96, 42);
            this.btnAddKeyword.TabIndex = 0;
            this.btnAddKeyword.Text = "Add Keyword...";
            this.btnAddKeyword.UseVisualStyleBackColor = true;
            // 
            // btnEditKeyword
            // 
            this.btnEditKeyword.AutoSize = true;
            this.btnEditKeyword.Location = new System.Drawing.Point(105, 3);
            this.btnEditKeyword.Name = "btnEditKeyword";
            this.btnEditKeyword.Size = new System.Drawing.Size(96, 42);
            this.btnEditKeyword.TabIndex = 2;
            this.btnEditKeyword.Text = "Edit Keyword...";
            this.btnEditKeyword.UseVisualStyleBackColor = true;
            // 
            // btnRemoveKeyword
            // 
            this.btnRemoveKeyword.AutoSize = true;
            this.btnRemoveKeyword.Location = new System.Drawing.Point(207, 3);
            this.btnRemoveKeyword.Name = "btnRemoveKeyword";
            this.btnRemoveKeyword.Size = new System.Drawing.Size(98, 42);
            this.btnRemoveKeyword.TabIndex = 1;
            this.btnRemoveKeyword.Text = "RemoveKeyword";
            this.btnRemoveKeyword.UseVisualStyleBackColor = true;
            // 
            // lstKeywords
            // 
            this.lstKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstKeywords.FormattingEnabled = true;
            this.lstKeywords.Location = new System.Drawing.Point(12, 12);
            this.lstKeywords.Name = "lstKeywords";
            this.lstKeywords.Size = new System.Drawing.Size(583, 303);
            this.lstKeywords.TabIndex = 1;
            // 
            // KeywordsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 385);
            this.Controls.Add(this.lstKeywords);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "KeywordsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Keywords";
            this.Load += new System.EventHandler(this.KeywordsForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ListBox lstKeywords;
        private System.Windows.Forms.Button btnAddKeyword;
        private System.Windows.Forms.Button btnRemoveKeyword;
        private System.Windows.Forms.Button btnEditKeyword;
    }
}