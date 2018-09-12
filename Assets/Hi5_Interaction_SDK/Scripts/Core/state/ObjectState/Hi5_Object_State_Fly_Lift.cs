using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hi5_Interaction_Core
{
    public class Hi5_Object_State_Fly_Lift : Hi5_Object_State_Base
    {
        internal Hi5_Glove_Interaction_Hand hand = null;
        Hi5_Object_Move mMoveObject = null;
		Vector3 offsetDragSpeed = Vector3.zero;
		protected Hi5_Record mRecord = new Hi5_Record();
        internal Hi5_Object_Move Move
        {
            get { return mMoveObject; }
        }

        override internal protected void Init(Hi5_Glove_Interaction_Item itemObject, Hi5_Obiect_State_Manager state)
        {
            mObjectItem = itemObject;
            mState = state;
            mMoveObject = new Hi5_Object_Move(itemObject, state);
            mMoveObject.SetAttribute(mObjectItem.AirFrictionRate, mObjectItem.PlaneFrictionRate);

        }

        override public void Start()
        {
//			if (Hi5_Interaction_Const.TestLift)
//			{
				mObjectItem.SetIsKinematic(false);
				mObjectItem.SetUseGravity(true);
				mObjectItem.SetIsLockYPosition(false);
				mObjectItem.transform.parent = hand.mPalm.transform;
				mObjectItem.transform.rotation = Quaternion.Euler (0.0f,mObjectItem.transform.eulerAngles.y,0.0f);
				hand.mPalm.OpenPhyCollider (true);
				//mObjectItem.ChangeColor (Color.blue);
//			}
//			else
//			{
//				mObjectItem.SetIsKinematic(false);
//				mObjectItem.SetUseGravity(true);
//				mObjectItem.SetIsLockYPosition(false);
//			}
//           
			mRecord.RecordClean ();
			offsetDragSpeed = Vector3.zero;
        }



        override public void Update(float deltaTime)
        {
			
			FollowpalmMove (deltaTime);


			mRecord.RecordPosition(Time.deltaTime, mObjectItem.transform);

//			if (!Hi5_Interaction_Const.TestLift) {
//				if (hand != null && hand.mPalm != null) {
//					float distance = Vector3.Distance (hand.mPalm.transform.position, mObjectItem.transform.position);
//					if (distance > Hi5_Interaction_Const.liftChangeMoveDistance) {
//						Transform temp = hand.mPalm.transform;
//						if (mObjectItem.mObjectType == EObject_Type.ECommon) {
//							Hi5_Glove_Interaction_Hand handtemp = hand;
//							mObjectItem.ChangeState (E_Object_State.EMove);
//							//if(mObjectItem.mstatemanager != null && mObjectItem.mstatemanager.GetMoveState() != null)
//							mObjectItem.mstatemanager.GetMoveState ().SetFreeMove (temp);
//							{
//								Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance (ObjectItem.idObject,
//									                                               ObjectItem.mObjectType,
//									handtemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
//									                                               EEventObjectType.EMove);
//								Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
//							}
//
//							{
//								Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance (ObjectItem.idObject,
//									handtemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
//									                                             EEventHandType.ERelease);
//								Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
//							}
//
//
//						}
//
//					}
//				}
//
//			}
//			else
//			{
				bool IsRelease = false;
				float distance = Vector3.Distance (hand.mPalm.transform.position, mObjectItem.transform.position);
				if (distance > Hi5_Interaction_Const.liftChangeMoveDistance) {
					IsRelease = true;
					//Debug.Log("distance release");
				}
				
				float angle = Vector3.Angle(Hi5_Interaction_Object_Manager.GetObjectManager().transform.up, -hand.mPalm.transform.up);
				//Debug.Log("IsLift angle" + angle);
				if (!IsRelease &&  angle > 40.0f && angle > 0.0f) {
					IsRelease = true;
					//Debug.Log("angle release");
				}
				if (!IsRelease && !mObjectItem.IsLiftTrigger()) {
					IsRelease = true;
					
				}
				if(IsRelease)
				{
					Transform temp = hand.mPalm.transform;
					ObjectItem.transform.parent = Hi5_Interaction_Object_Manager.GetObjectManager ().transform;
					if (mObjectItem.mObjectType == EObject_Type.ECommon)
					{
						Hi5_Glove_Interaction_Hand handTemp = hand;
						mObjectItem.ChangeState (E_Object_State.EMove);
						//if(mObjectItem.mstatemanager != null && mObjectItem.mstatemanager.GetMoveState() != null)
						mObjectItem.mstatemanager.GetMoveState ().SetFreeMove (temp);
						
						{
							Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance (ObjectItem.idObject,
								ObjectItem.mObjectType,
								handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
								EEventObjectType.EMove);
							Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
						}

						{
							Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance (ObjectItem.idObject,
								handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
								EEventHandType.ERelease);
							Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
						}
					}
				}
			//}
        }


		private void FollowpalmMove(float deltime)
		{
			if (hand != null && hand.mVisibleHand != null && hand.mVisibleHand.palmMove != null)
			{	
				Queue<Hi5_Position_Record>  records =  mRecord.GetRecord();

				//Queue<Hi5_Position_Record>  records =  hand.mVisibleHand.palmMove.GetRecord();
				if (records != null)
				{
					Hi5_Position_Record[] recordArray = records.ToArray ();
//					if (!Hi5_Interaction_Const.TestLift) 
//					{
//						//计算摩擦力产生的位移
//						if (recordArray.Length > 2)
//						{
//							Hi5_Position_Record temp1 = recordArray [recordArray.Length - 2];
//							Hi5_Position_Record temp2 = recordArray [recordArray.Length - 1];
//
//							Vector3 power = Hi5_PhySics_Caculate_Utilities.CaculateForce(ObjectItem.GetMass (),temp1.mMoveVector,temp1.mIntervalTime,temp2.mMoveVector,temp2.mIntervalTime); 
//							//Debug.Log ("power ="+power);
//							Vector3 drag = 0.5f*power;
//							//Debug.Log ("drag ="+drag);
//							Vector3 accelerationDrag =  -Hi5_PhySics_Caculate_Utilities.CaculateAccelerationByForce (ObjectItem.GetMass (), drag);
//							//Debug.Log ("accelerationDrag ="+accelerationDrag);
//							offsetDragSpeed += accelerationDrag * deltime;
//							//Debug.Log ("offsetDragSpeed ="+offsetDragSpeed);
//							if (!float.IsNaN (offsetDragSpeed.x)  && !float.IsNaN (offsetDragSpeed.z)) {
//								ObjectItem.transform.Translate (new Vector3 (offsetDragSpeed.x, 0.0f, offsetDragSpeed.z), Space.World);
//							}
//							//Vector3 offsetVector3 = records[]
//
//
//						}
//					}

//					if (!Hi5_Interaction_Const.TestLift) {
//						Hi5_Position_Record[] recordPalm = hand.mVisibleHand.palmMove.GetRecord ().ToArray ();
//						if (recordPalm.Length > 1) {
//							Hi5_Position_Record temp = recordPalm [recordPalm.Length - 1];
//							Vector3 offset = new Vector3 (temp.mMoveVector.x, temp.mMoveVector.y, temp.mMoveVector.z);
//							if (!float.IsNaN (offset.x) && !float.IsNaN (offset.z)) {
//								ObjectItem.transform.Translate (offset, Space.World);
//							}
//						}
//					}
//					else
//					{
////						Hi5_Record record = hand.mVisibleHand.palmMove.GetHi5Record ();
////						Vector3 offset =  hand.mVisibleHand.palmMove.transform.position - record.prePosiotnRecord;
////						ObjectItem.transform.Translate (offset, Space.World);
//					}
					//计算随手掌移动位移

//					if (!Hi5_Interaction_Const.TestLift) {
//						//追手的位置
//						{
//							Vector3 palmPosition = new Vector3 (hand.mVisibleHand.palmMove.transform.position.x, 0, hand.mVisibleHand.palmMove.transform.position.z);
//							Vector3 ObjectItemPosition = new Vector3 (ObjectItem.transform.position.x, 0, ObjectItem.transform.position.z);
//
//							float distance = Vector3.Distance (palmPosition, ObjectItemPosition);
//							if (distance > 0.0005f) {
//								Vector3 speed = new Vector3 (palmPosition.x - ObjectItemPosition.x, 0, palmPosition.z - ObjectItemPosition.z) * 0.05f;
//								ObjectItem.transform.Translate (speed, Space.World);
//							}
//						}
//					}
				}
			}
			//if(hand.mVisibleHand.palmMove)
		}

        override public void End()
        {
//			if (Hi5_Interaction_Const.TestLift) 
//			{
//				if(ObjectItem.state != E_Object_State.EPinch)
//					ObjectItem.transform.parent = Hi5_Interaction_Object_Manager.GetObjectManager ().transform;
//			}
			///mObjectItem.ChangeColor (mObjectItem.orgColor);
			if (Hi5_Interaction_Const.TestChangeState)
				hand.mState.ChangeState(E_Hand_State.ERelease);
			hand.mPalm.OpenPhyCollider (false);
			hand = null;
			mRecord.RecordClean ();
        }

        public override void FixedUpdate(float deltaTime)
        {

        }

    }
}
