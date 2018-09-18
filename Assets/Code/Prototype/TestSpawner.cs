using UnityEngine;
using CustomEvents;
using Hi5_Interaction_Core;

public class TestSpawner : MonoBehaviour {

    public Transform FactoryObjSpawnLoc;
    public Transform FactoryContainerSpawnLoc;
    public GameEvent SpawnFactoryObjectEvent;

    public static GameObject CurrentContainer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Factory Object Event Raised");
            SpawnFactoryObjectEvent.Raise();
        }
    }

    public void SpawnFactoryObject ()
    {
        GameObject activeObj = GameManager.S.GrabUnusedFactoryObject();
        activeObj.GetComponent<FactoryObject>().IsInUse = true;
        activeObj.transform.position = FactoryObjSpawnLoc.position;
        activeObj.GetComponent<Rigidbody>().useGravity = true;
        activeObj.GetComponent<Rigidbody>().drag = 8f;     // Magic number so that it doesn't phase through objects
        activeObj.GetComponent<Hi5_Glove_Interaction_Item>().enabled = true;
        activeObj.GetComponent<FactoryObject>().IsFalling = true;
        //activeObj.GetComponent<FactoryObject>().Initialize();
    }

    // This may need to be tweaked. Because the object isn't falling. It's spawning then raising up
    public void SpawnFactoryContainer ()
    {
        // Changed useGravity and enabled to false. We need a way to set it to true after a specified amount of time
        GameObject activeObj = GameManager.S.GrabUnusedFactoryContainer();
        activeObj.transform.parent = FactoryContainerSpawnLoc;
        activeObj.GetComponent<FactoryContainer>().IsInUse = true;
        activeObj.transform.position = FactoryContainerSpawnLoc.position;
        activeObj.GetComponent<Rigidbody>().useGravity = false;
        activeObj.GetComponent<Rigidbody>().drag = 8f;     // Magic number so that it doesn't phase through objects
        activeObj.GetComponent<Hi5_Glove_Interaction_Item>().enabled = false;
        activeObj.GetComponent<FactoryContainer>().IsFalling = false;
        CurrentContainer = activeObj;
        //activeObj.GetComponent<FactoryObject>().Initialize();
    }
}
