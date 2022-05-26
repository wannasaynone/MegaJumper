using System;
using STORIAMonetization.Init;
using STORIAMonetization.Event;
using STORIAMonetization.Advertisement;

namespace STORIAMonetization
{
    public class MonetizeCenter
    {
        public static MonetizeCenter Instance
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = new MonetizeCenter();
                }

                return m_instance;
            }
        }

        private static MonetizeCenter m_instance = null;

        public enum State
        {
            None,
            Initializing,
            Normal,
            WaitingCallback
        }

        public State CurrentState { get; private set; }

        /// <summary>
        /// Will rise up BEFORE whenever this runner's initializer starts its process and output its PluginName
        /// </summary>
        public event Action<string> OnAnyInitializerReadyToStart
        {
            add
            {
                m_initRunner.OnAnyInitializerReadyToStart += value;
            }
            remove
            {
                m_initRunner.OnAnyInitializerReadyToStart -= value;
            }
        }
        /// <summary>
        /// Will rise up whenever this runner's initializer have completed its process and output its PluginName
        /// </summary>
        public event Action<string> OnAnyInitializerDone
        {
            add
            {
                m_initRunner.OnAnyInitializerDone += value;
            }
            remove
            {
                m_initRunner.OnAnyInitializerDone -= value;
            }
        }
        private Init.InitializerRunner m_initRunner = null;
        private Action m_onInitialized = null;

        public readonly GameEventSender EventSender = null;
        public readonly AdvertisementManager AdManager = null;

        private MonetizeCenter()
        {
            CurrentState = State.None;
            EventSender = new GameEventSender();
            AdManager = new AdvertisementManager();
        }

        public void Initialize(InitializerBase[] initializers, Action onInitialized)
        {
            if (CurrentState != State.None)
            {
                throw new Exception("[MiniVaultCenter][Initialize] Is trying to initialize MiniVaultCenter but it is initialized.");
            }

            CurrentState = State.Initializing;
            m_onInitialized = onInitialized;
            m_initRunner = new InitializerRunner(initializers);
            m_initRunner.Start(OnAllInited);
        }

        public InitializerRunner.InitizerInfo GetCurrentInitizeProcessInfo()
        {
            System.Collections.Generic.List<string> _names = new System.Collections.Generic.List<string>();
            for(int i = 0; i < m_initRunner.InitializerCount; i++)
            {
                _names.Add(m_initRunner.GetInitializer(i).GetPluginName());
            }

            return new InitializerRunner.InitizerInfo
            {
                currentStep = m_initRunner.CurrnetRunningIndex + 1,
                pluginNames = _names,
                totalStepsCount = m_initRunner.InitializerCount
            };
        }

        private void OnAllInited()
        {
            CurrentState = State.Normal;
            m_onInitialized?.Invoke();
        }
    }
}
