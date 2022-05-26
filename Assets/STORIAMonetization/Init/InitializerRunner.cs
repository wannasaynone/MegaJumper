using System;

namespace STORIAMonetization.Init
{
    public class InitializerRunner
    {
        public struct InitizerInfo
        {
            public int totalStepsCount;
            public int currentStep;
            public System.Collections.Generic.List<string> pluginNames;
        }

        /// <summary>
        /// Will rise up BEFORE whenever this runner's initializer starts its process and output its InitializerName
        /// </summary>
        public event Action<string> OnAnyInitializerReadyToStart = null;

        /// <summary>
        /// Will rise up whenever this runner's initializer have completed its process and output its InitializerName
        /// </summary>
        public event Action<string> OnAnyInitializerDone = null;

        private InitializerBase[] m_initializers;

        public int InitializerCount { get { return m_initializers.Length; } }
        public int CurrnetRunningIndex { get; private set; }
        private Action m_onAllDone;

        public InitializerRunner(InitializerBase[] initializers)
        {
            ResetInitializers(initializers);
        }

        public void ResetInitializers(InitializerBase[] initializers)
        {
            m_initializers = initializers;
            if (m_initializers == null)
            {
                m_initializers = new InitializerBase[0];
            }
        }

        public void Start(Action onAllInitialized)
        {
            InitializerBase.OnInitialFailed += OnInitialFailed;
            m_onAllDone = onAllInitialized;
            CurrnetRunningIndex = -1;
            GoNext();
        }

        private void OnInitialFailed(InitializerBase.FailedInfo failedInfo)
        {
            InitializerBase.OnInitialFailed -= OnInitialFailed;
            throw new Exception("[InitializerRunner] Initializing FAILED and InitializerRunner suspended due to the error!!!! Error Messages are below:\n\n====================\nCurrent Plugin=" + failedInfo.pluginName + "\nError Msg=" + failedInfo.errorMessage + "\n====================\n\n");
        }

        public InitializerBase GetInitializer(int index)
        {
            if (index < 0 || index >= m_initializers.Length)
            {
                UnityEngine.Debug.LogError("[InitializerRunner][GetInitializer] input index must greater then zero and less then m_initializers.Length. It will return null here. index=" + index + ", m_initializers.Length = " + m_initializers.Length);
                return null;
            }

            return m_initializers[index];
        }

        private void GoNext()
        {
            CurrnetRunningIndex++;
            if (CurrnetRunningIndex >= m_initializers.Length)
            {
                InitializerBase.OnInitialFailed -= OnInitialFailed;
                m_onAllDone?.Invoke();
                return;
            }

            OnAnyInitializerReadyToStart?.Invoke(m_initializers[CurrnetRunningIndex].GetPluginName());
            m_initializers[CurrnetRunningIndex].Start(OnInitializerDone);
        }

        private void OnInitializerDone()
        {
            OnAnyInitializerDone?.Invoke(m_initializers[CurrnetRunningIndex].GetPluginName());
            GoNext();
        }
    }
}

