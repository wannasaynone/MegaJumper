using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace STORIAMonetization.Event
{
    public interface IEventSendable
    {
        void Send(string msg);
    }

    public interface IGameEventSendable
    {
        void SendGameStartEvent(int level);
        void SendGameStartEvent(GameEventSender.GameEventArgs agrs);
        void SendGameWinEvent(int level);
        void SendGameWinEvent(GameEventSender.GameEventArgs agrs);
        void SendGameLoseEvent(int level);
        void SendGameLoseEvent(GameEventSender.GameEventArgs agrs);
    }

    public class GameEventSender
    {
        public struct GameEventArgs
        {
            public int level;
            public int scroe;
        }

        private List<IEventSendable> m_eventSenders = new List<IEventSendable>();
        private List<IGameEventSendable> m_gameEventSenders = new List<IGameEventSendable>();
        // IAP sender here

        public void RegisterCustomEventSender(IEventSendable sender)
        {
            if (m_eventSenders.Contains(sender))
            {
                return;
            }
            m_eventSenders.Add(sender);
        }

        public void RegisterGameEventSender(IGameEventSendable sender)
        {
            if (m_gameEventSenders.Contains(sender))
            {
                return;
            }
            m_gameEventSenders.Add(sender);
        }

        public void Unregister(object sender)
        {
            if ((IEventSendable)sender != null) m_eventSenders.Remove((IEventSendable)sender);
            if ((IGameEventSendable)sender != null) m_gameEventSenders.Remove((IGameEventSendable)sender);
        }

        public void SendCustomEvent(string msg)
        {
            if (Regex.IsMatch(msg, "[^A-Za-z0-9_]"))
            {
                throw new Exception("[MiniVaultEventSender][SendCutsomEvent] Message can ONLY contain character and \"_\". msg=" + msg);
            }

            if (MonetizeCenter.Instance.CurrentState == MonetizeCenter.State.Initializing)
            {
                throw new Exception("[MiniVaultEventSender][Send] SDKs are not initialized not. Please use MiniVaultCenter.Instance.Initialize first.");
            }

            for(int i = 0; i < m_eventSenders.Count; i++)
            {
                m_eventSenders[i].Send(msg);
            }
        }

        public void SendGameStartEvent(int level)
        {
            if (MonetizeCenter.Instance.CurrentState == MonetizeCenter.State.Initializing)
            {
                throw new Exception("[MiniVaultEventSender][Send] SDKs are not initialized not. Please use MiniVaultCenter.Instance.Initialize first.");
            }

            for (int i = 0; i < m_gameEventSenders.Count; i++)
            {
                m_gameEventSenders[i].SendGameStartEvent(level);
            }
        }

        public void SendGameWinEvent(int score)
        {
            if (MonetizeCenter.Instance.CurrentState == MonetizeCenter.State.Initializing)
            {
                throw new Exception("[MiniVaultEventSender][Send] SDKs are not initialized not. Please use MiniVaultCenter.Instance.Initialize first.");
            }

            for (int i = 0; i < m_gameEventSenders.Count; i++)
            {
                m_gameEventSenders[i].SendGameWinEvent(score);
            }
        }

        public void SendGameWinEvent(GameEventArgs args)
        {
            if (MonetizeCenter.Instance.CurrentState == MonetizeCenter.State.Initializing)
            {
                throw new Exception("[MiniVaultEventSender][Send] SDKs are not initialized not. Please use MiniVaultCenter.Instance.Initialize first.");
            }

            for (int i = 0; i < m_gameEventSenders.Count; i++)
            {
                m_gameEventSenders[i].SendGameWinEvent(args);
            }
        }

        public void SendGameLoseEvent(int score)
        {
            if (MonetizeCenter.Instance.CurrentState == MonetizeCenter.State.Initializing)
            {
                throw new Exception("[MiniVaultEventSender][Send] SDKs are not initialized not. Please use MiniVaultCenter.Instance.Initialize first.");
            }

            for (int i = 0; i < m_gameEventSenders.Count; i++)
            {
                m_gameEventSenders[i].SendGameLoseEvent(score);
            }
        }

        public void SendGameLoseEvent(GameEventArgs args)
        {
            if (MonetizeCenter.Instance.CurrentState == MonetizeCenter.State.Initializing)
            {
                throw new Exception("[MiniVaultEventSender][Send] SDKs are not initialized not. Please use MiniVaultCenter.Instance.Initialize first.");
            }

            for (int i = 0; i < m_gameEventSenders.Count; i++)
            {
                m_gameEventSenders[i].SendGameLoseEvent(args);
            }
        }
    }
}
