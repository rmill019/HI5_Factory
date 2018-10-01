using UnityEngine;
using CustomEvents;
using Hi5_Interaction_Core;

public class ContainerMechanism : MonoBehaviour {

    public AnimationClip pressClip;
    public GameEvent spawnContainerEvent;
    private Animator m_anim;

    public GameObject CurrentContainer
    {
        get { return CurrentContainer; }
        set { CurrentContainer = value; }
    }

    // Use this for initialization
    void Start()
    {

        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_anim.SetTrigger("Press");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            print("Increase animation speed");
            m_anim.speed += 0.5f;
        }
    }

    public void ActivateContainerBench ()
    {
        m_anim.SetTrigger("Press");
    }

    // This is called through an Animation Event on the Refill Animation of "ContainerBench" Prefab
    public void ReplenishContainer()
    {
        print("Container Replenished");
        spawnContainerEvent.Raise();
    }

    public void FreeContainer()
    {
        if (TestSpawner.CurrentContainer != null)
        {
            TestSpawner.CurrentContainer.transform.parent = null;
            TestSpawner.CurrentContainer.GetComponent<Rigidbody>().useGravity = true;
            TestSpawner.CurrentContainer.GetComponent<Hi5_Glove_Interaction_Item>().enabled = true;
            TestSpawner.CurrentContainer = null;
        }
        else
            print("NOT FREE");
    }
}
