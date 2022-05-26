using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MegaJumper
{
    public class Jumper : MonoBehaviour
    {
        private GameProperties m_gameProperties;

        [Inject]
        public void Constructor(SignalBus signalBus, GameProperties gameProperties)
        {
            m_gameProperties = gameProperties;
            signalBus.Subscribe<Event.InGameEvent.OnPointUp>(OnPointUp);
        }

        private void OnPointUp(Event.InGameEvent.OnPointUp obj)
        {
            transform.position += Vector3.forward * obj.PressTime * m_gameProperties.MOVE_DIS_PER_SEC;
        }
    }
}