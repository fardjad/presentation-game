using Controllers;
using JetBrains.Annotations;
using UnityEngine;

namespace Utils
{
    [UsedImplicitly]
    public class ChairManager
    {
        private readonly ChairController[,] _chairControllers;
        private readonly ChairSpawnController.Settings _chairSpawnControllerSettings;

        public ChairManager(ChairSpawnController.Settings settings)
        {
            _chairSpawnControllerSettings = settings;
            _chairControllers = new ChairController[settings.CountX, settings.CountZ];
        }

        public void RegisterChairController(int row, int col, ChairController chairController)
        {
            _chairControllers[row, col] = chairController;

            foreach (var rowCol in _chairSpawnControllerSettings.DisabledChairs)
                if (row == rowCol.Row && col == rowCol.Col)
                    chairController.gameObject.SetActive(false);
        }

        public bool IsOccupied(int row, int col)
        {
            return false;
        }


        public Vector3 GetChairWalkToPosition(int row, int col)
        {
            var chairController = _chairControllers[row, col];
            var renderer = chairController.gameObject.GetComponentInChildren<Renderer>();
            var walkToPosition = VectorBuilder.FromVector(renderer.bounds.max)
                .SetY(renderer.bounds.min.y)
                .SetX(renderer.bounds.center.x)
                .ToVector();
            return walkToPosition;
        }

        public Vector3 GetChairCenter(int row, int col)
        {
            var chairController = _chairControllers[row, col];
            var renderer = chairController.gameObject.GetComponentInChildren<Renderer>();
            return renderer.bounds.center;
        }
    }
}