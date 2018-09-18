using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTopper : MonoBehaviour
{
    public GameObject BoxTopperPrefab;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "FactoryContainer":
                FactoryContainer fCon = other.gameObject.GetComponent<FactoryContainer>();
                // Grab the third child in the Hierarchy and turn on the MeshRenderer
                other.gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().enabled = true;
                other.gameObject.transform.GetChild(2).GetComponent<MeshCollider>().isTrigger = false;
                fCon.IsTopped = true;
                fCon.UpdateCheckList();
                break;
            default:
                break;
        }
    }
}
