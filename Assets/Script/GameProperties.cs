namespace MegaJumper
{
    [System.Serializable]
    public class GameProperties
    {
        public float MAX_ADD_DISTANCE = 12f;
        public float MIN_ADD_DISTANCE = 5f;
        public int MAX_CLONE_COUNT = 10;
        public float MOVE_DIS_PER_SEC = 6f;
        public float GAMEOVER_DIS = 2f;
        public UnityEngine.Vector3 CAMERA_OFFSET = default;
        public UnityEngine.Vector3 CAMERA_ANGLE_OFFSET = default;
        public float CAMERA_SHAKE_FORCE = 1f;
        public float CAMERA_SHAKE_TIME = 0.25f;
        public float JUMP_FORCE = 5f;
        public float JUMP_TIME_SCALE = 0.75f;
    }
}