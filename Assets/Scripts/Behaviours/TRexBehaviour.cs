using System;
using System.Collections;
using Game.Managers;
using UnityEngine;
using UnityEngine.Playables;

namespace Game
{
    public class TRexBehaviour : MonoBehaviour
    {
        private Vector3 m_StartingPosition;

        private bool m_ShouldAppear = false;
        [SerializeField] private float m_AppearingTime = 10f;
        private float m_AppearingTimer;
        
        [SerializeField] private Animator m_Animator;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            DimensionManager.Instance.OnDimensionChange.AddListener(CheckForTimerActivation);
        }

        private void OnDestroy()
        {
            DimensionManager.Instance.OnDimensionChange.RemoveListener(CheckForTimerActivation);
        }

        private void CheckForTimerActivation(Timeline from, Timeline to)
        {
            m_ShouldAppear = to == Timeline.Past;
            m_AppearingTimer = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_ShouldAppear)
            {
                return;
            }

            m_AppearingTimer += Time.deltaTime;
            if (m_AppearingTimer >= m_AppearingTime)
            {
                StartCoroutine(AnimateGameOver());
                m_ShouldAppear = false;
            }
        }

        private IEnumerator AnimateGameOver()
        {
            GameManager.Instance.Player.DisableInput();
            
            m_Animator.Play("TRexAnimation");

            // Shit work but work (don't want to use timeline tbh)
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.Player.GetComponentInChildren<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.75f);
            
            LevelManager.Instance.OnLevelUnloaded.AddListener(OnLevelUnloaded);
            LevelManager.Instance.ReloadLevel();
        }

        private void OnLevelUnloaded(int arg0)
        {
            DimensionObjectBehaviour.ResetContext();
            DimensionManager.Instance.SetDimension(Timeline.Present);
            
            GameManager.Instance.ResetPlayerPositionInRoom();
            
            AudioManager.Instance.PlayDimensionChangeSound(DimensionManager.Instance.CurrentDimension);
            
            LevelManager.Instance.OnLevelUnloaded.RemoveListener(OnLevelUnloaded);
        }
    }
}
