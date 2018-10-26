using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Controllers
{
    public class LoggerController : MonoBehaviour
    {
        public Camera MainCamera;

        private StreamWriter _tw;
        private ScoreManager _scoreManager;
        private CommunicationManager _communicationManager;
        private IDisposable _currentSlideDisposable;

        private int _totalKeywordsForCurrentSlide = -1;
        private int _mentionedKeywordsForCurrentSlide = -1;

        private float _elapsedTime = 0;

        [Inject]
        [UsedImplicitly]
        private void Construct(ScoreManager scoreManager, CommunicationManager communicationManager)
        {
            _scoreManager = scoreManager;
            _communicationManager = communicationManager;
        }

        private void Start()
        {
            var fileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-zz") + ".csv";
            _tw = new StreamWriter(Application.persistentDataPath + @"/" + fileName);
            _tw.WriteLine(
                "timestamp, tx, ty, tz, fx, fy, fz, vx, vy, vz, slide, average_attention, mentioned_keywords, total_keywords, looking_at");

            _currentSlideDisposable = _communicationManager.GetObservableForType("currentSlideScore")
                .Select(scoreObject =>
                {
                    var scoreDictionary = (Dictionary<string, object>) scoreObject;
                    return new
                    {
                        NumberOfKeywords = scoreDictionary["numberOfKeywords"],
                        NumberOfMentionedKeywords = scoreDictionary["numberOfMentionedKeywords"]
                    };
                })
                .Subscribe(score =>
                {
                    _totalKeywordsForCurrentSlide = int.Parse(score.NumberOfKeywords.ToString());
                    _mentionedKeywordsForCurrentSlide = int.Parse(score.NumberOfMentionedKeywords.ToString());
                });
        }

        private static long GetJsNow()
        {
            return (long) Math.Floor((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        private void Update()
        {
            if (MainCamera == null)
            {
                Debug.Log("Main camera is null");
                return;
            }

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < 0.5f) return;
            _elapsedTime = 0;

            var timestamp = GetJsNow();

            var tx = MainCamera.transform.position.x;
            var ty = MainCamera.transform.position.y;
            var tz = MainCamera.transform.position.z;

            var fx = MainCamera.transform.forward.x;
            var fy = MainCamera.transform.forward.y;
            var fz = MainCamera.transform.forward.z;

            var vx = MainCamera.velocity.x;
            var vy = MainCamera.velocity.y;
            var vz = MainCamera.velocity.z;

            var ray = MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            var lookingAt = "Nothing";
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 500f, int.MaxValue,
                QueryTriggerInteraction.Collide))
            {
                lookingAt = hit.transform.name;
            }

            _tw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, \"{14}\"",
                timestamp,
                tx, ty, tz, fx, fy, fz, vx, vy, vz, _scoreManager.CurrentSlide, _scoreManager.AverageAttention,
                _mentionedKeywordsForCurrentSlide, _totalKeywordsForCurrentSlide, lookingAt.Replace(",", "-"));
        }

        private void OnDestroy()
        {
            _currentSlideDisposable.Dispose();
            _tw.Close();
            _tw.Dispose();
        }
    }
}