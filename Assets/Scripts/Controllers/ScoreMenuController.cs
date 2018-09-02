using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Controllers
{
    public class ScoreMenuController : MonoBehaviour
    {
        private ZenjectSceneLoader _loader;
        private ScoreManager _scoreManager;

        [Inject]
        [UsedImplicitly]
        private void Construct(ZenjectSceneLoader loader, ScoreManager scoreManager)
        {
            _scoreManager = scoreManager;
            _loader = loader;
        }

        // Use this for initialization
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;

            var attentionText = GameObject.Find("AttentionText").GetComponent<Text>();
            var slidesText = GameObject.Find("SlidesText").GetComponent<Text>();

            attentionText.text = string.Format("Average Attention: {0:P}", _scoreManager.AverageAttention);
            var slidesStatsText = "Slides Stats:\n";

            var i = 0;

            _scoreManager.SlideTime.ToList()
                .ForEach(kv =>
                {
                    i += 1;
                    slidesStatsText +=
                        string.Format("{0:00}: {1:000}s\t", kv.Key, kv.Value.TotalSeconds);
                    if (i % 6 == 0) slidesStatsText += "\n";
                });
            slidesText.text = slidesStatsText;
        }

        public void HandleMainMenuButtonClick()
        {
            _loader.LoadScene(0, LoadSceneMode.Single);
        }
    }
}
