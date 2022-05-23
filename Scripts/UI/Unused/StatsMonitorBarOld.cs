using System;
using System.Collections.Generic;
using Armageddon.UI.Base;
using CodeStage.AdvancedFPSCounter;
using CodeStage.AdvancedFPSCounter.CountersData;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Armageddon.UI.Unused
{
    public enum StatsMonitorBarMode
    {
        Overall,
        FpsCounter,
        MemoryCounter
    }

    public class StatsMonitorBarOld : Widget, IPointerClickHandler
    {
        private const float Torrence = 0.0001f;

        [SerializeField]
        private Transform m_safePanelTransform;

        [SerializeField]
        private List<RectTransform> m_rowTransforms;

        [SerializeField]
        private TextMeshProUGUI m_fpsText;

        [SerializeField]
        private TextMeshProUGUI m_averageText;

        [SerializeField]
        private TextMeshProUGUI m_minText;

        [SerializeField]
        private TextMeshProUGUI m_maxText;

        [SerializeField]
        private TextMeshProUGUI m_renderText;

        [SerializeField]
        private TextMeshProUGUI m_memTotalText;

        [SerializeField]
        private TextMeshProUGUI m_memAllocText;

        [SerializeField]
        private TextMeshProUGUI m_memMonoText;

        [SerializeField]
        private TextMeshProUGUI m_memGfxText;

        private int m_avg;

        private int m_fps;

        private int m_max;

        private int m_memAlloc;

        private float m_memGfx;

        private float m_memMono;

        private float m_memTotal;

        private int m_min;

        private float m_ren;

        [ShowInInspector]
        public StatsMonitorBarMode Mode { private set; get; }

        protected override void OnEnable()
        {
            base.OnEnable();

            AdjustMode();

            CanTick = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int modeInt = (int)Mode;
            modeInt++;
            modeInt %= (int)StatsMonitorBarMode.MemoryCounter + 1;

            Mode = (StatsMonitorBarMode)modeInt;

            AdjustMode();
        }

        private void SetTextToRow(TextMeshProUGUI text, int row)
        {
            text.gameObject.SetActive(true);
            text.transform.SetParent(m_rowTransforms[row]);
        }

        private void SetAllTextsToInactive()
        {
            TextMeshProUGUI[] texts = m_safePanelTransform.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                text.transform.SetParent(m_safePanelTransform);
                text.gameObject.SetActive(false);
            }
        }

        private void AdjustMode()
        {
            SetAllTextsToInactive();

            switch (Mode)
            {
                case StatsMonitorBarMode.Overall:
                    SetTextToRow(m_fpsText, 0);
                    SetTextToRow(m_averageText, 0);
                    SetTextToRow(m_minText, 1);
                    SetTextToRow(m_maxText, 1);
                    SetTextToRow(m_memTotalText, 2);
                    SetTextToRow(m_memAllocText, 3);
                    break;
                case StatsMonitorBarMode.FpsCounter:
                    SetTextToRow(m_fpsText, 0);
                    SetTextToRow(m_averageText, 1);
                    SetTextToRow(m_minText, 2);
                    SetTextToRow(m_maxText, 2);
                    SetTextToRow(m_renderText, 3);
                    break;
                case StatsMonitorBarMode.MemoryCounter:
                    SetTextToRow(m_memTotalText, 0);
                    SetTextToRow(m_memAllocText, 1);
                    SetTextToRow(m_memMonoText, 2);
                    SetTextToRow(m_memGfxText, 3);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Tick()
        {
            FPSCounterData fpsCounter = AFPSCounter.Instance.fpsCounter;
            MemoryCounterData memCounter = AFPSCounter.Instance.memoryCounter;

            SetFps(fpsCounter.LastValue);
            SetAvg(fpsCounter.LastAverageValue);
            SetMin(fpsCounter.LastMinimumValue);
            SetMax(fpsCounter.LastMaximumValue);
            SetRen(fpsCounter.LastRenderValue);

            const float divider = MemoryCounterData.MemoryDivider;
            SetMemTotal(memCounter.LastTotalValue / divider);
            SetMemAlloc(memCounter.LastAllocatedValue / divider);
            SetMemMono(memCounter.LastMonoValue / divider);
            SetMemGfx(memCounter.LastGfxValue / divider);
        }

        private void SetFps(int fps, float ms = -1)
        {
            if (m_fps != fps)
            {
                string text = $"FPS: {fps}";

                if (ms > -1)
                {
                    text += $" [{ms:0.00} MS]";
                }

                m_fpsText.text = text;
            }
        }

        private void SetAvg(int avg, float ms = -1)
        {
            if (m_avg != avg)
            {
                string text = $"AVG: {avg}";

                if (ms > -1)
                {
                    text += $" [{ms:0.00} MS]";
                }

                m_averageText.text = text;
            }
        }

        private void SetMin(int min)
        {
            if (m_min != min)
            {
                string text = $"Min: {min}";

                m_minText.text = text;
            }
        }

        private void SetMax(int max)
        {
            if (m_max != max)
            {
                string text = $"Max: {max}";

                m_maxText.text = text;
            }
        }

        private void SetRen(float ren)
        {
            if (Math.Abs(m_ren - ren) > Torrence)
            {
                string text = $"Ren: {ren:0.00} MS";

                m_renderText.text = text;
            }
        }

        private void SetMemTotal(float memTotal)
        {
            if (Math.Abs(m_memTotal - memTotal) > Torrence)
            {
                string text = $"M Total: {memTotal:0.00} MB";

                m_memTotalText.text = text;
            }
        }

        private void SetMemAlloc(float memAlloc)
        {
            if (m_memAlloc != (int)memAlloc)
            {
                string text = $"M Alloc: {memAlloc:0.00} MB";

                m_memAllocText.text = text;
            }
        }

        private void SetMemMono(float memMono)
        {
            if (Math.Abs(m_memMono - memMono) > Torrence)
            {
                string text = $"M Mono: {memMono:0.00} MB";

                m_memMonoText.text = text;
            }
        }

        private void SetMemGfx(float memGfx)
        {
            if (Math.Abs(m_memGfx - memGfx) > Torrence)
            {
                string text = $"M GFX: {memGfx:0.00} MB";

                m_memGfxText.text = text;
            }
        }
    }
}
