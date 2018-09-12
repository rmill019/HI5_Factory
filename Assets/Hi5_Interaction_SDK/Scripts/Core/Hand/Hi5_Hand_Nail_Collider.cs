using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hi5_Interaction_Core
{
    public class Hi5_Hand_Nail_Collider : Hi5_Glove_Interaction_Collider
    {
        internal bool IsNail(int objectId)
        {
            foreach (Collider item in m_Triggers)
            {
                if (item.GetComponent<Hi5_Glove_Interaction_Item>() != null)
                {
                    if (item.GetComponent<Hi5_Glove_Interaction_Item>().idObject == objectId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
