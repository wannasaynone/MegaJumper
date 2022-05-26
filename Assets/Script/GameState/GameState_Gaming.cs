using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MegaJumper.GameState
{
    public class GameState_Gaming : GameStateBase
    {
        public GameState_Gaming(SignalBus signalBus) : base(signalBus)
        {
        }

        public override void Start()
        {
            Debug.Log("GameState_Gaming Start");
        }

        public override void Tick()
        {
        }
    }
}