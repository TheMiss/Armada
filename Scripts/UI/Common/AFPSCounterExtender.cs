using System;
using Armageddon.UI.Base;
using CodeStage.AdvancedFPSCounter;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Armageddon.UI.Common
{
    // ReSharper disable once InconsistentNaming
    public enum AFPSCounterExtenderMode
    {
        None,
        Fps,
        Full,
        FullAndDeviceDetails
    }

    // ReSharper disable once InconsistentNaming
    public class AFPSCounterExtender : Widget, IPointerUpHandler
    {
        [OnValueChanged(nameof(SetMode))]
        [SerializeField]
        private AFPSCounterExtenderMode m_mode = AFPSCounterExtenderMode.Fps;

        public AFPSCounterExtenderMode Mode
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

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log($"{name} is up!");
            SwitchMode();
        }

        private void SetMode(AFPSCounterExtenderMode mode)
        {
            m_mode = mode;

            var fpsCounter = AFPSCounter.Instance;

            switch (mode)
            {
                case AFPSCounterExtenderMode.None:
                    fpsCounter.OperationMode = OperationMode.Disabled;
                    break;
                case AFPSCounterExtenderMode.Fps:
                    fpsCounter.OperationMode = OperationMode.Normal;
                    fpsCounter.fpsCounter.Enabled = true;
                    fpsCounter.fpsCounter.Average = false;
                    fpsCounter.fpsCounter.Milliseconds = false;
                    fpsCounter.fpsCounter.MinMax = false;
                    fpsCounter.fpsCounter.AverageMilliseconds = false;

                    fpsCounter.memoryCounter.Enabled = false;
                    fpsCounter.deviceInfoCounter.Enabled = false;

                    break;
                case AFPSCounterExtenderMode.Full:
                    fpsCounter.OperationMode = OperationMode.Normal;
                    fpsCounter.fpsCounter.Average = true;
                    fpsCounter.fpsCounter.Milliseconds = true;
                    fpsCounter.fpsCounter.MinMax = true;
                    fpsCounter.fpsCounter.AverageMilliseconds = true;

                    fpsCounter.memoryCounter.Enabled = true;
                    fpsCounter.deviceInfoCounter.Enabled = false;

                    break;
                case AFPSCounterExtenderMode.FullAndDeviceDetails:
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
                case AFPSCounterExtenderMode.None:
                    break;
                case AFPSCounterExtenderMode.Fps:
                    break;
                case AFPSCounterExtenderMode.Full:
                    break;
                case AFPSCounterExtenderMode.FullAndDeviceDetails:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.RightShift))
            {
                SwitchMode();
            }
        }

        private void SwitchMode()
        {
            int mode = (int)m_mode;
            mode++;
            if (mode > (int)AFPSCounterExtenderMode.FullAndDeviceDetails)
            {
                mode = (int)AFPSCounterExtenderMode.Fps;
            }

            Mode = (AFPSCounterExtenderMode)mode;
        }
    }
}
