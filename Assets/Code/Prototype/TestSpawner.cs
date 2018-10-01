using UnityEngine;
using CustomEvents;
using Hi5_Interaction_Core;

public class TestSpawner : MonoBehaviour {


    public Transform FactoryObjSpawnLoc;
    public Transform FactoryContainerSpawnLoc;
    public GameEvent SpawnFactoryObjectEvent;
    public GameEvent SpawnFactoryContainerEvent;
    public float m_retryTime = 2f;
    private bool bCanRetry = false;
    private float m_timer;

    public static GameObject CurrentContainer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Factory Object Event Raised");
            SpawnFactoryObjectEvent.Raise();
        }

        // In case we could not spawn a container we will keep retrying in Update
        if (bCanRetry && Time.time > m_timer)
            SpawnFactoryContainerEvent.Raise();
    }

    // Make this a template?
    public void SpawnFactoryObject ()
    {
        GameObject activeObj = GameManager.S.GrabUnusedFactoryObject();
        // Ensure that a gameObject was available
        if (activeObj)
        {
            activeObj.GetComponent<FactoryObject>().IsInUse = true;
            activeObj.transform.position = FactoryObjSpawnLoc.position;
            activeObj.GetComponent<Rigidbody>().useGravity = true;
            activeObj.GetComponent<Rigidbody>().drag = 8f;     // Magic number so that it doesn't phase through objects
            activeObj.GetComponent<Hi5_Glove_Interaction_Item>().enabled = true;
            activeObj.GetComponent<FactoryObject>().IsFalling = true;
            //activeObj.GetComponent<FactoryObject>().Initialize();
        }
    }

    // This may need to be tweaked. Because the object isn't falling. It's spawning then raising up
    public void SpawnFactoryContainer ()
    {
        // Changed useGravity and enabled to false. We need a way to set it to true after a specified amount of time
        GameObject activeObj = GameManager.S.GrabUnusedFactoryContainer();
        // Make sure that a GameObject was available
        if (activeObj)
        {
            bCanRetry = false;
            activeObj.transform.parent = FactoryContainerSpawnLoc;
            activeObj.GetComponent<FactoryContainer>().IsInUse = true;
            activeObj.transform.position = FactoryContainerSpawnLoc.position;
            activeObj.GetComponent<Rigidbody>().useGravity = false;
            activeObj.GetComponent<Rigidbody>().drag = 8f;     // Magic number so that it doesn't phase through objects
            activeObj.GetComponent<Hi5_Glove_Interaction_Item>().enabled = false;
            activeObj.GetComponent<FactoryContainer>().IsFalling = false;
            CurrentContainer = activeObj;
            //activeObj.GetComponent<FactoryContainer>().Initialize();
            return;
        }

        // Need to fire event here or in Update
        bCanRetry = true;
        m_timer = Time.time + m_retryTime;
        
    }

}
