using TG.Core;
using UnityEngine;
using DG.Tweening;

namespace TG.GameJamTemplate
{
    /// <summary>
    /// A simple scene transition with fade-in and out
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeSceneTransition : SceneTransition
    {
        [SerializeField] float _fadeDuration = .3f;

        [Header("Optional")]
        [Tooltip("Cosmetic object to activate after fade in completes.")]
        [SerializeField] CanvasGroupFadeComponent _activateAfterFadeIn = default;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            ScenesManager.OnTransitionFadedIn += OnFadedIn;
            ScenesManager.OnTransitionIsGoingToFadeOut += BeforeFadeOut;
        }

        private void OnDisable()
        {
            ScenesManager.OnTransitionFadedIn -= OnFadedIn;
            ScenesManager.OnTransitionIsGoingToFadeOut -= BeforeFadeOut;
        }

        private void Start()
        {
        }

        public override void FadeIn()
        {
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            Fade(1, _fadeDuration);
        }

        public override void FadeOut()
        {
            Fade(0, _fadeDuration);
        }

        private void OnFadedIn()
        {
            if (_activateAfterFadeIn != null)
            {
                _activateAfterFadeIn.gameObject.SetActive(true);
                _activateAfterFadeIn.FadeIn(BeforeFadeStallDuration / 2f);
            }
        }

        private void BeforeFadeOut()
        {
            if (_activateAfterFadeIn != null)
            {
                _activateAfterFadeIn.FadeOut(BeforeFadeStallDuration);
            }
        }

        public void Fade(float targetAlpha, float duration)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.DOFade(targetAlpha, duration).SetEase(Ease.Linear).SetUpdate(true).OnComplete(OnCompleteCallback);
        }

        private void OnCompleteCallback()
        {
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}