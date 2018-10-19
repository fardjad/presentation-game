using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Utils.VR;
using Zenject;
using Debug = UnityEngine.Debug;

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
            StartCoroutine(VrUtils.SwitchMode(false));

            Cursor.lockState = CursorLockMode.None;
        }

        public void HandleExit()
        {
            Debug.Log("Exit!");
            StartCoroutine(Quit());
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private IEnumerator Quit()
        {
            yield return new WaitForEndOfFrame();
            Process.GetCurrentProcess().Kill();
        }

        public void HandlePlay()
        {
            _loader.LoadScene(1); // Main
        }

        public void HandlePlayInVr()
        {
            _loader.LoadScene(2); // MainVR
        }
    }
}