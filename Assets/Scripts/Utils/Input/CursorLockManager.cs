using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Zenject;

namespace Utils.Input
{
    public class CursorLockManager : MonoBehaviour
    {
        private InputObservableHelper _inputObservableHelper;
        private ZenjectSceneLoader _loader;
        private NpcManager _npcManager;
        private ScoreManager _scoreManager;

        [Inject]
        [UsedImplicitly]
        public void Construct(UpdateInputObservableHelper inputObservableHelper,
            ZenjectSceneLoader loader,
            NpcManager npcManager,
            ScoreManager scoreManager)
        {
            _scoreManager = scoreManager;
            _npcManager = npcManager;
            _inputObservableHelper = inputObservableHelper;
            _loader = loader;
        }

        private void Start()
        {
            var escapeObservable = _inputObservableHelper.GetKeyDownObservable(KeyCode.Escape);

            // Cursor.lockState = CursorLockMode.Locked;

            escapeObservable
                .Subscribe(_ => LoadScoreScene());
        }

        private void LoadScoreScene()
        {
            Destroy(GameObject.Find("NPCs"));
            Destroy(GameObject.Find("Chairs"));
            _npcManager.Dispose();

            _loader.LoadScene(3);
        }

        private void Update()
        {
            if (_scoreManager.Finished) LoadScoreScene();
        }
    }
}