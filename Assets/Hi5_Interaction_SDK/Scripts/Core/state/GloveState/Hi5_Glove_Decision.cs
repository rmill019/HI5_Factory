using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hi5_Interaction_Core
{
	public class Hi5_Glove_Decision 
	{
		Hi5_Object_JudgeMent mJudgeMent;
		private Hi5_Glove_Interaction_Hand mHand = null;
		internal protected Hi5_Glove_Interaction_State mState = null;
		internal Hi5_Glove_Decision (Hi5_Object_JudgeMent judgement,Hi5_Glove_Interaction_Hand hand,Hi5_Glove_Interaction_State state)
		{
			mJudgeMent = judgement;
			mHand = hand;
			mState = state;
		}


		internal bool IsPinch()
		{
			List<int> pinchs;
			int ObjectId = -1;
			bool isPinch = mHand.mState.mJudgeMent.IsPinch(out pinchs, out ObjectId);
			if (isPinch)
			{
				Hi5_Glove_Interaction_Item item = Hi5_Interaction_Object_Manager.GetObjectManager().GetItemById(ObjectId);
				if (item != null 
					&& item.mObjectType == EObject_Type.ECommon
					&& item.mstatemanager != null
					&& item.mstatemanager.GetMoveState() != null
					&& (item.state == E_Object_State.EStatic
						|| item.state == E_Object_State.EPinch
						|| item.state == E_Object_State.EFlyLift
						|| item.state == E_Object_State.EClap
						|| (item.state == E_Object_State.EMove && item.mstatemanager.GetMoveState().mMoveType == Hi5ObjectMoveType.EPlaneMove)))
				{

					Hi5_Interaction_Message.GetInstance().DispenseMessage(Hi5_MessageKey.messagePinchObject, pinchs, mHand, ObjectId);
					//isPinchCollider
					Hi5_Glove_State_Base baseState = mState.GetBaseState(E_Hand_State.EPinch);
					if (baseState != null)
					{
						(baseState as Hi5_Glove_State_Pinch).isPinchCollider = true;
						(baseState as Hi5_Glove_State_Pinch).objectId = ObjectId;
					}

					//Hi5_Glove_Interraction_Item item = Hi5_Interaction_Object_Manager.GetObjectManager().GetItemById(ObjectId);
					if (item != null && item.state == E_Object_State.EMove
						&& (item.mstatemanager.GetMoveState().mMoveType == Hi5ObjectMoveType.EThrowMove || item.mstatemanager.GetMoveState().mMoveType == Hi5ObjectMoveType.EFree)
						&& !item.mstatemanager.GetMoveState().IsProtectionFly())
					{
						bool ContactIsSelf = false;
						float distance = Hi5_Interaction_Const.GetDistance(mHand.mPalm.transform, item, out ContactIsSelf);
						if (ContactIsSelf)
						{
							Vector3 offset = new Vector3(mHand.mPalm.transform.position.x - item.transform.position.x,
								mHand.mPalm.transform.position.y - item.transform.position.y,
								mHand.mPalm.transform.position.z - item.transform.position.z).normalized;
							offset = offset * distance;
							//item.transform.position = new Vector3(item.transform.position.x + offset.x,
							//    item.transform.position.y + offset.y,
							//    item.transform.position.z + offset.z);
						}
					}
					item.CleanRecord();
					Hi5_Glove_Interaction_Hand handTemp = mHand;
					mState.ChangeState(E_Hand_State.EPinch);

//					if (handTemp.m_IsLeftHand) 
//					{
//						Debug.Log ("Left pinch");
//					}
					{
						Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(item.idObject,
							item.mObjectType,
							handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
							EEventObjectType.EPinch);
						Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
					}

					{
						Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance(item.idObject,
							handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
							EEventHandType.EPinch);
						Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
					}
					//Hi5_Glove_Interaction_Object_Event_Data data = new Hi5_Glove_Interaction_Object_Event_Data();
					//if (Hand.m_IsLeftHand)
					//    data.mHandType = EHandType.EHandLeft;
					//else
					//    data.mHandType = EHandType.EHandRight;
					//data.mObjectType = item.mObjectType;
					//data.mEventType = EEventType.EPinch;
					//data.mObjectId = item.idObject;
					//Hi5InteractionManger.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);

					return true;;
				}
				return false;
			}
			return false;
		}


		internal bool IsFlyPinch()
		{
			List<int> pinchs4;
			//isPinch = Hand.mState.mJudgeMent.IsPinch(out pinchs4, out ObjectId);
			//if(!isPinch)
			int ObjectId = -1000;
			bool isPinch = mHand.mState.mJudgeMent.IsFlyIngPinch(out pinchs4, out ObjectId);
			if (isPinch)
			{
				Hi5_Glove_Interaction_Item item = Hi5_Interaction_Object_Manager.GetObjectManager().GetItemById(ObjectId);
				float dis = Mathf.Abs(item.PlaneY - item.transform.position.y);
				if (item != null && item.state == E_Object_State.EMove
					&& (item.mstatemanager.GetMoveState ().mMoveType == Hi5ObjectMoveType.EThrowMove || item.mstatemanager.GetMoveState ().mMoveType == Hi5ObjectMoveType.EFree )
					&& !item.mstatemanager.GetMoveState ().IsProtectionFly () && dis>(0.1f*item.GetHeight()))
				{
					if (item.mObjectType == EObject_Type.ECommon)
					{
						Hi5_Interaction_Message.GetInstance ().DispenseMessage (Hi5_MessageKey.messageFlyPinchObject, pinchs4, mHand, ObjectId);
						Hi5_Glove_State_Base baseState = mState.GetBaseState (E_Hand_State.EPinch);
						if (baseState != null) {
							(baseState as Hi5_Glove_State_Pinch).isPinchCollider = true;
							(baseState as Hi5_Glove_State_Pinch).objectId = ObjectId;
						}
				
						item.transform.position = mHand.GetThumbAndMiddlePoin ();


						item.CleanRecord();
						mState.ChangeState (E_Hand_State.EPinch);
						Hi5_Glove_Interaction_Hand handTemp = mHand;

//						if (handTemp.m_IsLeftHand) 
//						{
//							Debug.Log ("Left pinch");
//						}
						{
							Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance (item.idObject,
								item.mObjectType,
								handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
								EEventObjectType.EPinch);
							Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
						}

						{
							Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance (item.idObject,
								handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
								EEventHandType.EPinch);
							Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
						}
						return true;
					}
				}

			}
			return false;
		}

		internal bool IsPinch2()
		{
			List<int> pinchs2;
			List<Hi5_Glove_Interaction_Finger> fingers = null;
			int ObjectId = -1000;
			bool isPinch2 = mHand.mState.mJudgeMent.IsPinch2(out pinchs2, out fingers, out ObjectId);

			Hi5_Glove_Interaction_Item item = Hi5_Interaction_Object_Manager.GetObjectManager().GetItemById(ObjectId);


			if (item != null 
				&& item.mObjectType == EObject_Type.ECommon
				&& item.mstatemanager != null
				&& item.mstatemanager.GetMoveState() != null
				&& (item.state == E_Object_State.EStatic || item.state == E_Object_State.EPinch || item.state == E_Object_State.EFlyLift || item.state == E_Object_State.EClap
					|| (item.state == E_Object_State.EMove && item.mstatemanager.GetMoveState().mMoveType == Hi5ObjectMoveType.EPlaneMove)
					|| (item.state == E_Object_State.EMove && item.mstatemanager.GetMoveState().mMoveType == Hi5ObjectMoveType.EFree)))
			{ 
				if (isPinch2 && pinchs2 != null && pinchs2.Count > 0 && fingers != null)
				{
					foreach (Hi5_Glove_Interaction_Finger tempItem in fingers)
					{
						tempItem.NotifyEnterPinch2State();
					}
					Hi5_Interaction_Message.GetInstance().DispenseMessage(Hi5_MessageKey.messagePinchObject2, pinchs2, mHand, ObjectId);
					Hi5_Glove_State_Base baseState = mState.GetBaseState(E_Hand_State.EPinch2);
					if (baseState != null)
					{
						(baseState as Hi5_Glove_State_Pinch2).objectId = ObjectId;
					}

					item.CleanRecord();
					Hi5_Glove_Interaction_Hand handTemp = mHand;
					mState.ChangeState(E_Hand_State.EPinch2);
					{
						Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(item.idObject,
							item.mObjectType,
							handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
							EEventObjectType.EPinch);
						Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
					}

					{
						
						Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance(item.idObject,
							handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
							EEventHandType.EPinch);
						Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
					}
					return true;
				}
			}
			return false;
		}


		internal bool IsClap()
		{
			int objectId = 0;
			Hi5_Glove_Interaction_Finger_Type fingTypeOne;
			Hi5_Glove_Interaction_Finger_Type fingTypeTwo;
			bool isClap = mHand.mState.mJudgeMent.IsClap(out objectId, out fingTypeOne,out fingTypeTwo);
			Hi5_Glove_Interaction_Item item = Hi5_Interaction_Object_Manager.GetObjectManager().GetItemById(objectId);
			if (item != null && (item.state == E_Object_State.EStatic || item.state == E_Object_State.EPoke))
			{
				Hi5_Object_State_Clap clapState = item.mstatemanager.GetState(E_Object_State.EClap) as Hi5_Object_State_Clap;
				clapState.fingerTypeOne = fingTypeOne;
				clapState.fingerTypeTwo = fingTypeTwo;
				clapState.hand = mHand;
				if (mHand.m_IsLeftHand)
					clapState.handType = Hi5_Object_State_Clap.Hi5_Object_Clap_Type.ELeft;
				else
					clapState.handType = Hi5_Object_State_Clap.Hi5_Object_Clap_Type.ERight;
				Hi5_Glove_Interaction_Hand handTemp = mHand;
				item.mstatemanager.ChangeState(E_Object_State.EClap);
				if (Hi5_Interaction_Const.TestChangeState)
					mState.ChangeState(E_Hand_State.EClap);
				{
					Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(item.idObject,
						item.mObjectType,
						handTemp.m_IsLeftHand? EHandType.EHandLeft: EHandType.EHandRight,
						EEventObjectType.EClap);
					Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
				}

				{
					Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance(item.idObject,
						handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
						EEventHandType.EClap);
					Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
				}

				return true;
			}
			return false;
		}



		internal bool IsLift()
		{
			int objectId = 0;
			// Hi5_Glove_Interaction_Finger_Type fingType;
			bool islift = mHand.mState.mJudgeMent.IsLift(out objectId);

			Hi5_Glove_Interaction_Item item = Hi5_Interaction_Object_Manager.GetObjectManager().GetItemById(objectId);

			if (item != null
				&& item.mObjectType == EObject_Type.ECommon
				&& (item.state == E_Object_State.EStatic /*|| item.state == E_Object_State.EPinch || item.state == E_Object_State.EFlyLift || item.state == E_Object_State.EClap*/
					|| (item.state == E_Object_State.EMove))
				&& islift)
			{

				Hi5_Glove_State_Lift liftState = mHand.mState.GetBaseState(E_Hand_State.ELift) as Hi5_Glove_State_Lift;
				liftState.objectId = objectId;
				Hi5_Glove_Interaction_Hand handTemp = mHand;
				Hi5_Interaction_Message.GetInstance().DispenseMessage(Hi5_MessageKey.messageLiftObject, mHand, objectId);
											if (Hi5_Interaction_Const.TestChangeState)
												mState.ChangeState(E_Hand_State.ELift);
				{
					Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(item.idObject,
						item.mObjectType,
						handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
						EEventObjectType.ELift);
					Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
				}

				{
					Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance(item.idObject,
						handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
						EEventHandType.ELift);
					Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
				}


				//Hi5_Glove_Interaction_Object_Event_Data data = new Hi5_Glove_Interaction_Object_Event_Data();
				//if (Hand.m_IsLeftHand)
				//    data.mHandType = EHandType.EHandLeft;
				//else
				//    data.mHandType = EHandType.EHandRight;
				//data.mObjectType = item.mObjectType;
				//data.mEventType = EEventType.ELift;
				//data.mObjectId = item.idObject;
				//Hi5InteractionManger.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
				return true;
			}
			return false;
		}


	}
}
