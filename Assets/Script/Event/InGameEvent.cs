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

        }

        public class OnJumpEnded
        {
            public UnityEngine.Vector3 Position { get; private set; }

            public OnJumpEnded(UnityEngine.Vector3 position)
            {
                Position = position;
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
    }
}
