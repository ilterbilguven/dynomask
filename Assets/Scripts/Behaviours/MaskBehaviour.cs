using System;
using System.Collections;
using Game.Behaviours;
using Game.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class MaskBehaviour : MonoBehaviour
    {
        public Timeline Timeline;

        public static UnityEvent OnMaskWasAlreadyCollected = new UnityEvent();

        public ParticleSystem PickableParicle;
        public GameObject MaskSpriteContainer;
        public AnimationCurve SpriteMaskCurve;
        
        private void Start()
        {
            MaskSpriteContainer.SetActive(false);
            if (PlayerBehaviour.AvailableTimelines.HasFlag(Timeline))
            {
                OnMaskWasAlreadyCollected?.Invoke();
                Destroy(gameObject);
            }
        }

        public void CollectMask(float animDuration = 0.5f, float holdDuration = 1f)
        {
            MaskSpriteContainer.SetActive(true);
            PickableParicle.Stop();
            StartCoroutine(AnimateMaskPickup(animDuration, holdDuration));
        }

        private IEnumerator AnimateMaskPickup(float animDuration, float holdDuration)
        {
            float time = 0;
            while (time < animDuration)
            {
                time += Time.deltaTime;
                float delta = time / animDuration;
                float curve = SpriteMaskCurve.Evaluate(delta);
                MaskSpriteContainer.transform.localScale = new Vector3(curve, curve, 1);
                yield return null;
            }
            MaskSpriteContainer.transform.localScale = Vector3.one;
            
            yield return new WaitForSeconds(holdDuration);
            
            time = 0;
            while (time < animDuration)
            {
                time += Time.deltaTime;
                float delta = time / animDuration;
                float curve = SpriteMaskCurve.Evaluate(1-delta);
                MaskSpriteContainer.transform.localScale = new Vector3(curve, curve, 1);
                yield return null;
            }
            MaskSpriteContainer.transform.localScale = Vector3.zero;
            MaskSpriteContainer.SetActive(false);
        }
    }
}
