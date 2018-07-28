using UnityEngine;
using Utils.Network;

// Courtesy of: http://wiki.unity3d.com/index.php/Toolbox

namespace Utils.Toolbox
{
    public class Toolbox : Singleton<Toolbox>
    {
        protected Toolbox()
        {
        }

        private void Awake()
        {
            RegisterComponent<ZMQIPCManager>();
        }

        private static void RegisterComponent<T>() where T : Component
        {
            Instance.GetOrAddComponent<T>();
        }
    }
}
