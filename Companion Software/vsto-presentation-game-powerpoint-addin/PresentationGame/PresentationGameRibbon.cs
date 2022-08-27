using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.Office.Tools.Ribbon;
using PresentationGame.Properties;
using PresentationGame.ReactiveExtensions;

namespace PresentationGame
{
    [UsedImplicitly]
    public partial class PresentationGameRibbon
    {
        private void PresentationGameRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            var state = State.GetInstance();

            var activeSlideIsNullObservable = state.ActiveSlideObservable.Select(slide => slide == null);

            var controlsStateObservable = state.ActiveSlideObservable.SelectMany(_ => grpSlide.Items).WithLatestFrom(
                activeSlideIsNullObservable,
                (control, isNull) => new
                {
                    Control = control,
                    Enabled = !isNull
                });

            var btnShowKeywordsFormClickObservable = btnShowKeywordsForm.GetClickObservable();
            var btnQuestionsClickObservable = btnShowQuestionsForm.GetClickObservable();

            var keywordsFormObservable = btnShowKeywordsFormClickObservable
                .WithLatestFrom(state.ActiveSlideObservable, (unit, slide) => slide)
                .Select(slide => new KeywordsForm(slide));

            var questionsFormObservable = btnQuestionsClickObservable
                .WithLatestFrom(state.ActiveSlideObservable, (unit, slide) => slide)
                .Select(slide => new QuestionsForm(slide));

            var formsObservable = keywordsFormObservable.OfType<Form>().Merge(questionsFormObservable.OfType<Form>());

            var shouldRaiseHandObservable = chbShouldRaiseHand.GetCheckedObservable().WithLatestFrom(
                state.ActiveSlideObservable,
                (b, slide) => new
                {
                    Slide = slide,
                    ShouldRaiseHand = b
                }).Select(
                o =>
                {
                    var nextMetadata = State.GetMetadata(o.Slide);
                    nextMetadata.ShouldRaiseHand = o.ShouldRaiseHand;
                    return new
                    {
                        o.Slide,
                        Metadata = nextMetadata
                    };
                });

            var slideMetadataObservable = state.ActiveSlideObservable.Where(slide => slide != null)
                .Select(State.GetMetadata);

            var slidesQuestionsStartIndexChangeObservable = btnSetQuestionsStart.GetClickObservable()
                .SelectMany(_ => state.Slides).Select(slide => new
                {
                    Slide = slide,
                    Metadata = State.GetMetadata(slide)
                }).WithLatestFrom(state.ActiveSlideObservable, (o, activeSlide) =>
                {
                    var nextMetadata = o.Metadata;
                    nextMetadata.IsFirstQuestionSlide = o.Slide == activeSlide;
                    return new
                    {
                        o.Slide,
                        Metadata = nextMetadata
                    };
                });


            var exportObservable =
                btnExport.GetClickObservable().Select(slides => state.Slides.Select(State.GetMetadata))
                    .Select(list => new GameModel(list).ToJson());

            // Effects

            controlsStateObservable.Subscribe(o => { o.Control.Enabled = o.Enabled; });
            formsObservable.Subscribe(form => { form.ShowDialog(); });
            slideMetadataObservable.Subscribe(metadata => { chbShouldRaiseHand.Checked = metadata.ShouldRaiseHand; });
            shouldRaiseHandObservable.Subscribe(o => { State.SetNotes(o.Slide, o.Metadata.ToJson()); });
            slidesQuestionsStartIndexChangeObservable.Subscribe(o => { State.SetNotes(o.Slide, o.Metadata.ToJson()); });
            exportObservable.Subscribe(json =>
            {
                Clipboard.SetText(json);
                MessageBox.Show(Resources.CopiedToClipboard);
            });
        }
    }
}