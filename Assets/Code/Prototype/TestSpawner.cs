using UnityEngine;
using Hi5_Interaction_Core;

public class TestSpawner : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject activeObj = GameManager.S.GrabUnusedFactoryObject();
            activeObj.GetComponent<FactoryObject>().IsInUse = true;
            activeObj.transform.position = transform.position;
            activeObj.GetComponent<Rigidbody>().useGravity = true;
            activeObj.GetComponent<Hi5_Glove_Interaction_Item>().enabled = true;
            activeObj.GetComponent<FactoryObject>().Initialize();
        }
	}
}
