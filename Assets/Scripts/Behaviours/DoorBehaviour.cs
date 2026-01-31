using System;
using Game.Managers;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DoorBehaviour : MonoBehaviour
    {
        // -1 will automatically go to next level..
        [SerializeField] private int m_NextRoomIndex = -1;
        private static readonly string PlayerTag = "Player";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(PlayerTag))
            {
                LevelManager.Instance.ChangeLevel(m_NextRoomIndex + 1);
            }
        }
    }
}
