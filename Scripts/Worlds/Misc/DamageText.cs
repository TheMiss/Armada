using System.Collections;
using Armageddon.Worlds.Actors;
using TMPro;
using UnityEngine;

namespace Armageddon.Worlds.Misc
{
    public enum DamageTextType
    {
        Normal,
        Critical
    }

    public class DamageText : WorldContext
    {
        public static readonly float FadeToAlphaZeroTime = 0.5f;

        public static readonly float MaxScale = 1.5f;
        public static readonly float MinScale = 1.2f;

        public static readonly float ScaleToMaxScaleTime = 0.15f;

        public static readonly float MinOffsetY = 0f;
        public static readonly float MaxOffsetY = 1.0f;

        public static readonly float MoveUpMinSpeed = 0.4f;
        public static readonly float MoveUpMaxSpeed = 1.6f;

        public static readonly float MoveHorizontalMinSpeed = 1.3f;
        public static readonly float MoveHorizontalMaxSpeed = 3.0f;


        public static readonly float RandomMinY = -0.3f;

        public static readonly float RandomMaxY = 0.3f;

        [SerializeField]
        private TextMeshPro m_text;

        private Actor m_actor;

        private Vector2 m_moveDirection;
        private Vector2 m_offsetFromOwner;

        private Transform Target { get; set; }

        public void Play(Transform target)
        {
            Play(target, Vector2.zero);
        }

        public void Play(Actor actor, float damage)
        {
            // float x = Random.Range(-actor.Bounds.extents.x, actor.Bounds.extents.x);
            // float y = actor.Bounds.size.y + Random.Range(RandomMinY, RandomMaxY);
            // var offset = new Vector2(x, y + 0.6f); // Plus offset for health bar

            float x = Random.Range(-0.5f, 0.5f);
            float y = Random.Range(MinOffsetY, MaxOffsetY) + actor.Bounds.size.y + 0.5f; // Plus offset for health bar
            var offset = new Vector2(x, y);

            m_actor = actor;

            m_moveDirection.x = m_actor.DamageTextDirectionX *
                Random.Range(MoveHorizontalMinSpeed, MoveHorizontalMaxSpeed);
            m_moveDirection.y = Random.Range(MoveUpMinSpeed, MoveUpMaxSpeed);
            m_actor.DamageTextDirectionX = -m_actor.DamageTextDirectionX;

            m_text.text = $"{(int)damage}";
            Play(actor.Transform, offset);
        }

        public void Play(Transform target, Vector2 offset)
        {
            Target = target;
            Vector2 ownerPosition = Target.position;

            m_offsetFromOwner = offset;

            Transform.position = ownerPosition + m_offsetFromOwner;

            CanTick = true;
            StopAllCoroutines();
            StartCoroutine(Execute());
            // Execute().Forget();

            // Target = target;
            // Vector2 ownerPosition = Target.position;
            //
            // var offsets = new[]
            // {
            //     new Vector2(0f, 0f),
            //     new Vector2(-1, 0.35f),
            //     new Vector2(1, 0.35f)
            // };
            //
            // Vector2 randomOffset = offsets[index++%3];
            // // Vector2 randomOffset = offsets[Random.Range(0, 3)];
            //
            // m_offsetFromOwner = offset + randomOffset;
            //
            // Transform.position = ownerPosition + m_offsetFromOwner ;
            //
            // CanTick = true;
            // StopAllCoroutines();
            // StartCoroutine(Execute());
            // // Execute().Forget();
        }

        public override void Tick()
        {
            m_offsetFromOwner += m_moveDirection * Time.deltaTime;

            if (Target != null)
            {
                Transform.position = (Vector2)Target.position + m_offsetFromOwner;
            }
        }

        private IEnumerator Execute()
        {
            float targetScale = Random.Range(MinScale, MaxScale);
            float scaleSpeed = (targetScale - 1.0f) / ScaleToMaxScaleTime;
            m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, 1);

            while (true)
            {
                Vector3 scale = Transform.localScale;
                scale.x += scaleSpeed * Time.deltaTime;
                scale.y += scaleSpeed * Time.deltaTime;

                if (scale.x > targetScale)
                {
                    scale.x = targetScale;
                    scale.y = targetScale;
                    scaleSpeed = -scaleSpeed;
                }
                else if (scale.x < 1.0f)
                {
                    scale.x = 1.0f;
                    scale.y = 1.0f;

                    StartCoroutine(UpdateAnimationStep2_Style1());
                    break;
                }

                Transform.localScale = scale;

                // await UniTask.Yield();
                yield return null;
            }
        }

        private IEnumerator UpdateAnimationStep2_Style1()
        {
            float fadeSpeed = 1.0f / FadeToAlphaZeroTime;

            while (true)
            {
                Color color = m_text.color;
                color.a -= fadeSpeed * Time.deltaTime;

                if (color.a < 0.0f)
                {
                    color.a = 0.0f;
                    break;
                }

                m_text.color = color;

                yield return new WaitForEndOfFrame();
            }

            StopAllCoroutines();
            World.ObjectPool.AddObject(this);
        }
    }
}
