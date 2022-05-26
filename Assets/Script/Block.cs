using UnityEngine;

namespace MegaJumper
{
    public class Block : MonoBehaviour
    {
        public class Factory : Zenject.PlaceholderFactory<Block>
        {

        }
    }
}