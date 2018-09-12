using System.Collections;
using UnityEngine;
using Hi5_Interaction_Core;
using CustomEvents;

public enum E_ScanStatus { Fail, Pass }

public class Scanner : MonoBehaviour
{
    public GameEvent ScanFailed;
    public GameEvent ScanPassed;
    public FloatVariable m_disableObjectTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ScannableObject")
        {
            print("Item Scanned");
            ScanItem (other.gameObject);
        }
    }

    E_ScanStatus ScanItem(GameObject Obj)
    {
        FactoryObject fObj = Obj.GetComponent<FactoryObject>();
        if (fObj != null)
        {
            // todo this amount of 3 refers to the amount of flags set in FactoryObject
            // that need to be checked to see if passes inspection
            for (int i = 0; i < fObj.CheckList.Length; i++)
            {
                if (fObj.CheckList[i])
                    continue;
                else
                {

                    StartCoroutine (RestartFactoryObjectLifeCycle(fObj.gameObject));
                    ScanFailed.Raise();
                    return E_ScanStatus.Fail;
                }
            }
            //print("Success");
            // If all items of the checklist pass then the scan is successful
            StartCoroutine (RestartFactoryObjectLifeCycle(fObj.gameObject));
            ScanPassed.Raise();
            return E_ScanStatus.Pass;
        }

        // If there is no FactoryObject Component attached then send an error
        Debug.LogWarning("No FactoryObject Component found in ScanItem");
        return E_ScanStatus.Fail;
    }

    IEnumerator DisableFactoryObject (GameObject fObj)
    {
        float targetTime = Time.time + m_disableObjectTimer.Value;

        while (Time.time < targetTime)
        {
            yield return null;
        }
        fObj.SetActive(false);
    }

    IEnumerator RestartFactoryObjectLifeCycle(GameObject fObj)
    {
        float targetTime = Time.time + m_disableObjectTimer.Value;

        while (Time.time < targetTime)
        {
            yield return null;
        }

        // RESET ALL PARAMETERS
        // Here we remove Interaction Item so that gravity won't affect the object.
        // We also tell it that it can't move and place it back in it's starting location offscreen
        fObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        fObj.GetComponent<Rigidbody>().useGravity = false;
        fObj.GetComponent<Hi5_Glove_Interaction_Item>().enabled = false;
        fObj.GetComponent<FactoryObject>().CanMove = false;
        fObj.transform.position = fObj.GetComponent<FactoryObject>().m_spawnPos;
        fObj.GetComponent<FactoryObject>().IsInUse = false;

    }
}
