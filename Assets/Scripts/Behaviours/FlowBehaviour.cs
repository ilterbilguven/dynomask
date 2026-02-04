using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Behaviours
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class FlowBehaviour : MonoBehaviour
    {
        [SerializeField] private Vector2 m_FlowDirection = Vector2.right;

        [SerializeField] private float m_MovementSpeed = 1f;

        private bool m_EnqueuedMovement = false;
        
        private ObstacleBehaviour m_Obstacle;

        private FlowPuddleBehaviour m_FlowPuddle;

        private void Start()
        {
            m_FlowPuddle = GetComponentInParent<FlowPuddleBehaviour>();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent(out ObstacleBehaviour obstacle)) return;
            m_FlowPuddle.Enqueue(obstacle, m_FlowDirection);
        }

        private void OnDrawGizmos()
        {
            Vector2 pos = transform.position;
            DrawArrow(pos - m_FlowDirection * 0.25f, m_FlowDirection * 0.5f);
        }
        
        // From here: https://discussions.unity.com/t/debug-drawarrow/442586/9
        public void DrawArrow (Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay (pos, direction);
            DrawArrowEnd(true, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
        }
        
        private void DrawArrowEnd (bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Vector3 right = Quaternion.LookRotation (direction) * Quaternion.Euler (arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 left = Quaternion.LookRotation (direction) * Quaternion.Euler (-arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 up = Quaternion.LookRotation (direction) * Quaternion.Euler (0, arrowHeadAngle, 0) * Vector3.back;
            Vector3 down = Quaternion.LookRotation (direction) * Quaternion.Euler (0, -arrowHeadAngle, 0) * Vector3.back;
            if (gizmos) {
                Gizmos.color = color;
                Gizmos.DrawRay (pos + direction, right * arrowHeadLength);
                Gizmos.DrawRay (pos + direction, left * arrowHeadLength);
                Gizmos.DrawRay (pos + direction, up * arrowHeadLength);
                Gizmos.DrawRay (pos + direction, down * arrowHeadLength);
            } else {
                Debug.DrawRay (pos + direction, right * arrowHeadLength, color);
                Debug.DrawRay (pos + direction, left * arrowHeadLength, color);
                Debug.DrawRay (pos + direction, up * arrowHeadLength, color);
                Debug.DrawRay (pos + direction, down * arrowHeadLength, color);
            }
        }
    }
}
