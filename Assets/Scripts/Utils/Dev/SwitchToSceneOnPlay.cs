using UnityEditor;
using UnityEngine;

namespace Utils.Dev
{
    public class SwitchToSceneOnPlay : MonoBehaviour
    {
        private void Start()
        {
#if UNITY_EDITOR
            EditorWindow.FocusWindowIfItsOpen(typeof(SceneView));
#endif
        }
    }
}