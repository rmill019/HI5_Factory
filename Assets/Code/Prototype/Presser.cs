using UnityEngine;
using CustomEvents;

// Pass in a GameObject to this class so that the AnimationEvent can know when to disable
// and enable gravity and InteractionItem
public class Presser : MonoBehaviour {

    public AnimationClip pressClip;
    public GameEvent PressDownAudioEvent;
    public float m_animSpeedIncrease;
    private Animator m_anim;

    public GameObject CurrentContainer
    {
        get { return CurrentContainer; }
        set { CurrentContainer = value; }
    }

	// Use this for initialization
	void Start () {

        m_anim = GetComponent<Animator>();
    }

    // todo delete this testing function
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            ActivatePresser();
    }

    public void ActivatePresser ()
    {
        m_anim.SetTrigger("Press");
    }

    public void PressDownAudio ()
    {
        PressDownAudioEvent.Raise();
    }

    public void IncreasePressSpeed ()
    {
        m_anim.speed += m_animSpeedIncrease;
    }
}
