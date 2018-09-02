using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        private ZenjectSceneLoader _loader;

        [Inject]
        [UsedImplicitly]
        private void Construct(ZenjectSceneLoader loader)
        {
            _loader = loader;
        }

        public void Start()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public void HandleExit()
        {
            Debug.Log("Exit!");
            Application.Quit();
        }

        public void HandlePlay()
        {
            _loader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
