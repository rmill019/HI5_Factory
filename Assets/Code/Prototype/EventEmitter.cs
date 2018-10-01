using UnityEngine;
using CustomEvents;

// This Emits or fires the specified event. And has a bool that lets us specify if
// we want this to fire off once in the beginning.

    // TODO This will have to be reworked once we determine how we are going to actually begin the game
public class EventEmitter : MonoBehaviour {

    public GameEvent EventToEmit;
    public bool FireOnceInBeginning = false;
    public bool ShouldRepeat = false;
    public FloatVariable m_interval;

    private float timer;
    private bool b_hasFired = false;

    private void Start()
    {
        timer = Time.time + m_interval.Value;
    }

    private void Update()
    {
        if (ShouldRepeat && GameManager.S.GameStarted)
        {
            EmitEvent();
            return;
        }
        else if (!ShouldRepeat && GameManager.S.GameStarted)
            EmitOnce(); 
    }

    void EmitEvent ()
    {
        if (Time.time > timer)
        {
            EventToEmit.Raise();
            timer = Time.time + m_interval.Value;
        }
    }

    void EmitOnce ()
    {
        if (Time.time > timer && !b_hasFired && FireOnceInBeginning)
        {
            EventToEmit.Raise();
            b_hasFired = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "FactoryContainer")
            EventToEmit.Raise();
    }
}
