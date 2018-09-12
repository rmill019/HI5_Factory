using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hi5_Interaction_Core
{
    public class Hi5_Hand_Palm : Hi5_Glove_Interaction_Collider
    {
        internal Hi5_Hand_Visible_Hand mHand;
        override protected void OnTriggerEnter(Collider other)
        {
            //if (other.gameObject.GetComponent<Hi5_Glove_Interraction_Item>())
            //{
            //    other.gameObject.GetComponent<Hi5_Glove_Interraction_Item>().ChangeState(E_Object_State.EStatic);
            //}
        }
    }
}
