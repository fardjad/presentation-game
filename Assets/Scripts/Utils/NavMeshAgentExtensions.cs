using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Utils
{
    public static class NavMeshAgentExtensions
    {
        public delegate void GameObjectMoveDelegate(Vector3 velocity);

        private const float Tolerance = 0.01f;

        public static void DrawChosenPath(this NavMeshAgent navMeshAgent, Color color)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.gameObject == null) return;
            var points = new List<Vector3> {navMeshAgent.gameObject.transform.position};
            points.AddRange(navMeshAgent.path.corners);
            if (points.Count < 2) return;
            for (var i = 0; i < points.Count - 1; i++)
            {
                Debug.DrawLine(points[i], points[i + 1], color, Time.deltaTime, false);
            }
        }

        public static bool MoveTo(this NavMeshAgent navMeshAgent,
            Vector3 destination,
            GameObjectMoveDelegate gameObjectMoveDelegate)
        {
            navMeshAgent.SetDestination(destination);
            if (navMeshAgent.pathPending)
            {
                gameObjectMoveDelegate.Invoke(Vector3.zero);
                return true;
            }

            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                gameObjectMoveDelegate.Invoke(navMeshAgent.desiredVelocity);
                return true;
            }

            gameObjectMoveDelegate.Invoke(Vector3.zero);
            return navMeshAgent.velocity.magnitude < Tolerance;
        }
    }
}
