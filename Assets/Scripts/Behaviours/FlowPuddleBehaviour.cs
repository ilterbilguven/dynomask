using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Behaviours
{
    public struct FlowMovement
    {
        public ObstacleBehaviour Obstacle;
        public Vector2 Direction;
    }
    
    public class FlowPuddleBehaviour : MonoBehaviour
    {
        public Queue<FlowMovement> MovementQueue = new();
        private HashSet<ObstacleBehaviour> m_Queued = new();

        private Coroutine m_Coroutine;

        public void Enqueue(ObstacleBehaviour obstacle, Vector2 direction)
        {
            if (!obstacle.CanBeFlowMoved) return;
            if (m_Queued.Contains(obstacle)) return;

            m_Queued.Add(obstacle);
            MovementQueue.Enqueue(new FlowMovement
            {
                Obstacle = obstacle,
                Direction = direction
            });
        }

        private void Update()
        {
            if (m_Coroutine == null && MovementQueue.Count > 0)
            {
                m_Coroutine = StartCoroutine(Process());
            }
        }

        private IEnumerator Process()
        {
            var move = MovementQueue.Dequeue();

            bool moved = move.Obstacle.TryMove(move.Direction, true, 1f);

            if (!moved)
            {
                MovementQueue.Enqueue(move);
            }
            else
            {
                move.Obstacle.LockFlow(1f); 
                m_Queued.Remove(move.Obstacle);
            }

            yield return null;
            m_Coroutine = null;
        }
    }
}