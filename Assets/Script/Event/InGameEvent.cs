namespace MegaJumper.Event
{
    public class InGameEvent
    {
        public class OnPointDown
        {

        }

        public class OnPointUp
        {
            public float PressTime { get; private set; }

            public OnPointUp(float pressTime)
            {
                PressTime = pressTime;
            }
        }

        public class OnStartJump
        {
            public bool IsFeverJump { get; private set; }
            public float PressTime { get; private set; }

            public OnStartJump()
            {
                IsFeverJump = false;
                PressTime = 0f;
            }

            public OnStartJump(bool isFeverJump, float pressTime)
            {
                IsFeverJump = isFeverJump;
                PressTime = pressTime;
            }
        }

        public class OnJumpEnded
        {
            public UnityEngine.Vector3 Position { get; private set; }
            public bool IsSuccess { get; private set; }
            public bool IsPerfect { get; private set; }
            public int RemainingLife { get; private set; }

            public OnJumpEnded(UnityEngine.Vector3 position, bool isSuccess, bool isPerfect, int remainingLife)
            {
                Position = position;
                IsSuccess = isSuccess;
                IsPerfect = isPerfect;
                RemainingLife = remainingLife;
            }
        }

        public class OnStartFever
        {

        }

        public class OnFeverEnded
        {

        }

        public class OnBlockSpawned
        {
            public Block Block { get; private set; }

            public OnBlockSpawned(Block block)
            {
                Block = block;
            }
        }

        public class OnGameStarted
        {

        }

        public class OnGameEnded
        {

        }

        public class OnGameResetCalled
        {
            public bool StartWithFever { get; private set; }

            public OnGameResetCalled(bool startWithFever)
            {
                StartWithFever = startWithFever;
            }
        }

        public class OnScoreAdded
        {
            public int Add { get; private set; }
            public int Current { get; private set; }

            public OnScoreAdded(int add, int current)
            {
                Add = add;
                Current = current;
            }
        }

        public class OnScoreReset
        {

        }

        public class OnJumperSettingSet
        {
            public JumperSetting JumperSetting { get; private set; }

            public OnJumperSettingSet(JumperSetting jumperSetting)
            {
                JumperSetting = jumperSetting;
            }
        }

        public class OnComboAdded
        {
            public int Current { get; private set; }
            public int FeverCombo { get; private set; }

            public OnComboAdded(int current, int feverCombo)
            {
                Current = current;
                FeverCombo = feverCombo;
            }
        }

        public class OnComboReset
        {
            
        }

        public class OnTutorialStart
        {

        }

        public class OnTutorialEnded
        {

        }

        public class OnStartRevive
        {

        }
    }
}
