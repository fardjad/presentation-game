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
    public partial class QuestionsForm : Form
    {
        private readonly Slide _activeSlide;
        private readonly GameSlideMetadata _activeSlideMetadata;

        public QuestionsForm(Slide activeSlide)
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

        private void QuestionsForm_Load(object sender, EventArgs e)
        {
            var questionsList = new BindingList<string>();

            var btnRemoveClickObservable = btnRemoveQuestion.GetClickObservable();
            var btnAddQuestionClickObservable = btnAddQuestion.GetClickObservable();
            var btnEditQuestionClickObservable = btnEditQuestion.GetClickObservable();

            var isItemSelectedObservable = lstQuestions.GetIsItemSelectedObservable();
            var lstQuestionsItemDoubleClickObservable = lstQuestions.GetDoubleClickObservable();

            var formClosedObservable = this.GetClosedObservable();

            var editSelectedItemObservable = FilterWithBooleanObservable(
                btnEditQuestionClickObservable.Merge(lstQuestionsItemDoubleClickObservable), isItemSelectedObservable);

            var removeSelectedItemObservable =
                FilterWithBooleanObservable(btnRemoveClickObservable, isItemSelectedObservable);

            var buttonStatesObservable = Observable.Return(false).Merge(isItemSelectedObservable);

            var editDialogObservable = editSelectedItemObservable
                .WithLatestFrom(lstQuestions.GetSelectedItemObservable(), (pattern, question) => question)
                .SelectMany(question =>
                    new TextInputDialog("Enter the new question to replace the current question:", question as string)
                        .AnswerObservable)
                .WithLatestFrom(lstQuestions.GetSelectedIndexObservable(), (answer, index) => new
                {
                    Answer = answer,
                    Index = index
                });

            var addDialogObservable = btnAddQuestionClickObservable
                .SelectMany(question =>
                    new TextInputDialog("Enter the question to be added to the list:")
                        .AnswerObservable);

            var removeDialogObservable = removeSelectedItemObservable
                .WithLatestFrom(lstQuestions.GetSelectedIndexObservable(), (pattern, i) => i);

            var slideJsonObservable = questionsList.GetListObservable().Scan(_activeSlideMetadata, (acc, value) =>
            {
                var nextSlideMetaData = acc;
                nextSlideMetaData.LikertScaleQuestions =
                    value.Aggregate(new List<LikertScaleQuestion>(), (list, question) =>
                    {
                        list.Add(new LikertScaleQuestion
                        {
                            Text = question,
                            Value = 0
                        });
                        return list;
                    });
                return nextSlideMetaData;
            }).Select(metadata => metadata.ToJson());


            // Effects

            _activeSlideMetadata.LikertScaleQuestions.Select(q => q.Text).ToList()
                .ForEach(item => questionsList.Add(item));
            lstQuestions.DataSource = questionsList;
            lstQuestions.ClearSelected();

            var editAndRemoveButtonsStateDisposable = buttonStatesObservable.Subscribe(enabled =>
            {
                btnEditQuestion.Enabled = enabled;
                btnRemoveQuestion.Enabled = enabled;
            });

            var editSelectedItemDisposable =
                editDialogObservable.Subscribe(o => { questionsList[o.Index] = o.Answer; });

            var addSelectedItemDisposable = addDialogObservable.Subscribe(answer =>
            {
                questionsList.Add(answer);
                lstQuestions.SetSelected(questionsList.Count - 1, true);
            });

            var removeSelectedItemDisposable = removeDialogObservable.Subscribe(index => questionsList.RemoveAt(index));

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