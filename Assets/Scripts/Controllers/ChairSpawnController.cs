using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Controllers
{
    public class ChairSpawnController : MonoBehaviour
    {
        private ChairController.Factory _chairControllerFactory;
        private ChairManager _chairManager;
        private Settings _settings;

        [Inject]
        [UsedImplicitly]
        public void Construct(
            ChairController.Factory factory,
            ChairManager manager,
            Settings settings
        )
        {
            _chairControllerFactory = factory;
            _chairManager = manager;
            _settings = settings;
        }

        private void Start()
        {
            var gameObjectRenderer = GetComponent<Renderer>();

            var countX = _settings.CountX;
            var countZ = _settings.CountZ;

            Observable.Range(0, countX)
                .SelectMany(ix => Observable.Range(0, countZ)
                    .Select(iz => new
                    {
                        Ix = ix,
                        Iz = iz,
                        Position = new Vector3(
                            gameObjectRenderer.bounds.min.x + ix * gameObjectRenderer.bounds.size.x / (countX - 1),
                            0,
                            gameObjectRenderer.bounds.min.z + iz * gameObjectRenderer.bounds.size.z / (countZ - 1)
                        ),
                        Name = string.Format("Chair[{0},{1}]", ix.ToString(), iz.ToString())
                    }))
                .Subscribe(data =>
                {
                    var chairController = _chairControllerFactory.Create();
                    chairController.gameObject.transform.position = data.Position;
                    chairController.gameObject.name = data.Name;
                    _chairManager.RegisterChairController(data.Ix, data.Iz, chairController);
                });
        }

        [Serializable]
        public class RowCol
        {
            [SerializeField] [UsedImplicitly] public int Col;
            [SerializeField] [UsedImplicitly] public int Row;

            public RowCol(int row, int col)
            {
                Row = row;
                Col = col;
            }

            private bool Equals(RowCol other)
            {
                return Col == other.Col && Row == other.Row;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == this.GetType() && Equals((RowCol) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Col * 397) ^ Row;
                }
            }
        }

        [Serializable]
        public class Settings
        {
            [SerializeField] [UsedImplicitly] public int CountX = 3;
            [SerializeField] [UsedImplicitly] public int CountZ = 3;
            [SerializeField] [UsedImplicitly] public List<RowCol> DisabledChairs;
        }
    }
}
