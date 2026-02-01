using System;
using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using Game.Utilities;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Game.Managers
{
    public class LevelManager : SingletonBehaviour<LevelManager>
    {
        private int m_CurrentSceneIndex = 0;
        public int CurrentSceneIndex => m_CurrentSceneIndex;

        [SerializeField] private AnimationCurve m_SceneChangeCurve;
        [SerializeField] private AnimationCurve m_SceneEnterCurve;
        [SerializeField] private float m_EvaluationSpeed = 1;
        [SerializeField] private float m_BlackoutTime = 1;
        [SerializeField] private FullScreenPassRendererFeature m_SceneChangeBlit;

        private HashSet<int> m_AdditiveScenes = new HashSet<int>();

        private Coroutine m_SceneChangeCoroutine;

        /// <summary>
        /// Called b4 level scene starts to change. No unloads/loads are done yet here
        /// </summary>
        public UnityEvent OnLevelChangeRequested { get; private set; }

        /// <summary>
        /// Called after level scene was unloaded. Param is the unloaded level scene build index
        /// </summary>
        public UnityEvent<int> OnLevelUnloaded { get; private set; }

        /// <summary>
        /// Called after level scene was loaded. Param is the loaded level scene build index
        /// </summary>
        public UnityEvent<int> OnLevelLoaded { get; private set; }

        /// <summary>
        /// Called after a level scene change animation ends
        /// </summary>
        public UnityEvent<int> OnLevelChanged { get; private set; }

        [field: ReadOnly, SerializeField] 
        public LevelBehaviour CurrentLevel { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            OnLevelChangeRequested = new UnityEvent();
            OnLevelUnloaded = new UnityEvent<int>();
            OnLevelLoaded = new UnityEvent<int>();
            OnLevelChanged = new UnityEvent<int>();
        }

        public void ChangeLevel(int sceneIndex = -1)
        {
            if (m_SceneChangeCoroutine != null)
            {
                Debug.LogWarning("Previous scene loading didn't finish. Too fast dude.. too fast");
                return;
            }

            m_SceneChangeCoroutine = StartCoroutine(RequestSceneChange(sceneIndex));
        }

        public void ReloadLevel()
        {
            if (m_CurrentSceneIndex == 0)
                return;

            ChangeLevel(m_CurrentSceneIndex);
        }

        IEnumerator RequestSceneChange(int index = -1)
        {
            HapticsManager.Instance.Rumble(0.18f, 0.22f, 0.2f);
            int validIndex = index == -1 ? m_CurrentSceneIndex + 1 : index;
            bool valid = validIndex < SceneManager.sceneCountInBuildSettings;
            if (!valid)
            {
                Debug.Log("No more scenes in build settings");
                yield break;
            }

            m_SceneChangeBlit.SetActive(true);

            OnLevelChangeRequested?.Invoke();

            yield return AnimateSceneChange(m_SceneChangeCurve);
            yield return UnloadSceneAsync(m_CurrentSceneIndex);
            OnLevelUnloaded?.Invoke(m_CurrentSceneIndex);
            yield return LoadSceneAsync(validIndex);
            OnLevelLoaded?.Invoke(validIndex);
            yield return new WaitForSeconds(m_BlackoutTime);
            yield return AnimateSceneChange(m_SceneEnterCurve, 0);

            m_SceneChangeBlit.SetActive(false);

            OnLevelChanged?.Invoke(index);

            m_SceneChangeCoroutine = null;
        }

        IEnumerator AnimateSceneChange(AnimationCurve curve, int clampValue = 1)
        {
            Material pMat = m_SceneChangeBlit.passMaterial;

            if (GameManager.Instance.Player != null)
            {
                Vector2 sspp = Camera.main.WorldToScreenPoint(GameManager.Instance.Player.transform.position);
                sspp = new Vector2(sspp.x / Screen.width, sspp.y / Screen.height);
                pMat.SetVector("_PlayerPosition", sspp);
            }

            float delta = 0;
            while (delta < 1)
            {
                delta += Time.deltaTime * m_EvaluationSpeed;
                float deltaAnim = curve.Evaluate(delta);
                pMat.SetFloat("_SceneOut", deltaAnim);
                yield return null;
            }

            pMat.SetFloat("_SceneOut", clampValue);
        }

        IEnumerator LoadSceneAsync(int index = -1)
        {
            m_CurrentSceneIndex = index;

            var asyncLoad = SceneManager.LoadSceneAsync(m_CurrentSceneIndex, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;
            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }

            m_AdditiveScenes.Add(index);
        }

        IEnumerator UnloadSceneAsync(int index)
        {
            if (!m_AdditiveScenes.Contains(index))
            {
                Debug.Log(index + " was not in hash set");
                yield break;
            }

            var asyncUnload = SceneManager.UnloadSceneAsync(index);
            while (!asyncUnload.isDone)
                yield return null;

            Debug.Log("Unloaded: " + index);
            m_AdditiveScenes.Remove(index);
        }

        public void LocateLevel(LevelBehaviour level)
        {
            CurrentLevel = level;
        }
    }
}