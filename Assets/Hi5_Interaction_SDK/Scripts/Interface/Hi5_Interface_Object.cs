using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hi5_Interaction_Core;
namespace Hi5_Interaction_Interface
{
	public class Hi5_Interface_Object : Hi5_Interface_Object_Base
    {

        bool isRegister = false;
        protected void OnEnable()
        {
            Hi5_Glove_Interaction_Item temp = ObjectItem;
			if (Hi5InteractionManager.Instance != null)
            {
				Hi5InteractionManager.Instance.GetMessage().RegisterMessage(MessageFun, Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent);
				isRegister = true; ;
            }
        }

        protected void Update()
        {
			if (Hi5InteractionManager.Instance != null && !isRegister)
            {
				Hi5InteractionManager.Instance.GetMessage().RegisterMessage(MessageFun, Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent);
				isRegister = true; ;
            }

			if (mItem != null && GetObjectItemState == E_Object_State.EStatic)
			{
				mItem.ChangeColor(mItem.orgColor);
			}
        }

        protected void OnDisable()
        {
			if(Hi5InteractionManager.Instance != null && Hi5InteractionManager.Instance.GetMessage() != null)
				Hi5InteractionManager.Instance.GetMessage().UnRegisterMessage(MessageFun, Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent);
			isRegister = false;
        }

        void MessageFun(string messageKey, object param1, object param2)
        {
            
            if (messageKey.CompareTo(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent) == 0)
            {
                Hi5_Glove_Interaction_Object_Event_Data data = param1 as Hi5_Glove_Interaction_Object_Event_Data;
                if (data.mObjectId == ObjectItem.idObject)
                {
                    switch (data.mEventType)
                    {
                        case EEventObjectType.EClap:
                            break;
						case EEventObjectType.EPoke:
                            break;
                            // Steps to take once an Interface Object has entered Pinch State
                        case EEventObjectType.EPinch:
                            FactoryObject fObj = gameObject.GetComponent<FactoryObject>();
                            FactoryContainer fCon = gameObject.GetComponent<FactoryContainer>();
                            if (fObj)
                                fObj.CanMove = false;
                            if (fCon)
                            {
                                fCon.CanMove = false;
                                fCon.TimerActive = false;
                            }
                            break;
                        case EEventObjectType.EMove:
                            break;
                        case EEventObjectType.ELift:
                            break;
                        case EEventObjectType.EStatic:
                            break;
                    }
                }
               
            }
        }

    }
}
