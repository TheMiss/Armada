using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Armageddon.Games;
using Cysharp.Threading.Tasks;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armageddon.UI.Base
{
    public enum WidgetAnimationState
    {
        Closed,
        Open
    }

    [SuppressMessage("ReSharper", "Unity.PreferAddressByIdToGraphicsParams")]
    public class Widget : Context
    {
#if DEBUG
        public const string NormalId = "Normal";
        public const string OpenId = "Open";
        public const string ClosedId = "Closed";
#else
        public static readonly int NormalId = Animator.StringToHash("Normal");
        public static readonly int OpenId = Animator.StringToHash("Open");
        public static readonly int ClosedId = Animator.StringToHash("Closed");
#endif

        [SerializeField]
        private bool m_playAnimationOnEnable;

        private Animator m_animator;
        private bool m_eventsAreRegistered;
        private bool m_isDoingAsync;
        private RectTransform m_rectTransform;

        private static UISystem m_ui;

        protected static UISystem UI
        {
            get
            {
                if (m_ui == null)
                {
                    m_ui = GetService<UISystem>();
                }

                return m_ui;
            }
        }

        private static Game m_game;

        protected static Game Game
        {
            get
            {
                if (m_game == null)
                {
                    m_game = GetService<Game>();
                }

                return m_game;
            }
        }
        
        private static EventSystem m_eventSystem;
        
        protected static EventSystem EventSystem
        {
            get
            {
                if (m_eventSystem == null)
                {
                    m_eventSystem = FindObjectOfType<EventSystem>();
                }

                return m_eventSystem;
            }
        }

        public static void SetRaycastTargetsInChildren(GameObject gameObject, bool enabled)
        {
            Image[] graphics = gameObject.GetComponentsInChildren<Image>(true);

            foreach (Image graphic in graphics)
            {
                graphic.raycastTarget = enabled;
                Debug.Log($"{graphic.name}.raycastTarget = {graphic.raycastTarget = false}");
            }

            // Not sure if this is better than only checking on Image
            // Graphic[] graphics = GetComponentsInChildren<Graphic>();
            //
            // foreach (Graphic graphic in graphics)
            // {
            //     graphic.raycastTarget = false;
            //     Debug.Log($"{graphic.name}.{graphic.raycastTarget = false}");
            // }
        }

        /// <summary>
        ///     It seems that is the only to force update layout group.
        ///     https://answers.unity.com/questions/1276433/get-layoutgroup-and-contentsizefitter-to-update-th.html
        ///     As we have to wait for one frame, we are going to see it blinks.
        /// </summary>
        public static IEnumerator RefreshLayout(GameObject layout)
        {
            yield return new WaitForEndOfFrame();
            var layoutGroup = layout.GetComponent<LayoutGroup>();
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }

        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null)
                {
                    m_rectTransform = GetComponent<RectTransform>();
                }

                return m_rectTransform;
            }
        }

        public UnityAction DoTweenAnimation { set; get; }

        protected Animator Animator
        {
            get
            {
                if (ReferenceEquals(m_animator, null))
                {
                    m_animator = GetComponent<Animator>();
                }

                // if (m_animator == null)
                // {
                //     m_animator = GetComponent<Animator>();
                // }

                return m_animator;
            }
        }

        /// <summary>
        ///     Can be used to show/hide control and still maintains its status, an obvious example would be using this with
        ///     UiExpandable control, like, when it's fully expanded and then want to hide without collapsing it.
        /// </summary>
        public bool IsActive
        {
            set => gameObject.SetActive(value);
            get => gameObject.activeInHierarchy;
        }

        public WidgetAnimationState AnimationState { protected set; get; }
        
        private UnityAction m_openAnimationFinished;
        private UnityAction m_closeAnimationFinished;

        protected override void Awake()
        {
            base.Awake();

            DisableBehaviourOnGamePaused = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_playAnimationOnEnable)
            {
                Show();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public virtual void Show(bool animate = true)
        {
            // In case Widget.Show() is called in Editor mode
            if (!Application.isPlaying)
            {
                gameObject.SetActive(true);
                return;
            }

            gameObject.SetActive(true);

            if (animate)
            {
                if (Animator != null)
                {
                    //if (AnimationState == WidgetAnimationState.Closed)
                    {
                        Animator.ResetTrigger(ClosedId);
                        Animator.SetTrigger(OpenId);
                        // Animator.Play(OpenId, -1, 0.0f);
                    }

                    StartCoroutine(NotifyAnimationFinishDelayed(Animator));
                }
                else if (DoTweenAnimation != null)
                {
                    DoTweenAnimation.Invoke();
                }
                else
                {
                    OnOpenAnimationFinished();
                }
            }
            else
            {
                OnOpenAnimationFinished();
            }
        }

        public async UniTask<bool> ShowAsync(UnityAction<Widget> onAnimationFinished = null)
        {
            m_isDoingAsync = true;

            Show();

            CancellationToken token = GetCancellationToken(nameof(ShowAsync));

            while (m_isDoingAsync)
            {
                await UniTask.Yield(token);
            }

            onAnimationFinished?.Invoke(this);

            return true;
        }

#if UNITY_EDITOR
        /// <summary>
        ///     Call this to set TextMesh's name to match its content which is very useful for debugging.
        /// </summary>
        protected void _SetTextMeshObjectNames()
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI text in texts)
            {
                const string symbol = "::";
                if (text.gameObject.name.Contains(symbol))
                {
                    continue;
                }
                
                text.Set(text.text);
            }
        }
