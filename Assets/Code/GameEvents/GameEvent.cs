using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CustomEvents
{

    [CreateAssetMenu(menuName = "Custom/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> listeners = new List<GameEventListener>();

        public void RegisterListener(GameEventListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void UnRegisterListener(GameEventListener listener)
        {
            if (listeners.Contains(listener))
                listeners.Remove(listener);
        }

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }
    }
}
