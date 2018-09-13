using UnityEngine;
using Hi5_Interaction_Core;

public class TestSpawner : MonoBehaviour {

    public Transform FactoryObjSpawner;
    public Transform FactoryContainerSpawner;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject activeObj = GameManager.S.GrabUnusedFactoryObject();
            activeObj.GetComponent<FactoryObject>().IsInUse = true;
            activeObj.transform.position = FactoryObjSpawner.position;
            activeObj.GetComponent<Rigidbody>().useGravity = true;
            activeObj.GetComponent<Rigidbody>().drag = 8f;     // Magic number so that it doesn't phase through objects
            activeObj.GetComponent<Hi5_Glove_Interaction_Item>().enabled = true;
            activeObj.GetComponent<FactoryObject>().IsFalling = true;
            //activeObj.GetComponent<FactoryObject>().Initialize();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            GameObject activeObj = GameManager.S.GrabUnusedFactoryContainer();
            activeObj.GetComponent<FactoryContainer>().IsInUse = true;
            activeObj.transform.position = FactoryContainerSpawner.position;
            activeObj.GetComponent<Rigidbody>().useGravity = true;
            activeObj.GetComponent<Rigidbody>().drag = 8f;     // Magic number so that it doesn't phase through objects
            activeObj.GetComponent<Hi5_Glove_Interaction_Item>().enabled = true;
            activeObj.GetComponent<FactoryContainer>().IsFalling = true;
            //activeObj.GetComponent<FactoryObject>().Initialize();
        }
    }
}
