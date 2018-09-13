using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hi5_Interaction_Core;

public enum EFactoryType { FactoryObject, FactoryContainer }
public enum EAxis { X, Y, Z }

// This class is a singleton
public class GameManager : MonoBehaviour
{
    public static GameManager S;
    public int m_fObjStartIndex = 0;
    public int m_fConStartIndex = 50;
    public float spawnPosInterval = .25f;

    public GameObject m_interactionManager;
    public GameObject m_factoryObjectPrefab;
    public int m_maxNumberOfObjects = 100;

    public List<GameObject> m_pooledFactoryObjects;
    public List<GameObject> m_pooledFactoryContainers;

    // This is the first place where we store the objects off-screen
    Vector3 factoryObjSpawnLoc = Vector3.zero;
    Vector3 containerObjSpawnLoc = new Vector3(0, 0, 10f);

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(S.gameObject);
        
    }

    private void OnEnable()
    {
        OrganizeFactoryComponent(m_pooledFactoryContainers, EFactoryType.FactoryContainer, EAxis.Z);
        OrganizeFactoryComponent(m_pooledFactoryObjects, EFactoryType.FactoryObject, EAxis.Y);
    }

    public GameObject GrabUnusedFactoryObject ()
    {
        foreach (GameObject go in m_pooledFactoryObjects)
        {
            FactoryObject fObj = go.GetComponent<FactoryObject>();
            // If our GameObject has a FactoryObject Component and is not in use then return it
            if (fObj && !fObj.IsInUse)
                return go;
        }
        // All objects are in use, return null
        return null;
    }

    public GameObject GrabUnusedFactoryContainer()
    {
        foreach (GameObject go in m_pooledFactoryContainers)
        {
            FactoryContainer fCon = go.GetComponent<FactoryContainer>();
            // If our GameObject has a FactoryObject Component and is not in use then return it
            if (fCon && !fCon.IsInUse)
                return go;
        }
        // All objects are in use, return null
        return null;
    }

    public void OrganizeFactoryComponent (List<GameObject> listToOrganize, EFactoryType type, EAxis axis)
    {
        int index = 0;
        if (type == EFactoryType.FactoryObject)
            index = m_fObjStartIndex;
        else if (type == EFactoryType.FactoryContainer)
            index = m_fConStartIndex;

        foreach (GameObject go in listToOrganize)
        {
            //go.GetComponent<Hi5_Glove_Interaction_Item>().enabled = false;
  
            index++;
            if (type == EFactoryType.FactoryObject)
            {
                go.name = "FactoryObject";
                go.transform.position = factoryObjSpawnLoc;
                switch (axis)
                {
                    case EAxis.X:
                        factoryObjSpawnLoc.x += spawnPosInterval;
                        break;
                    case EAxis.Y:
                        factoryObjSpawnLoc.y += spawnPosInterval;
                        break;
                    case EAxis.Z:
                        factoryObjSpawnLoc.z += spawnPosInterval;
                        break;
                    default:
                        break;
                }
            }
            else if (type == EFactoryType.FactoryContainer)
            {
                go.name = "FactoryContainer";
                go.transform.position = containerObjSpawnLoc;
                switch (axis)
                {
                    case EAxis.X:
                        containerObjSpawnLoc.x += spawnPosInterval;
                        break;
                    case EAxis.Y:
                        containerObjSpawnLoc.y += spawnPosInterval;
                        break;
                    case EAxis.Z:
                        containerObjSpawnLoc.z += spawnPosInterval;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
