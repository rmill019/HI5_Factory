using UnityEngine;
using CustomEvents;

public class EventEmitter : MonoBehaviour {

    public GameEvent EventToEmit;


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "FactoryContainer")
            EventToEmit.Raise();
    }
}
