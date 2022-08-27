namespace PresentationGame
{
    partial class PresentationGameRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {

        public PresentationGameRibbon() : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PresentationGameRibbon));
            this.tabAddins = this.Factory.CreateRibbonTab();
            this.tabPresentationGame = this.Factory.CreateRibbonTab();
            this.grpSlide = this.Factory.CreateRibbonGroup();
            this.btnShowKeywordsForm = this.Factory.CreateRibbonButton();
            this.btnShowQuestionsForm = this.Factory.CreateRibbonButton();
            this.btnSetQuestionsStart = this.Factory.CreateRibbonButton();
            this.chbShouldRaiseHand = this.Factory.CreateRibbonCheckBox();
            this.grpExport = this.Factory.CreateRibbonGroup();
            this.btnExport = this.Factory.CreateRibbonButton();
            this.tabAddins.SuspendLayout();
            this.tabPresentationGame.SuspendLayout();
            this.grpSlide.SuspendLayout();
            this.grpExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabAddins
            // 
            this.tabAddins.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabAddins.Label = "TabAddIns";
            this.tabAddins.Name = "tabAddins";
            // 
            // tabPresentationGame
            // 
            this.tabPresentationGame.Groups.Add(this.grpSlide);
            this.tabPresentationGame.Groups.Add(this.grpExport);
            this.tabPresentationGame.Label = "Presentation Game";
            this.tabPresentationGame.Name = "tabPresentationGame";
            // 
            // grpSlide
            // 
            this.grpSlide.Items.Add(this.btnShowKeywordsForm);
            this.grpSlide.Items.Add(this.btnShowQuestionsForm);
            this.grpSlide.Items.Add(this.btnSetQuestionsStart);
            this.grpSlide.Items.Add(this.chbShouldRaiseHand);
            this.grpSlide.Label = "Slide";
            this.grpSlide.Name = "grpSlide";
            // 
            // btnShowKeywordsForm
            // 
            this.btnShowKeywordsForm.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnShowKeywordsForm.Image = ((System.Drawing.Image)(resources.GetObject("btnShowKeywordsForm.Image")));
            this.btnShowKeywordsForm.Label = "Keywords";
            this.btnShowKeywordsForm.Name = "btnShowKeywordsForm";
            this.btnShowKeywordsForm.ShowImage = true;
            this.btnShowKeywordsForm.SuperTip = "Set keywords for this slide";
            // 
            // btnShowQuestionsForm
            // 
            this.btnShowQuestionsForm.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnShowQuestionsForm.Image = ((System.Drawing.Image)(resources.GetObject("btnShowQuestionsForm.Image")));
            this.btnShowQuestionsForm.Label = "Questions";
            this.btnShowQuestionsForm.Name = "btnShowQuestionsForm";
            this.btnShowQuestionsForm.ShowImage = true;
            this.btnShowQuestionsForm.SuperTip = "Set questions for this slide";
            // 
            // btnSetQuestionsStart
            // 
            this.btnSetQuestionsStart.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnSetQuestionsStart.Image = ((System.Drawing.Image)(resources.GetObject("btnSetQuestionsStart.Image")));
            this.btnSetQuestionsStart.Label = "Set as First QA Slide";
            this.btnSetQuestionsStart.Name = "btnSetQuestionsStart";
            this.btnSetQuestionsStart.ShowImage = true;
            this.btnSetQuestionsStart.SuperTip = "Set this slide as the first QA slide";
            // 
            // chbShouldRaiseHand
            // 
            this.chbShouldRaiseHand.Label = "Raise Hand";
            this.chbShouldRaiseHand.Name = "chbShouldRaiseHand";
            this.chbShouldRaiseHand.SuperTip = "Should an NPC raise its hand when this slide is shown?";
            // 
            // grpExport
            // 
            this.grpExport.Items.Add(this.btnExport);
            this.grpExport.Label = "Export";
            this.grpExport.Name = "grpExport";
            // 
            // btnExport
            // 
            this.btnExport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnExport.Image = global::PresentationGame.Properties.Resources.Export;
            this.btnExport.Label = "Export";
            this.btnExport.Name = "btnExport";
            this.btnExport.ShowImage = true;
            this.btnExport.SuperTip = "Copies the JSON model to the clipboard";
            // 
            // PresentationGameRibbon
            // 
            this.Name = "PresentationGameRibbon";
            this.RibbonType = "Microsoft.PowerPoint.Presentation";
            this.Tabs.Add(this.tabAddins);
            this.Tabs.Add(this.tabPresentationGame);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.PresentationGameRibbon_Load);
            this.tabAddins.ResumeLayout(false);
            this.tabAddins.PerformLayout();
            this.tabPresentationGame.ResumeLayout(false);
            this.tabPresentationGame.PerformLayout();
            this.grpSlide.ResumeLayout(false);
            this.grpSlide.PerformLayout();
            this.grpExport.ResumeLayout(false);
            this.grpExport.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabAddins;
        private Microsoft.Office.Tools.Ribbon.RibbonTab tabPresentationGame;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpSlide;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnShowQuestionsForm;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSetQuestionsStart;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox chbShouldRaiseHand;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnShowKeywordsForm;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpExport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnExport;
    }

    partial class ThisRibbonCollection
    {
        internal PresentationGameRibbon PresentationGameRibbon
        {
            get { return this.GetRibbon<PresentationGameRibbon>(); }
        }
    }
}
