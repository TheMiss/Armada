using System;
using System.Collections.Generic;
using Armageddon.UI.Base;
using CodeStage.AdvancedFPSCounter;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI.Common
{
    public enum StatsMonitorBarMode
    {
        None,
        Fps,
        Full,
        FullAndDeviceDetails
    }

    public class StatsMonitorBar : Widget
    {
        [OnValueChanged(nameof(SetMode))]
        [SerializeField]
        private StatsMonitorBarMode m_mode;

        private readonly List<Vector2> m_gesturePoints = new();
        private int m_gestureCount;

        public StatsMonitorBarMode Mode
        {
            get => m_mode;
            set => SetMode(value);
        }

        protected override void Awake()
        {
            base.Awake();

            RegisterService(this);

            SetMode(m_mode);
        }

        protected override void Start()
        {
            CanTick = true;
        }

        private void SetMode(StatsMonitorBarMode mode)
        {
            m_mode = mode;

            var fpsCounter = AFPSCounter.Instance;

            switch (mode)
            {
                case StatsMonitorBarMode.None:
                    fpsCounter.OperationMode = OperationMode.Disabled;
                    break;
                case StatsMonitorBarMode.Fps:
                    fpsCounter.OperationMode = OperationMode.Normal;
                    fpsCounter.fpsCounter.Enabled = true;
                    fpsCounter.fpsCounter.Average = false;
                    fpsCounter.fpsCounter.Milliseconds = false;
                    fpsCounter.fpsCounter.MinMax = false;
                    fpsCounter.fpsCounter.AverageMilliseconds = false;

                    fpsCounter.memoryCounter.Enabled = false;
                    fpsCounter.deviceInfoCounter.Enabled = false;

                    break;
                case StatsMonitorBarMode.Full:
                    fpsCounter.OperationMode = OperationMode.Normal;
                    fpsCounter.fpsCounter.Average = true;
                    fpsCounter.fpsCounter.Milliseconds = true;
                    fpsCounter.fpsCounter.MinMax = true;
                    fpsCounter.fpsCounter.AverageMilliseconds = true;

                    fpsCounter.memoryCounter.Enabled = true;
                    fpsCounter.deviceInfoCounter.Enabled = false;

                    break;
                case StatsMonitorBarMode.FullAndDeviceDetails:
                    fpsCounter.OperationMode = OperationMode.Normal;
                    fpsCounter.fpsCounter.Average = true;
                    fpsCounter.fpsCounter.Milliseconds = true;
                    fpsCounter.fpsCounter.MinMax = true;
                    fpsCounter.fpsCounter.AverageMilliseconds = true;

                    fpsCounter.memoryCounter.Enabled = true;
                    fpsCounter.deviceInfoCounter.Enabled = true;

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public override void Tick()
        {
            switch (m_mode)
            {
                case StatsMonitorBarMode.None:
                    break;
                case StatsMonitorBarMode.Fps:
                    break;
                case StatsMonitorBarMode.Full:
                    break;
                case StatsMonitorBarMode.FullAndDeviceDetails:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SwitchMode();
            }

            if (CircleGestureMade())
            {
                SwitchMode();
            }
        }

        private void SwitchMode()
        {
            int mode = (int)m_mode;
            mode++;
            if (mode > (int)StatsMonitorBarMode.FullAndDeviceDetails)
            {
                mode = (int)StatsMonitorBarMode.None;
            }

            Mode = (StatsMonitorBarMode)mode;
        }

        private bool CircleGestureMade()
        {
            int pointsCount = m_gesturePoints.Count;

            if (Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Input.mousePosition;
                if (pointsCount == 0 || (mousePosition - m_gesturePoints[pointsCount - 1]).magnitude > 10)
                {
                    m_gesturePoints.Add(mousePosition);
                    pointsCount++;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                pointsCount = 0;
                m_gestureCount = 0;
                m_gesturePoints.Clear();
            }

            if (pointsCount < 10)
            {
                return false;
            }

            float finalDeltaLength = 0;

            Vector2 finalDelta = Vector2.zero;
            Vector2 previousPointsDelta = Vector2.zero;

            for (int i = 0; i < pointsCount - 2; i++)
            {
                Vector2 pointsDelta = m_gesturePoints[i + 1] - m_gesturePoints[i];
                finalDelta += pointsDelta;

                float pointsDeltaLength = pointsDelta.magnitude;
                finalDeltaLength += pointsDeltaLength;

                float dotProduct = Vector2.Dot(pointsDelta, previousPointsDelta);
                if (dotProduct < 0f)
                {
                    m_gesturePoints.Clear();
                    m_gestureCount = 0;
                    return false;
                }

                previousPointsDelta = pointsDelta;
            }

            bool result = false;
            int gestureBase = (Screen.width + Screen.height) / 4;

            if (finalDeltaLength > gestureBase && finalDelta.magnitude < gestureBase / 2f)
            {
                m_gesturePoints.Clear();
                m_gestureCount++;

                if (m_gestureCount >= 2)
                {
                    m_gestureCount = 0;
                    result = true;
                }
            }

            return result;
        }
    }
}
