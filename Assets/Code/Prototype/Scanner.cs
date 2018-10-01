using System.Collections;
using UnityEngine;
using Hi5_Interaction_Core;
using CustomEvents;

public enum E_ScanStatus { Fail, Pass }
public enum E_FactoryItemType { Object, Container }

public class Scanner : MonoBehaviour
{
    public GameEvent ScanFailed;
    public GameEvent ScanPassed;
    public FloatVariable m_disableObjectTimer;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag == "ScannableObject" || other.gameOb)
        //{
        //    // Need to refactor this to work with FactoryObjects and FactoryContainers
        //    print("Item Scanned");
        //    //if (other.GetComponent<FactoryObject>())
        //    //    //ScanItem (other.gameObject, EFactoryType.FactoryObject);

        //    //if (other.GetComponent<FactoryContainer>())
        //    //    //ScanItem (other.gameObject, EFactoryType.FactoryContainer);
        //}
        switch (other.gameObject.tag)
        {
            case "FactoryContainer":
            case "ScannableObject":
                if (other.GetComponent<FactoryObject>())
                {
                    ScanItem(other.gameObject, EFactoryType.FactoryObject);
                }

                if (other.GetComponent<FactoryContainer>())
                {
                    ScanItem(other.gameObject, EFactoryType.FactoryContainer);
                }
                break;
            default:
                break;
        }
    }

    E_ScanStatus ScanItem(GameObject Obj, EFactoryType objType)
    {
        FactoryContainer fObj = null;
        switch (objType)
        {
            // Automatic Scan fail for scanning a FactoryObject by itselt
            case EFactoryType.FactoryObject:
                StartCoroutine(RestartFactoryObjectLifeCycle(Obj, EFactoryType.FactoryObject));
                ScanFailed.Raise();
                return E_ScanStatus.Fail;
                // Check that the Factory Container passes the test by having all items in CheckList and then
                // Fire appropriate Game Event
            case EFactoryType.FactoryContainer:
                fObj = Obj.GetComponent<FactoryContainer>();
                if (fObj != null)
                {
                    Debug.LogWarning("FACTORY CONTAINER COMPONENT FOUND");

                    // that need to be checked to see if passes inspection
                    for (int i = 0; i < fObj.CheckList.Length; i++)
                    {
                        if (fObj.CheckList[i])
                            continue;
                        else
                        {
                            StartCoroutine(RestartFactoryObjectLifeCycle(fObj.gameObject, EFactoryType.FactoryContainer));
                            ScanFailed.Raise();
                            return E_ScanStatus.Fail;
                        }
                    }
                    //print("Success");
                    // If all items of the checklist pass then the scan is successful
                    StartCoroutine(RestartFactoryObjectLifeCycle(fObj.gameObject, EFactoryType.FactoryContainer));
                    ScanPassed.Raise();
                    return E_ScanStatus.Pass;
                }
                else
                    Debug.LogWarning("NO FACTORY CONTAINER FOUND");
                fObj.CanMove = false;
                return E_ScanStatus.Fail;
            default:
                // If there is no FactoryObject Component attached then send an error
                Debug.LogWarning("No FactoryObject Component found in ScanItem");
                fObj.CanMove = false;
                return E_ScanStatus.Fail;
        }
        
    }


    IEnumerator RestartFactoryObjectLifeCycle(GameObject fObj, EFactoryType fType)
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
        if (fType == EFactoryType.FactoryObject)
        {
            Debug.LogWarning("Initializing Factory Object");
            fObj.GetComponent<FactoryObject>().CanMove = false;
            fObj.transform.position = fObj.GetComponent<FactoryObject>().m_spawnPos;
            fObj.GetComponent<FactoryObject>().Initialize();
        }
        else if (fType == EFactoryType.FactoryContainer)
        {
            fObj.GetComponent<FactoryContainer>().CanMove = false;
            fObj.transform.position = fObj.GetComponent<FactoryContainer>().SpawnPos;
            fObj.GetComponent<FactoryContainer>().IsInUse = false;
            fObj.GetComponent<FactoryContainer>().Initialize();
        }

    }
}
