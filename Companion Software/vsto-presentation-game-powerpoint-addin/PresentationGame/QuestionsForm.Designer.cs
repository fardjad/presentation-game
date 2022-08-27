namespace PresentationGame
{
    partial class QuestionsForm
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
            this.btnAddQuestion = new System.Windows.Forms.Button();
            this.btnEditQuestion = new System.Windows.Forms.Button();
            this.btnRemoveQuestion = new System.Windows.Forms.Button();
            this.lstQuestions = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.btnAddQuestion);
            this.flowLayoutPanel1.Controls.Add(this.btnEditQuestion);
            this.flowLayoutPanel1.Controls.Add(this.btnRemoveQuestion);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 325);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(583, 48);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnAddQuestion
            // 
            this.btnAddQuestion.AutoSize = true;
            this.btnAddQuestion.Location = new System.Drawing.Point(3, 3);
            this.btnAddQuestion.Name = "btnAddQuestion";
            this.btnAddQuestion.Size = new System.Drawing.Size(96, 42);
            this.btnAddQuestion.TabIndex = 0;
            this.btnAddQuestion.Text = "Add Keyword...";
            this.btnAddQuestion.UseVisualStyleBackColor = true;
            // 
            // btnEditQuestion
            // 
            this.btnEditQuestion.AutoSize = true;
            this.btnEditQuestion.Location = new System.Drawing.Point(105, 3);
            this.btnEditQuestion.Name = "btnEditQuestion";
            this.btnEditQuestion.Size = new System.Drawing.Size(96, 42);
            this.btnEditQuestion.TabIndex = 2;
            this.btnEditQuestion.Text = "Edit Keyword...";
            this.btnEditQuestion.UseVisualStyleBackColor = true;
            // 
            // btnRemoveQuestion
            // 
            this.btnRemoveQuestion.AutoSize = true;
            this.btnRemoveQuestion.Location = new System.Drawing.Point(207, 3);
            this.btnRemoveQuestion.Name = "btnRemoveQuestion";
            this.btnRemoveQuestion.Size = new System.Drawing.Size(98, 42);
            this.btnRemoveQuestion.TabIndex = 1;
            this.btnRemoveQuestion.Text = "RemoveKeyword";
            this.btnRemoveQuestion.UseVisualStyleBackColor = true;
            // 
            // lstQuestions
            // 
            this.lstQuestions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstQuestions.FormattingEnabled = true;
            this.lstQuestions.Location = new System.Drawing.Point(12, 12);
            this.lstQuestions.Name = "lstQuestions";
            this.lstQuestions.Size = new System.Drawing.Size(583, 303);
            this.lstQuestions.TabIndex = 1;
            // 
            // QuestionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 385);
            this.Controls.Add(this.lstQuestions);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "QuestionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Questions";
            this.Load += new System.EventHandler(this.QuestionsForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ListBox lstQuestions;
        private System.Windows.Forms.Button btnAddQuestion;
        private System.Windows.Forms.Button btnRemoveQuestion;
        private System.Windows.Forms.Button btnEditQuestion;
    }
}