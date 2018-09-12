using UnityEngine;
using UnityEngine.Events;

namespace CustomEvents
{
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("The Event that this Listener is listening for.")]
        public GameEvent Event;

        [Tooltip("The Responses that should fire once the Event is heard.")]
        public UnityEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnRegisterListener(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke();
        }
    }
}
