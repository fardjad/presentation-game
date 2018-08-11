using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class ChairController : MonoBehaviour
    {
        [UsedImplicitly]
        public class Factory : PlaceholderFactory<ChairController>
        {
        }
    }
}
