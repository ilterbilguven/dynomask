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

        [SerializeField] private float m_AppearAfter = 10f;
        [SerializeField] private float m_RoarAfter = 2f;
        
        private Coroutine m_MainCoroutine;
        
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
            bool shouldAppear = to == Timeline.Past;
            if (shouldAppear && m_MainCoroutine == null)
            {
                m_MainCoroutine = StartCoroutine(UpdateTRexAppearance());
            }
            else if (!shouldAppear && m_MainCoroutine != null)
            {
                StopCoroutine(m_MainCoroutine);
                m_MainCoroutine = null;
            }
        }

        private IEnumerator UpdateTRexAppearance()
        {
            yield return new WaitForSeconds(m_RoarAfter);
            AudioManager.Instance.PlayDinoRoarSound();
            yield return new WaitForSeconds(m_AppearAfter - m_RoarAfter);
            
            m_MainCoroutine = null;
            
            StartCoroutine(AnimateGameOver());
        }

        private IEnumerator AnimateGameOver()
        {
            GameManager.Instance.Player.DisableInput();
            
            m_Animator.Play("TRexAnimation");

            // Shit work, but works (don't want to use timeline tbh)
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.Player.GetComponentInChildren<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.75f);
            
            AudioManager.Instance.PlayDeathSound();
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
