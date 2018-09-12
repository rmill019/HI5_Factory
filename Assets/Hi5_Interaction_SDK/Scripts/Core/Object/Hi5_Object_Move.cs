using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hi5_Interaction_Core
{
    public enum Hi5ObjectMoveType
    {
        ENone = 0,
        EThrowMove = 1,
        EPlaneMove = 2,
        EFree = 3,
    }
    public class Hi5_Object_Move
    {
        
        #region move data
        public class ObjectMoveData
        {
            public Vector3 mDirection;
            public float cd = 0.0f;
            public float y;
            public float ySpeed;
        }
        #endregion

        protected internal Hi5_Glove_Interaction_Item mItem = null;
        protected internal ObjectMoveData mMoveData = null;
        internal float mAirFrictionRate;
        internal float mPlaneFrictionRate;
        Hi5_Obiect_State_Manager mState = null;

        Vector3 contactPointNormal;
        // bool isMove = false;
        internal Hi5ObjectMoveType mMoveType = Hi5ObjectMoveType.ENone;
    
        //飞行中暂停
        bool mIsFlyMovePause = false;
        //float mFlyMoveStartProtectionCd = Hi5_Interaction_Const.FingerPinchPauseProtectionTime;
        bool IsProtectFly = false;
        Transform protecedTransform;
        float mWaitFlyPauseTime = Hi5_Interaction_Const.FingerColliderPinchPauseTime;
        internal Hi5_Object_Move(Hi5_Glove_Interaction_Item objectItem, Hi5_Obiect_State_Manager state)
        {
            mState = state;
            mItem = objectItem;
        }

        internal void SetMoveData(ObjectMoveData data)
        {
            mMoveData = data;
        }

        internal void StopMove()
        {
            //isMove = false;
            mMoveData = null;
            mMoveType = Hi5ObjectMoveType.ENone;
            //if (Hi5_Interaction_Const.TestPlaneMoveUsePhysic)
            {
//                mItem.SetIsKinematic(false);
//                mItem.SetIsLockYPosition(true);
            }
            //else
            //{
            //    mItem.SetIsKinematic(true);
            //    mItem.SetIsLockYPosition(false);
            //}
                
           // mItem.SetUseGravity(true);
            
            //mItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }

        private void FreeFly(float deltaTime)
        {
            //if (Hi5_Interaction_Const.TestPlaneMoveUsePhysic)
            {
                mItem.SetUseGravity(true);
                mItem.SetIsKinematic(false);
                mItem.SetIsLockYPosition(false);

				if (Hi5_Interaction_Const.TestPlaneStatic)
				{
					if (mItem.isTouchPlane) {
						mState.ChangeState (E_Object_State.EStatic);
						if (Hi5_Interaction_Const.TestChangeState)
						{
							Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(mItem.idObject, mItem.mObjectType, EHandType.EHandLeft, EEventObjectType.EStatic);
							Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
						}
					}
					if (mItem.mQueuePositionRecord != null) {
						Hi5_Position_Record[] records = mItem.mQueuePositionRecord.ToArray ();
						if (records != null && records.Length > 0) {
							if (records.Length - 2 > 0) {
								Vector3 offset = records [records.Length - 2].position - records [records.Length - 1].position;
								if (Mathf.Abs (offset.x) < 0.003f && Mathf.Abs (offset.y) < 0.003f&& Mathf.Abs (offset.z) < 0.003f) {
									mState.ChangeState (E_Object_State.EStatic);
									if (Hi5_Interaction_Const.TestChangeState)
									{
										Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(mItem.idObject, mItem.mObjectType, EHandType.EHandLeft, EEventObjectType.EStatic);
										Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
									}
								}
							}
						}
					}
				}
				else
				{
					if (mItem.mQueuePositionRecord != null)
					{
						Hi5_Position_Record[] records = mItem.mQueuePositionRecord.ToArray();
						if (records != null && records.Length > 0)
						{
							if (records.Length - 2 > 0)
							{
								Vector3 offset =  records[records.Length - 2].position - records[records.Length - 1].position;
								if (Mathf.Abs(offset.x) < 0.003f && Mathf.Abs(offset.z) < 0.003f)
								{
									mState.ChangeState(E_Object_State.EStatic);
									if (Hi5_Interaction_Const.TestChangeState)
									{
										Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(mItem.idObject, mItem.mObjectType, EHandType.EHandLeft, EEventObjectType.EStatic);
										Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
									}
								}
							}
						}
					}
				}


                //if (mMoveType == Hi5ObjectMoveType.EThrowMove)
                {
                    //if (mIsFlyMovePause)
                    //{
                    //    mWaitFlyPauseTime -= deltaTime;
                    //    if (mWaitFlyPauseTime < 0.0f)
                    //    {
                    //        mIsFlyMovePause = false;
                    //        mItem.SetUseGravity(true);
                    //        mItem.SetIsKinematic(false);
                    //        mWaitFlyPauseTime = Hi5_Interaction_Const.FingerColliderPinchPauseTime;
                    //    }
                    //    else
                    //    {
                    //        return;
                    //    }
                    //}
                }
            }
        }

        internal void Update(float deltaTime)
        {
            if (mMoveType == Hi5ObjectMoveType.EThrowMove)
            {
                ThrowMove(deltaTime);

                if (protecedTransform == null)
                {
                    if (IsProtectFly)
                        IsProtectFly = false;
                }
                else
                {
                    float distance = Vector3.Distance(protecedTransform.position, mItem.transform.position);
                    bool ContactIsSelf = true;
                    // float distance = Hi5_Interaction_Const.GetDistance(protecedTransform, mItem, out ContactIsSelf);
                    if (!ContactIsSelf)
                        IsProtectFly = false;
                    else
                    {
                        if (distance < Hi5_Interaction_Const.ThrowObjectProtecteionDistance)
                        {
                            IsProtectFly = true;
                        }
                        else
                            IsProtectFly = false;
                    }
                }
                //float distance = Vector3.Distance(hand.mPalm.transform.position, mObjectItem.transform.position);
                //if (distance < Hi5_Interaction_Const.liftChangeMoveDistance)
                //{


            }
            
            PlaneMove(deltaTime);
            if (mMoveType == Hi5ObjectMoveType.EFree)
            {
                FreeFly(deltaTime);
                if (protecedTransform == null)
                {
                    if (IsProtectFly)
                        IsProtectFly = false;
                }
                else
                {
                    float distance = Vector3.Distance(protecedTransform.position, mItem.transform.position);
                    bool ContactIsSelf = true;
                    if (!ContactIsSelf)
                        IsProtectFly = false;
                    else
                    {
                        if (distance < Hi5_Interaction_Const.ThrowObjectProtecteionDistance)
                        {
                            IsProtectFly = true;
                        }
                        else
                            IsProtectFly = false;
                    }
                   
                    
                }
            }
        }


        

        internal bool IsProtectionFly()
        {
            if (mMoveType == Hi5ObjectMoveType.EFree || mMoveType == Hi5ObjectMoveType.EThrowMove)
            {
                return IsProtectFly;
            }
            else
                return false;
        }

        internal void FixUpdate(float deltaTime)
        {
            
            
        }
        internal void SetFreeMove(Transform preted)
        {
            protecedTransform = preted;
            mMoveType = Hi5ObjectMoveType.EFree;
            IsProtectFly = true ;
        }

        internal void SetPlaneMove(Collision collision)
        {
			
            if (mMoveType == Hi5ObjectMoveType.EThrowMove || mMoveType == Hi5ObjectMoveType.EFree)
                return;
            //if (Hi5_Interaction_Const.TestPlaneMoveUsePhysic)
            {
                mMoveType = Hi5ObjectMoveType.EPlaneMove;
                return;
            }
              
            Queue<Hi5_Position_Record> records = null;
            if (collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger>() != null)
            {
                records = collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger>().mQueuePositionRecord;
            }
            if (collision.gameObject.GetComponent<Hi5_Hand_Palm_Move>() != null)
            {
                records = collision.gameObject.GetComponent<Hi5_Hand_Palm_Move>().GetRecord();
            }
            if (records != null && records.Count > 0)
            {
                Vector3 distanceVector = Vector3.zero;
                int index = 0;
                int weightPointCount = 0;
                float timeCount = 0.0f;

                foreach (Hi5_Position_Record item in records)
                {
                    if (Hi5_Interaction_Const.RecordPositionWeight.Length > index)
                    {
                        int weight = Hi5_Interaction_Const.RecordPositionWeight[index];
                        weightPointCount += weight;
                        timeCount += item.mIntervalTime * weight;
                        distanceVector += item.mMoveVector * weight;
                    }
                    index++;
                }
                mMoveData = new ObjectMoveData();
                mMoveData.mDirection = distanceVector / timeCount;
                mMoveData.y = mMoveData.mDirection.y;
                mMoveData.ySpeed = mMoveData.mDirection.y;
                mMoveType = Hi5ObjectMoveType.EPlaneMove;
                
                contactPointNormal = collision.contacts[0].normal;
                contactPointNormal.y = 0.0f;
                mItem.SetIsLockYPosition(true);
                //mFlyMoveStartProtectionCd = Hi5_Interaction_Const.FingerPinchPauseProtectionTime;
            }
        }

        private void ThrowMove(float deltaTime)
        {
            if (mMoveType == Hi5ObjectMoveType.EThrowMove && mMoveData != null )
            {
//				if (Hi5_Interaction_Const.TestFlyMoveUsedGravity) {
//					mItem.SetUseGravity (true);
//					mItem.SetIsKinematic (false);
//					mItem.SetIsLockYPosition (false);
//
//					mMoveData.cd += deltaTime;
//					float gravity = Physics.gravity.y;
//					//float gravity = Physics.gravity.y*4.7f/9.8f;
//					float y = 0;
//					{
//						//y = (mMoveData.ySpeed - gravity * deltaTime)* (1.0f - mAirFrictionRate);
//						y = (mMoveData.ySpeed- (1-4.7f/9.8f)*gravity * deltaTime)* (1.0f - mAirFrictionRate);
//
//						mMoveData.ySpeed = y;
//					}
//				
//					mMoveData.mDirection *= (1.0f - mAirFrictionRate+mItem.GetDrag());
//					Vector3 move = new Vector3(mMoveData.mDirection.x, y, mMoveData.mDirection.z);
//
//					if (!float.IsNaN(move.x) && !float.IsNaN(move.y) && !float.IsNaN(move.z))
//					{
//						if (deltaTime > Hi5_Interaction_Const.PRECISION)
//							mItem.transform.Translate(move * deltaTime, Space.World);
//					}
//				}
//				else
//				{
					mMoveData.cd += deltaTime;
					float gravity = Physics.gravity.y*8.1f/9.8f;
					//float gravity = Physics.gravity.y;
					float y = 0;
					y = (mMoveData.ySpeed + gravity * deltaTime)* (1.0f - mAirFrictionRate);
					mMoveData.ySpeed = y;
					
					mMoveData.mDirection *= (1.0f - mAirFrictionRate);
					Vector3 move = new Vector3(mMoveData.mDirection.x, y, mMoveData.mDirection.z);
					if (!float.IsNaN(move.x) && !float.IsNaN(move.y) && !float.IsNaN(move.z))
					{
						if (deltaTime > Hi5_Interaction_Const.PRECISION)
							mItem.transform.Translate(move * deltaTime, Space.World);
					}
				//}      
            }
            
        }

        private void PlaneMove(float deltaTime)
        {
            //if (!mItem.isTouchPlane)
            //{
            //    //Debug.Log("SetPlaneMove false isTouchPlane");
            //    //mItem.GetComponent<Rigidbody>().useGravity = true;
            //    //mItem.GetComponent<Rigidbody>().isKinematic = false;
            //    mItem.SetUseGravity(true);
            //    mItem.SetIsKinematic(false);
            //    mState.ChangeState(E_Object_State.EStatic);
            //    return;
            //}
            if (/*Hi5_Interaction_Const.TestPlaneMoveUsePhysic &&*/ mMoveType == Hi5ObjectMoveType.EPlaneMove)
            {
                mItem.SetIsKinematic(false);
                mItem.SetUseGravity(true);
                if (mItem.isTouchPlane)
                {
                    mItem.SetIsLockYPosition(true);
                }
                else
                {
                    mItem.SetIsLockYPosition(false);
                }
                if (mItem.mQueuePositionRecord != null)
                {
                    Hi5_Position_Record[] records = mItem.mQueuePositionRecord.ToArray();
                    if (records != null && records.Length > 0)
                    {
                        if (records.Length - 2 > 0)
                        {
                            Vector3 offset =  records[records.Length - 2].position - records[records.Length - 1].position;
                            if (Mathf.Abs(offset.x) < 0.003f && Mathf.Abs(offset.z) < 0.003f)
                            {
                                mState.ChangeState(E_Object_State.EStatic);
                            }
                        }
                    }
                }

                //if (Mathf.Abs(contactMove.x) < 0.003f && Mathf.Abs(contactMove.z) < 0.003f)
                //{
                //    mItem.SetIsLockYPosition(false);
                //    mState.ChangeState(E_Object_State.EStatic);
                //} 

                return;
            }
            return;
//            if (mMoveType == Hi5ObjectMoveType.EPlaneMove && mMoveData != null)
//            {
//                List<Collider> colliders = mItem.GetTriggers();
//                foreach (Collider item in colliders)
//                {
//                    if (item.gameObject.GetComponent<Hi5_Hand_Nail_Collider>() != null)
//                    {
//                        mState.ChangeState(E_Object_State.EStatic);
//                        return;
//                    }
//                }
//                mMoveData.mDirection *= (1.0f - mPlaneFrictionRate);
//                float speed = mMoveData.mDirection.magnitude;
//                Vector3 contactMove = contactPointNormal * speed;
//
//                //Debug.Log("contactMove" + contactMove);
//                if (!mItem.isTouchPlane)
//                {
//                    //Debug.Log("SetIsKinematic false");
//                    mItem.SetIsKinematic(false);
//                }
//                else
//                {
//                   
//                    mItem.SetIsKinematic(true);
//                }
//                    
//                mMoveData.cd += deltaTime;
//
//                if (!float.IsNaN(contactMove.x) && !float.IsNaN(contactMove.y) && !float.IsNaN(contactMove.z))
//                {
//                    contactMove.y = 0.0f;
//                    //Debug.Log("contactMove.x =" + contactMove.x + "contactMove.y=" + contactMove.y + "contactMove.z=" + contactMove.z);
//                    float yTemp = mItem.transform.position.y;
//                    if (deltaTime > Hi5_Interaction_Const.PRECISION)
//                    {
//                        mItem.transform.Translate(contactMove * deltaTime, Space.World);
//                        mItem.transform.position = new Vector3(mItem.transform.position.x, yTemp, mItem.transform.position.z);
//                    }
//                }
//                if (Mathf.Abs(contactMove.x) < 0.003f && Mathf.Abs(contactMove.z) < 0.003f)
//                {
//                    mItem.SetIsLockYPosition(false);
//                    mState.ChangeState(E_Object_State.EStatic);
//                }
//                if (!mItem.isTouchPlane)
//                {
//                    mItem.SetIsLockYPosition(false);
//                }
//                else
//                {
//                    mItem.SetIsLockYPosition(true);
//                }
//            }
        }
      
        internal void SetAttribute(float AirFrictionRate,
                                   float PlaneFrictionRate)
        {
            mAirFrictionRate = AirFrictionRate;
            mPlaneFrictionRate = PlaneFrictionRate;
           // mMass = Mass;
        }

        internal void SetFlyPause()
        {
            //if (Hi5_Interaction_Const.TestFlyMoveNoUsedGravity)
            //{
            //    if (mMoveType == Hi5ObjectMoveType.EFree || mMoveType == Hi5ObjectMoveType.EThrowMove)
            //    {
            //        if (!mIsFlyMovePause && mFlyMoveStartProtectionCd < 0.0f)
            //        {
            //            mIsFlyMovePause = true;
            //            mItem.SetUseGravity(false);
            //            mItem.SetIsKinematic(true);
            //            mWaitFlyPauseTime = Hi5_Interaction_Const.FingerColliderPinchPauseTime;
            //        }
            //    }
                   
            //}
              
        }

		internal void CacullateThrowMove(Queue<Hi5_Position_Record> records, Transform handPalm,Hi5_Glove_Interaction_Hand hand)
        {
            mIsFlyMovePause = false;
            int index = 0;
            int weightPointCount = 0;
            float timeCount = 0.0f;
            Vector3 distanceVector = Vector3.zero;

            foreach (Hi5_Position_Record item in records)
            {
                if (Hi5_Interaction_Const.RecordPositionWeight.Length > index)
                {
                    int weight = Hi5_Interaction_Const.RecordPositionWeight[index];
                    weightPointCount += weight;
                    timeCount += item.mIntervalTime * weight;
                    distanceVector += item.mMoveVector * weight;
                }
                index++;
            }
			if (index <= 1) {
//				mMoveData = new ObjectMoveData();
//				mMoveData.mDirection = new Vector3 (0.0f, 0.08598139f, 0.0f);
//				mMoveData.y = mMoveData.mDirection.y;
//				mMoveData.ySpeed = mMoveData.mDirection.y;

				Vector3 temp = hand.MoveAnchor.position - hand.mPalm.transform.position;
				temp.Normalize ();
				mMoveData = new ObjectMoveData();
				mMoveData.mDirection = temp*0.3998139f;
				mMoveData.y = mMoveData.mDirection.y;
				mMoveData.ySpeed = mMoveData.mDirection.y;

//				mMoveData = new ObjectMoveData();
//				mMoveData.mDirection = distanceVector / timeCount* Hi5_Interaction_Const.ThrowSpeed;
//				mMoveData.y = mMoveData.mDirection.y;
//				mMoveData.ySpeed = mMoveData.mDirection.y;

				Hi5_Interaction_Const.WriteItemMoveXml (records,mMoveData);
			}
			else 
			{
				mMoveData = new ObjectMoveData();
				mMoveData.mDirection = distanceVector / timeCount* Hi5_Interaction_Const.ThrowSpeed;
				mMoveData.y = mMoveData.mDirection.y;
				mMoveData.ySpeed = mMoveData.mDirection.y;
				Hi5_Interaction_Const.WriteItemMoveXml (records,mMoveData);
			}


            
            //if (Hi5_Interaction_Const.TestFlyMoveNoUsedGravity)
            {
                mItem.SetIsKinematic(true);
                mItem.SetUseGravity(false);
            }
            //else
            //{
            //    mItem.SetIsKinematic(false);
            //    mItem.SetUseGravity(true);
            //}
               

            mMoveType = Hi5ObjectMoveType.EThrowMove;
            protecedTransform = handPalm;
            IsProtectFly = true;
        }
    }
}

