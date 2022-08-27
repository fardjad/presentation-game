using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using Microsoft.Office.Interop.PowerPoint;
using PresentationGame.ReactiveExtensions;

namespace PresentationGame
{
    public partial class KeywordsForm : Form
    {
        private readonly Slide _activeSlide;
        private readonly GameSlideMetadata _activeSlideMetadata;

        public KeywordsForm(Slide activeSlide)
        {
            InitializeComponent();

            _activeSlide = activeSlide;
            _activeSlideMetadata = State.GetMetadata(_activeSlide);
        }

        private static IObservable<T> FilterWithBooleanObservable<T>(IObservable<T> sourceObservable,
            IObservable<bool> predicateObservable)
        {
            return sourceObservable
                .WithLatestFrom(predicateObservable, (v, b) => new {Value = v, Predicate = b})
                .Where(o => o.Predicate).Select(o => o.Value);
        }

        private void KeywordsForm_Load(object sender, EventArgs e)
        {
            var keywordsList = new BindingList<string>();

            var btnRemoveClickObservable = btnRemoveKeyword.GetClickObservable();
            var btnAddKeywordClickObservable = btnAddKeyword.GetClickObservable();
            var btnEditKeywordClickObservable = btnEditKeyword.GetClickObservable();

            var isItemSelectedObservable = lstKeywords.GetIsItemSelectedObservable();
            var lstKeywordsItemDoubleClickObservable = lstKeywords.GetDoubleClickObservable();

            var formClosedObservable = this.GetClosedObservable();

            var editSelectedItemObservable = FilterWithBooleanObservable(
                btnEditKeywordClickObservable.Merge(lstKeywordsItemDoubleClickObservable), isItemSelectedObservable);

            var removeSelectedItemObservable =
                FilterWithBooleanObservable(btnRemoveClickObservable, isItemSelectedObservable);

            var buttonStatesObservable = Observable.Return(false).Merge(isItemSelectedObservable);

            var editDialogObservable = editSelectedItemObservable
                .WithLatestFrom(lstKeywords.GetSelectedItemObservable(), (pattern, keyword) => keyword)
                .SelectMany(keyword =>
                    new TextInputDialog("Enter the new keyword to replace the current keyword:", keyword as string)
                        .AnswerObservable)
                .WithLatestFrom(lstKeywords.GetSelectedIndexObservable(), (answer, index) => new
                {
                    Answer = answer,
                    Index = index
                });

            var addDialogObservable = btnAddKeywordClickObservable
                .SelectMany(keyword =>
                    new TextInputDialog("Enter the keyword to be added to the list:")
                        .AnswerObservable);

            var removeDialogObservable = removeSelectedItemObservable
                .WithLatestFrom(lstKeywords.GetSelectedIndexObservable(), (pattern, i) => i);

            var slideJsonObservable = keywordsList.GetListObservable().Scan(_activeSlideMetadata, (acc, value) =>
            {
                var nextSlideMetaData = acc;
                nextSlideMetaData.Keywords =
                    value.Aggregate(new Dictionary<string, bool>(), (bools, s) =>
                    {
                        bools[s] = false;
                        return bools;
                    });
                return nextSlideMetaData;
            }).Select(metadata => metadata.ToJson());


            // Effects

            _activeSlideMetadata.Keywords?.Keys.ToList().ForEach(key => keywordsList.Add(key));
            lstKeywords.DataSource = keywordsList;
            lstKeywords.ClearSelected();

            var editAndRemoveButtonsStateDisposable = buttonStatesObservable.Subscribe(enabled =>
            {
                btnEditKeyword.Enabled = enabled;
                btnRemoveKeyword.Enabled = enabled;
            });

            var editSelectedItemDisposable = editDialogObservable.Subscribe(o => { keywordsList[o.Index] = o.Answer; });

            var addSelectedItemDisposable = addDialogObservable.Subscribe(answer =>
            {
                keywordsList.Add(answer);
                lstKeywords.SetSelected(keywordsList.Count - 1, true);
            });

            var removeSelectedItemDisposable = removeDialogObservable.Subscribe(index => keywordsList.RemoveAt(index));

            var slideJsonDisposable =
                slideJsonObservable.Subscribe(json => { State.SetNotes(_activeSlide, json); });


            formClosedObservable.Subscribe(_ =>
            {
                DialogResult = DialogResult.OK;

                editAndRemoveButtonsStateDisposable.Dispose();
                addSelectedItemDisposable.Dispose();
                editSelectedItemDisposable.Dispose();
                removeSelectedItemDisposable.Dispose();
                slideJsonDisposable.Dispose();
            });
        }
    }
}