using System.Collections.Generic;
using Controllers;
using JetBrains.Annotations;
using UniRx;
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

        public IEnumerable<Tuple<int, int>> GetChairPositions()
        {
            var ret = new List<Tuple<int, int>>();
            for (var i = 0; i < _chairSpawnControllerSettings.CountX; i += 1)
            for (var j = 0; j < _chairSpawnControllerSettings.CountZ; j += 1)
                if (!_chairSpawnControllerSettings.DisabledChairs.Contains(new ChairSpawnController.RowCol(i, j)))
                    ret.Add(new Tuple<int, int>(i, j));

            return ret;
        }

        public int GetNumberOfRows()
        {
            return _chairSpawnControllerSettings.CountX;
        }


        public int GetNumberOfCols()
        {
            return _chairSpawnControllerSettings.CountZ;
        }

        public int GetChairsCount()
        {
            var c = GetNumberOfCols() * GetNumberOfRows();
            _chairSpawnControllerSettings.DisabledChairs.ForEach(rowCol =>
            {
                var row = rowCol.Row;
                var col = rowCol.Col;

                if (row < GetNumberOfRows() && col < GetNumberOfCols()) c -= 1;
            });
            return c;
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