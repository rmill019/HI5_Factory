using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hi5_Interaction_Core;

// This class is a singleton
public class GameManager : MonoBehaviour
{
    public static GameManager S;

    public GameObject m_interactionManager;
    public GameObject m_factoryObjectPrefab;
    public int m_maxNumberOfObjects = 100;

    public List<GameObject> m_pooledFactoryObjects;

    // This is the first place where we store the objects off-screen
    Vector3 startSpawnLoc = Vector3.zero;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(S.gameObject);

        //// Created as many objects as desired
        //for (int i = 0; i < m_maxNumberOfObjects; i++)
        //{
        //    GameObject tempGO = Instantiate(m_factoryObjectPrefab);
        //    tempGO.transform.position = startSpawnLoc;
        //    tempGO.GetComponent<FactoryObject>().SpawnPos = startSpawnLoc;
        //    // Name needs to stay the same for parent and child
        //    tempGO.name = "FactoryObjectPrefab";
        //    tempGO.transform.parent = m_interactionManager.transform;
        //    // Set the unique id
        //    tempGO.GetComponent<Hi5_Glove_Interaction_Item>().idObject = i;
        //    m_pooledFactoryObjects.Add(tempGO);
        //    // Increment the Y-Pos of startSpawnLoc so that our objects don't overlap
        //    startSpawnLoc.y += 1f;
        //}
        
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

	public GameObject GrabFactoryObjectFromPool ()
    {
        foreach (GameObject go in m_pooledFactoryObjects)
        {
            if (!go.activeInHierarchy)
                return go;
        }
        // No suitable objects found in the List, log message and return null
        Debug.LogWarning("No unactive object found in object pool");
        return null;
    }

    public void CreateObjectPool (int amount, List<GameObject> list)
    {
        // Initialize the object pool
        for (int i = 0; i < amount; i++)
        {
            GameObject tempGO = Instantiate(m_factoryObjectPrefab) as GameObject;
            // Name needs to stay the same for parent and child
            tempGO.transform.GetChild(0).name = tempGO.name;
            tempGO.transform.parent = m_interactionManager.transform;
            // Set the unique id
            tempGO.GetComponent<Hi5_Glove_Interaction_Item>().idObject = i;
            tempGO.SetActive(false);
            list.Add(tempGO);
        }
    }
}
