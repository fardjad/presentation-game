using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Utils
{
    [UsedImplicitly]
    public class ScoreManager
    {
        public readonly IDictionary<int, TimeSpan> SlideTime;
        public double AverageAttention;
        public bool Finished = false;

        public ScoreManager()
        {
            SlideTime = new Dictionary<int, TimeSpan>();
        }
    }
}
