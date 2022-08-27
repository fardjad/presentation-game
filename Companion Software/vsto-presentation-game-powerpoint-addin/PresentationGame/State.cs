using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Newtonsoft.Json;
using Shape = Microsoft.Office.Interop.PowerPoint.Shape;

namespace PresentationGame
{
    public class State
    {
        private static readonly Mutex Mutex = new Mutex();
        private readonly Application _application;

        public readonly IObservable<Slide> ActiveSlideObservable;

        private State()
        {
            _application = Globals.ThisAddIn.Application;

            ActiveSlideObservable = Observable.FromEvent<EApplication_SlideSelectionChangedEventHandler, SlideRange>(
                    handler => _application.SlideSelectionChanged += handler,
                    handler => _application.SlideSelectionChanged -= handler)
                .Select(slideRange => slideRange.Count == 1 ? slideRange[1] : null);
        }

        public IEnumerable<Slide> Slides
        {
            get
            {
                var activePresentation = _application.ActivePresentation;
                return activePresentation == null
                    ? new List<Slide>()
                    : _application.ActivePresentation.Slides.OfType<Slide>().ToList();
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static string GetNotes(Slide slide)
        {
            if (slide.HasNotesPage != MsoTriState.msoTrue) return "";
            var shapes = slide.NotesPage.Shapes;
            foreach (Shape shape in shapes)
                if (shape.Type == MsoShapeType.msoPlaceholder &&
                    shape.PlaceholderFormat.Type == PpPlaceholderType.ppPlaceholderBody)
                    return shape.TextFrame.TextRange.Text;

            return "";
        }

        public static GameSlideMetadata GetMetadata(Slide slide)
        {
            var notes = GetNotes(slide);
            try
            {
                return JsonConvert.DeserializeObject<GameSlideMetadata>(notes);
            }
            catch (JsonException)
            {
                return new GameSlideMetadata
                {
                    IsFirstQuestionSlide = false,
                    Keywords = new Dictionary<string, bool>(),
                    LikertScaleQuestions = new List<LikertScaleQuestion>(),
                    ShouldRaiseHand = false
                };
            }
        }

        public static void SetNotes(Slide slide, string text)
        {
            var shapes = slide.NotesPage.Shapes;
            foreach (Shape shape in shapes)
            {
                if (shape.Type != MsoShapeType.msoPlaceholder ||
                    shape.PlaceholderFormat.Type != PpPlaceholderType.ppPlaceholderBody) continue;

                shape.TextFrame.TextRange.Text = text;
                return;
            }

            var newShape = shapes.AddPlaceholder(PpPlaceholderType.ppPlaceholderBody);
            newShape.TextFrame.TextRange.Text = text;
        }

        public static State GetInstance()
        {
            Mutex.WaitOne();
            var state = new State();
            Mutex.ReleaseMutex();

            return state;
        }
    }
}