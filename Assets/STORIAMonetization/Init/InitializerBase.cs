using System;

namespace STORIAMonetization.Init
{
    public abstract class InitializerBase
    {
        public class FailedInfo
        {
            public string pluginName;
            public string errorMessage;
        }

        public static event Action<FailedInfo> OnInitialFailed;

        public abstract void Start(Action onDone);
        public abstract string GetPluginName();

        protected void Panic(FailedInfo failedInfo)
        {
            OnInitialFailed?.Invoke(failedInfo);
        }
    }
}
