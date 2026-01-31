using System;
using System.Collections.Generic;
using NaughtyAttributes;
using PrimeTween;
using UnityEngine;

namespace Game.Behaviours
{
    public class ObstacleBehaviour : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        
        public float Movement = 1;

        [ReadOnly, SerializeField] private bool _canMove;

        private void Start()
        {
            _canMove = true;
        }

        public bool TryMove(Vector3 direction)
        {
            // if (!_canMove)
            // {
            //     return false;
            // }
            
            // raycast from the bounds of the collider
            // if there is an obstacle, return false
            // else return true    
            
            var destination = transform.position + direction;

            var results = new List<Collider2D>();
            
            var size = Physics2D.OverlapBox(destination, Vector2.one * Movement, 0, ContactFilter2D.noFilter, results);

            if (size == 0)
            {
                // _canMove = false;
                // Tween.RigidbodyMovePosition(_rigidbody2D, transform.position + direction, 0.2f)
                //     .OnComplete(() => _canMove = true);

                transform.Translate(direction);
            }
            
            return size == 0;
        }

    }
}