#endif

        private void ResetToOpenState()
        {
            // Why this causes an error!
            // if (ReferenceEquals(Animator, null))
            // {
            //     return;
            // }
            //
            // Animator.ResetTrigger(ClosedId);
            // Animator.ResetTrigger(OpenId);
            // Animator.SetTrigger(NormalId);

            if (Animator != null)
            {
                Animator.ResetTrigger(ClosedId);
                Animator.Play(OpenId, 0, 1.0f);
            }
        }

        private void ResetToCloseState()
        {
            if (Animator != null)
            {
                Animator.ResetTrigger(OpenId);
                Animator.Play(ClosedId, 0, 1.0f);
            }
        }

        private IEnumerator NotifyAnimationFinishDelayed(Animator animator)
        {
            bool openStateReached = false;

            float timeout = 5.0f;

            while (!openStateReached)
            {
                if (!animator.IsInTransition(0))
                {
                    AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

#if DEBUG
                    int openId = Animator.StringToHash(OpenId);
#else
                    int openId = OpenId;
#endif

                    if (animatorStateInfo.shortNameHash == openId &&
                        animatorStateInfo.normalizedTime >= 1)
                    {
                        openStateReached = true;
                    }
                }

                timeout -= Time.deltaTime;

                if (timeout <= 0f)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            OnOpenAnimationFinished();
        }

        /// <summary>
        ///     Hide() still lives in memory, in the future, we might have Close() which will not live in memory
        /// </summary>
        public virtual void Hide(bool animate = true)
        {
            // In case Widget.Hide() is called in Editor mode
            if (!Application.isPlaying)
            {
                gameObject.SetActive(false);
                return;
            }

            if (animate)
            {
                if (Animator != null)
                {
                    // To prevent "Coroutine couldn't be started because the the game object 'XxxObject' is inactive!"
                    if (IsActive)
                    {
                        switch (AnimationState)
                        {
                            case WidgetAnimationState.Closed:
                                OnCloseAnimationFinished();
                                break;
                            case WidgetAnimationState.Open:
                                Animator.ResetTrigger(OpenId);
                                Animator.SetTrigger(ClosedId);

                                StartCoroutine(DisableDelayed(Animator));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    // else
                    // {
                    //     OnCloseAnimationFinished();
                    // }
                }
                else if (DoTweenAnimation != null)
                {
                    DoTweenAnimation.Invoke();
                }
                else
                {
                    OnCloseAnimationFinished();
                }
            }
            else
            {
                OnCloseAnimationFinished();
            }
        }

        public void HideAndNotify(UnityAction callback)
        {
            m_closeAnimationFinished = callback;
            Hide();
        }

        // Update there might not be a bug anymore. Need some tests.
        // There's a bug for this method...
        public async UniTask<bool> HideAsync(UnityAction<Widget> onAnimationFinished = null)
        {
            m_isDoingAsync = true;

            Hide();

            CancellationToken token = GetCancellationToken(nameof(HideAsync));

            while (m_isDoingAsync)
            {
                await UniTask.Yield(token);
            }

            onAnimationFinished?.Invoke(this);

            return true;
        }

        private IEnumerator DisableDelayed(Animator animator)
        {
            bool closedStateReached = false;

            float timeout = 5.0f;

            while (!closedStateReached)
            {
                if (!animator.IsInTransition(0))
                {
                    AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
#if DEBUG
                    int closedId = Animator.StringToHash(ClosedId);
#else
                    int closedId = ClosedId;
#endif
                    if (animatorStateInfo.shortNameHash == closedId &&
                        animatorStateInfo.normalizedTime >= 1)
                    {
                        closedStateReached = true;
                    }
                }

                timeout -= Time.deltaTime;

                if (timeout <= 0f)
                {
                    break;
                }
                //wantToClose = !animator.GetBool(OpenStateId);

                yield return null;
            }

            OnCloseAnimationFinished();
        }

        public virtual void ToggleState()
        {
            if (gameObject.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        protected virtual void OnOpenAnimationFinished()
        {
            // Debug.Log($"{name}.OnOpenAnimationFinished");
            ResetToOpenState();

            AnimationState = WidgetAnimationState.Open;
            m_isDoingAsync = false;

            m_openAnimationFinished?.Invoke();
        }

        protected virtual void OnCloseAnimationFinished()
        {
            // Debug.Log($"{name}.OnCloseAnimationFinished");
            // ResetToCloseState();

            AnimationState = WidgetAnimationState.Closed;
            m_isDoingAsync = false;
            gameObject.SetActive(false);

            m_closeAnimationFinished?.Invoke();
        }

        public T TestHit<T>(GraphicRaycaster raycaster, PointerEventData eventData, bool includeParent = true)
            where T : MonoBehaviour
        {
            var raycastResults = new List<RaycastResult>();
            raycaster.Raycast(eventData, raycastResults);

            foreach (RaycastResult raycastResult in raycastResults)
            {
                T hit;

                if (includeParent)
                {
                    hit = raycastResult.gameObject.transform.parent.GetComponent<T>();

                    if (hit != null)
                    {
                        return hit;
                    }
                }

                hit = raycastResult.gameObject.GetComponent<T>();

                if (hit != null)
                {
                    return hit;
                }
            }

            return default;
        }

        /// <summary>
        ///     Get a hit object from pointer position
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="includeParent">
        ///     Sometimes, if not always, we define another hit box object under an object. In that case
        ///     this should be true
        /// </param>
        /// <returns></returns>
        public T TestHit<T>(PointerEventData eventData, bool includeParent = true) where T : MonoBehaviour
        {
            var raycaster = GetComponentInParent<GraphicRaycaster>();
            return TestHit<T>(raycaster, eventData, includeParent);
        }

        public static void SetBlockerSettings(Transform parent, Canvas topmostCanvas)
        {
            UI.BlockerManager.Parent = parent;
            UI.BlockerManager.TopmostCanvas = topmostCanvas;
        }

        /// <summary>
        ///     Equals to UI.BlockerManager.AddBlock()
        /// </summary>
        protected Blocker AddBlocker(Transform parent, Color? blockerColor = null, UnityAction onClickCall = null)
        {
            return UI.BlockerManager.AddBlocker(parent, Transform.GetSiblingIndex(), blockerColor, onClickCall);
        }

        /// <summary>
        ///     Equals to UI.BlockerManager.AddBlock()
        /// </summary>
        protected Blocker AddBlocker(Transform parent, int siblingIndex, Color? blockerColor = null,
            UnityAction onClickCall = null)
        {
            return UI.BlockerManager.AddBlocker(parent, siblingIndex, blockerColor, onClickCall);
        }

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
        protected void RemoveBlocker()
        {
            UI.BlockerManager.RemoveBlocker();
        }

        public virtual void OnResourcesUnloading()
        {
        }

        // public void SetBadge(bool isNoticed, BadgeType badgeType = BadgeType.Dot, bool attachToButton = true)
        // {
        //     if (attachToButton)
        //     {
        //         var button = Transform.GetComponentInChildren<Button>();
        //         if (button == null)
        //         {
        //             Debug.LogWarning($"Cannot find {nameof(Button)} in {name}'s children.");
        //             return;
        //         }
        //
        //         var badge = button.GetComponentInChildren<Badge>();
        //
        //         if (badge == null)
        //         {
        //             badge = Instantiate(UI.PrefabBank.Badge, button.transform);
        //         }
        //
        //         badge.RectTransform.SetAnchor(AnchorPreset.TopLeft, 25, -25);
        //
        //         if (isNoticed)
        //         {
        //             badge.Consume();
        //         }
        //         else
        //         {
        //             badge.SetDot();
        //         }
        //     }
        // }
        
        public void SetBadge(bool isNoticed, BadgeType badgeType = BadgeType.Dot)
        {
            var badge = GetComponentInChildren<Badge>(true);
            if (badge == null)
            {
                Debug.LogWarning($"Cannot find {nameof(Badge)} in {name}'s children.");
                return;
            }
        
            if (isNoticed)
            {
                badge.Consume();
            }
            else
            {
                badge.SetDot();
            }
        }
    }
}
