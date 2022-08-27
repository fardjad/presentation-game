using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
// ReSharper disable NotAccessedField.Global

namespace PresentationGame
{
    public struct LikertScaleQuestion
    {
        public string Text;
        public int Value;
    }

    public struct GameSlideMetadata
    {
        public IDictionary<string, bool> Keywords;
        public IList<LikertScaleQuestion> LikertScaleQuestions;
        public bool ShouldRaiseHand;
        public bool IsFirstQuestionSlide;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public struct GameSlide
    {
        public int Index;
        public IDictionary<string, bool> Keywords;
        public IList<LikertScaleQuestion> LikertScaleQuestions;
        public string Uri;
        public bool ShouldRaiseHand;
    }

    public struct GameModel
    {
        // ReSharper disable MemberCanBePrivate.Global
        public IDictionary<int, GameSlide> Slides;
        public int ElapsedTime;
        public int SlideNumber;
        public int QuestionsStartIndex;

        public bool IsInQaTime;
        // ReSharper restore MemberCanBePrivate.Global

        public GameModel(IEnumerable<GameSlideMetadata> slides)
        {
            var gameSlideMetadataList = slides.ToList();

            Slides = gameSlideMetadataList.Select((slide, index) => new
            {
                Slide = slide,
                Index = index
            }).Aggregate(new Dictionary<int, GameSlide>(), (dictionary, o) =>
            {
                dictionary[o.Index] = new GameSlide
                {
                    Index = o.Index,
                    ShouldRaiseHand = o.Slide.ShouldRaiseHand,
                    Keywords = o.Slide.Keywords,
                    LikertScaleQuestions = o.Slide.LikertScaleQuestions,
                    Uri = $"/slides/Presentation-{o.Index}.jpeg"
                };
                return dictionary;
            });
            ElapsedTime = 0;
            SlideNumber = 0;
            QuestionsStartIndex = gameSlideMetadataList.Select((slide, index) => new
            {
                slide.IsFirstQuestionSlide,
                Index = index
            }).Where(o => o.IsFirstQuestionSlide).Select(o => o.Index).DefaultIfEmpty(0).First();
            IsInQaTime = false;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}