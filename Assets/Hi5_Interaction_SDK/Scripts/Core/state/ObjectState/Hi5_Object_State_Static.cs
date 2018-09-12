using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hi5_Interaction_Core
{
    public class Hi5_Object_State_Static : Hi5_Object_State_Base
    {
        internal Vector3 prePosition = Vector3.zero;
        internal Quaternion preRotaion = Quaternion.identity;
		internal float staticStartY = 0.0f;
        override public void Start()
        {
			staticStartY = ObjectItem.transform.position.y;
		
        }

        internal void ResetPreTransform()
        {
            ObjectItem.transform.position =  new Vector3(ObjectItem.transform.position.x, prePosition.y, ObjectItem.transform.position.z);
            ObjectItem.transform.rotation = preRotaion;
        }

		bool IsTouchPlane()
		{
			if (ObjectItem.trigger != null)
			{
				Vector3 downPosition = new Vector3(ObjectItem.transform.position.x,
													ObjectItem.transform.position.y - 0.2f,
													ObjectItem.transform.position.z);	
				Ray ray = new Ray( ObjectItem.transform.position, ObjectItem.transform.position - downPosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
					if (hit.transform == ObjectItem.trigger.planeTransform) {
						return true;
					} else {
						return false;
					}
				}
				else {
					return true;
				}
			} 
			else
				return true;
		}

		float detachCd = 0.4f;
		bool StartY = false;
		float Y = 0.0f;
        override public void Update(float deltaTime)
        {

			if (Hi5_Interaction_Const.TestChangeState1)
			{

				if (ObjectItem.transform.parent != Hi5_Interaction_Object_Manager.GetObjectManager ().transform) {
					///Debug.Log ("static palm parent");
					ObjectItem.transform.parent = Hi5_Interaction_Object_Manager.GetObjectManager ().transform;
				}
			}

			{

				if (ObjectItem.mObjectType == EObject_Type.ECommon)
				{
					if (((!ObjectItem.IsTouchPlane ()) && (!ObjectItem.IsTouchStaticObject ()))) {
						//Debug.Log ("Move From");
						ObjectItem.SetIsKinematic (false);
						ObjectItem.SetUseGravity (true);
						ObjectItem.SetIsLockYPosition (false);
						if (Hi5_Interaction_Const.TestPlaneStatic)
						{
							if (!StartY) {
								StartY = true;
								Y = ObjectItem.transform.position.y;
							}
							else {
								if (StartY) {
									if (Mathf.Abs (Y - ObjectItem.transform.position.y) > 0.2f) {
										ObjectItem.mstatemanager.GetMoveState ().SetFreeMove (null);
										ObjectItem.ChangeState (E_Object_State.EMove);
										{
											Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance (ObjectItem.idObject,
												ObjectItem.mObjectType,
												EHandType.ENone,
												EEventObjectType.EMove);
											Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
										}
									}
								}
							}
							

						}
						else
						{
							ObjectItem.mstatemanager.GetMoveState ().SetFreeMove (null);
							ObjectItem.ChangeState (E_Object_State.EMove);
							{
								Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance (ObjectItem.idObject,
									ObjectItem.mObjectType,
									EHandType.ENone,
									EEventObjectType.EMove);
								Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
							}
						}
					}
					else
					{
						ObjectItem.SetIsKinematic(false);
						ObjectItem.SetIsLockYPosition(false);
						ObjectItem.SetUseGravity (true);
					}
				}
				else
				{            
					ObjectItem.SetIsKinematic(true);
					ObjectItem.SetIsLockYPosition(true);
					ObjectItem.SetUseGravity (false);
						//ObjectItem.transform.position = new Vector3 (ObjectItem.transform.position.x,ObjectItem.Y,ObjectItem.transform.position.z);
				}

			}


            if (ObjectItem.mObjectType == EObject_Type.EButton)
            {
                ObjectItem.SetIsKinematic(true);
                ObjectItem.SetIsLockYPosition(true);
            }
        }

        override public void End()
        {
            prePosition = ObjectItem.transform.position;
            preRotaion = ObjectItem.transform.rotation;
			StartY = false;
			Y = 0.0f;
        }

        override public  void FixedUpdate(float deltaTime)
        {

        }
    }
}
