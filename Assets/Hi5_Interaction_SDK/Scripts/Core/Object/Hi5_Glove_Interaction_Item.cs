using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hi5_Interaction_Core
{
    public struct Hi5_Position_Record
    {
        public Vector3 mMoveVector;
        public float mIntervalTime;
        public Vector3 position;
        public Hi5_Position_Record(Vector3 point, Vector3 prePoint, float cd)
        {
            mMoveVector = point - prePoint;
            mIntervalTime = cd;
            position = point;
        }
    }

	public enum E_Hand_State
	{
		ERelease = -1,
		EPinch = 2,
		EPinch2 = 3,
		ELift = 4,
		EClap = 5,
	}
    public class Hi5_Glove_Interaction_Item : Hi5_Glove_Interaction_Collider
    {
		class ContactPointClass
		{
			public ContactPoint contactPoint; 	
		}

		internal float  PlaneY = 0.0f;
		bool IsPokeInLoop = false; 
		float IsPokeProtectionCd = Hi5_Interaction_Const.PokeProtectionCd;

		Dictionary<Hi5_Glove_Interaction_Finger_Type, int> mRecordLeftPoke = new Dictionary<Hi5_Glove_Interaction_Finger_Type, int> ();
		Dictionary<Hi5_Glove_Interaction_Finger_Type, int> mRecordRightPoke = new Dictionary<Hi5_Glove_Interaction_Finger_Type, int> ();
        #region object Attribute
        public EObject_Type mObjectType = EObject_Type.ECommon;
        public string nameObject;
        public int idObject;
        #endregion

        #region object MoveAttribute
        public float AirFrictionRate;
        //public float Gravity;
		internal float PlaneFrictionRate = Hi5_Interaction_Const.PlaneFrictionRateConst;

        //public float Mass;
        #endregion 

        internal Hi5_Obiect_State_Manager mstatemanager = null;
        internal Hi5_Glove_Interaction_Item_Trigger trigger = null;
        Quaternion initRotation;
        //internal float initY = -10.0f;
        internal bool isTouchPlane = false;
        internal  Queue<Hi5_Position_Record> mQueuePositionRecord = new Queue<Hi5_Position_Record>();
        Vector3 prePosiotnRecord;
        public E_Object_State state = E_Object_State.EStatic;
        public Hi5ObjectMoveType moveType = Hi5ObjectMoveType.ENone;
        internal Color orgColor;
        internal Vector3 scale;
        

        #region unity system
        override protected void Awake()
        {
            initRotation = transform.rotation;
            base.Awake();
            mstatemanager = Hi5_Obiect_State_Manager.CreateState(this);
            isTouchPlane = false;
            mQueuePositionRecord.Clear();
            prePosiotnRecord = transform.position;
			//Y = transform.position.y;
            orgColor = GetComponent<MeshRenderer>().material.color;
            scale = transform.localScale;
        }

		internal void CleanRecord()
		{
			mQueuePositionRecord.Clear();
		}

        internal void ChangeColor(Color color)
        {
            GetComponent<MeshRenderer>().material.color = color;
        }


		internal bool IsTouchPlane()
		{
			if (trigger == null)
				return true;
			
			return trigger.IsTrigger;;
		}
		internal void LateUpdate ()
		{
			IsPokeInLoop = false;
		}
		internal void Update()
        {
			if (IsPokeInLoop) 
			{
				
				IsPokeProtectionCd -= Time.deltaTime;
				if (IsPokeProtectionCd < 0.0f)
					IsPokeInLoop = false;
			}
			else
			{
				IsPokeProtectionCd = Hi5_Interaction_Const.PokeProtectionCd;
			}

            //transform.localScale = scale;
			if (mstatemanager != null) {
				state = mstatemanager.State;
			}

            


			if (mstatemanager != null && mstatemanager.GetMoveState() != null)
                moveType = mstatemanager.GetMoveState().mMoveType;
            trigger = GetComponentInChildren<Hi5_Glove_Interaction_Item_Trigger>();


            if (trigger != null)
            {
                trigger.itemObject = this;
                isTouchPlane = trigger.IsTrigger;
				//trigger.UpdateOther (Time.deltaTime);
            }
				   
            if (mstatemanager != null)
            {
                mstatemanager.Update(Time.deltaTime);
            }
        }



		//internal float Y = 0.0f; 
		internal void FixedUpdate()
        {
            RecordPosition(Time.deltaTime);
            if (mstatemanager != null)
            {
                mstatemanager.FixedUpdate(Time.deltaTime);
            }

			/*if (contactPointTemp != null) {
				if (!isResetContactPoint) {
					isResetContactPoint = true;
					return;
				}

				float separation = contactPointTemp.contactPoint.separation;
				Vector3 contactPointNormal =contactPointTemp.contactPoint.normal;
				contactPointNormal.Normalize();

				Vector3 separationVector = (contactPointNormal) * separation;
				transform.position = new Vector3(transform.position.x, transform.position.y+Mathf.Abs(separationVector.y), transform.position.z);
				Debug.Log ("contactPointTemp position Y"+transform.position.y);
				Y = transform.position.y;
				contactPointTemp = null;
				isResetContactPoint = false;
			}*/
            // Debug.Log("FixedUpdate isTouchPlane false");
            //isTouchPlane = false;
        }


        Vector3 originalPosition;
        Quaternion originalRotation;
        protected void OnEnable()
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            Hi5_Interaction_Message.GetInstance().RegisterMessage(Reset, Hi5_MessageKey.messageObjectReset);
        }

        protected void OnDisable()
        {
            Hi5_Interaction_Message.GetInstance().UnRegisterMessage(Reset, Hi5_MessageKey.messageObjectReset);
        }
        #endregion

        private void Reset(string messageKey, object param1, object param2, object param3, object param4)
        {
			if (mObjectType == EObject_Type.ECommon) 
			{
				if (messageKey.CompareTo(Hi5_MessageKey.messageObjectReset) == 0)
				{
					transform.parent = Hi5_Interaction_Object_Manager.GetObjectManager().transform;
					transform.position =  originalPosition;
					transform.rotation = originalRotation;
					mstatemanager.ChangeState(E_Object_State.EStatic);
					if (Hi5_Interaction_Const.TestChangeState)
					{
						Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject, mObjectType, EHandType.EHandLeft, EEventObjectType.EStatic);
						Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
					}
					SetIsKinematic(false);
					SetIsLockYPosition(true);
					SetUseGravity(true);
				}

			}
           
       }

        internal void ChangeState(E_Object_State state)
        {
            if (state == E_Object_State.EMove || state == E_Object_State.EFlyLift || state == E_Object_State.EPinch)
            {
                if (mObjectType == EObject_Type.EButton)
                    return;
            }
            mstatemanager.ChangeState(state);
        }

		internal void CacullateThrowMove(Transform handPalm,Hi5_Glove_Interaction_Hand hand)
        {
			mstatemanager.CacullateThrowMove(mQueuePositionRecord, handPalm,hand);
            
        }


        private void RecordPosition(float deltaTime)
        {
            if (mQueuePositionRecord.Count > (Hi5_Interaction_Const.ObjectPinchRecordPostionCount - 1))
            {
                mQueuePositionRecord.Dequeue();
            }
            Hi5_Position_Record record = new Hi5_Position_Record(transform.position, prePosiotnRecord, deltaTime);
            mQueuePositionRecord.Enqueue(record);
            prePosiotnRecord = transform.position;
        }

        internal void OnItemTriggerEnter(Collider collision)
        {
			if (collision.gameObject.layer == Hi5_Interaction_Const.PlaneLayer()  )
            {
                if (mObjectType == EObject_Type.ECommon)
                {
					//Debug.Log ("OnItemTriggerEnter collision"+transform.eulerAngles.y);
                    mstatemanager.StopThrowMove();
					transform.rotation = Quaternion.Euler(0.0f, transform.eulerAngles.y, 0.0f);
					if (Hi5_Interaction_Const.TestChangeState)
					{
						Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject, mObjectType, EHandType.EHandLeft, EEventObjectType.EStatic);
						Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
					}
					//transform.position = new Vector3(transform.position.x, Y-0.001f, transform.position.z);
					//Debug.Log ("transform.rotation x"+ transform.rotation.x+" y"+ transform.rotation.y+"transform.rotation z"+ transform.rotation.z);
                }
            }

			if (collision.gameObject.layer == Hi5_Interaction_Const.ObjectGrasp() 
				&& collision.gameObject.GetComponent<Hi5_Glove_Interaction_Item>() != null
				&& collision.gameObject.GetComponent<Hi5_Glove_Interaction_Item>().state == E_Object_State.EStatic)
			{
				if (mObjectType == EObject_Type.ECommon)
				{
					//Debug.Log ("OnItemTriggerEnter collision"+transform.eulerAngles.y);
					mstatemanager.StopThrowMove();
					transform.rotation = Quaternion.Euler(0.0f, transform.eulerAngles.y, 0.0f);
					if (Hi5_Interaction_Const.TestChangeState)
					{
						Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject, mObjectType, EHandType.EHandLeft, EEventObjectType.EStatic);
						Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
					}
					//transform.position = new Vector3(transform.position.x, Y-0.001f, transform.position.z);
					//Debug.Log ("transform.rotation x"+ transform.rotation.x+" y"+ transform.rotation.y+"transform.rotation z"+ transform.rotation.z);
				}
			}
        }



		ContactPointClass contactPointTemp = null;
		//bool isColliderStaticObjectGrasp = false;

		internal bool IsTouchStaticObject()
		{
			if (trigger != null && trigger.IsTiggerObject) 
			{
				Hi5_Glove_Interaction_Item item = Hi5_Interaction_Object_Manager.GetObjectManager ().GetItemById (trigger.mTiggerObjectId);
				if (item != null) {
					if (item.state == E_Object_State.EStatic) {
						return true;
					} else {
						//Debug.Log ("1");
						return false;
					}
						
				} else {
					//Debug.Log ("2");
					return false;
				}
			}
			else
			{
				//Debug.Log ("3");
				return false;
			}
				
		}
        override protected void OnCollisionEnter(Collision collision)
        {
			UnityEngine.Profiling.Profiler.BeginSample("Hi5_Glove_Interaction_Item");
            base.OnCollisionEnter(collision);
            if (mObjectType == EObject_Type.ECommon)
            {


				if ((collision.gameObject.layer == Hi5_Interaction_Const.PlaneLayer())
					||((collision.gameObject.layer == Hi5_Interaction_Const.ObjectGrasp() 
						&& collision.gameObject.GetComponent<Hi5_Glove_Interaction_Item>() != null
						&& collision.gameObject.GetComponent<Hi5_Glove_Interaction_Item>().state == E_Object_State.EStatic)))
					
                {
					if (state == E_Object_State.EPinch)
						return;

                    mstatemanager.StopThrowMove();
					Vector3 separationVector = Vector3.zero;
					ContactPoint[] contactPoints = collision.contacts;
					if (contactPoints != null && contactPoints.Length > 0) {
						contactPointTemp = new ContactPointClass (); 
						contactPointTemp.contactPoint= contactPoints [0];

						float separation = contactPointTemp.contactPoint.separation;
						Vector3 contactPointNormal =contactPointTemp.contactPoint.normal;
						contactPointNormal.Normalize();

						separationVector = (contactPointNormal) * separation;
						transform.position = new Vector3(transform.position.x, transform.position.y+Mathf.Abs(separationVector.y), transform.position.z);

					}
					PlaneY = transform.position.y;
					//transform.rotation = initRotation;

                    transform.rotation = Quaternion.Euler(0.0f, transform.eulerAngles.y, 0.0f);
					if (Hi5_Interaction_Const.TestChangeState)
					{
						Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject, mObjectType, EHandType.EHandLeft, EEventObjectType.EStatic);
						Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
					}
                    
                }
				if (collision.gameObject.layer == Hi5_Interaction_Const.OtherFingerTailLayer() 
					|| collision.gameObject.layer == Hi5_Interaction_Const.ThumbTailLayer())
                {
					//Debug.Log ("Hi5_Interaction_Const.OtherFingerTailLayer");
                    if (collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger>())
                    {
						


                        //float dot = Vector3.Dot(collision.contacts[0].normal, collision.gameObject.transform.parent.right);
                        //if (dot < 0.0f)
                        if (mObjectType == EObject_Type.ECommon)
                        {
							if (state == E_Object_State.EStatic || state == E_Object_State.EFlyLift)
							{

								if (!IsPokeInLoop) 
								{

									ContactPoint[] contactPoints = collision.contacts;
									if (contactPoints != null && contactPoints.Length > 0)
									{
										Vector3 normal =  contactPoints [0].normal;
										float angle = Vector3.Angle(Hi5_Interaction_Object_Manager.GetObjectManager ().transform.up, normal);

										if (Mathf.Abs(angle)>25.0f) 
										{
											
											NotifyPokeEvent (collision);
											IsPokeInLoop = true;

										}
									}
								}
							}	




							if (Hi5_Interaction_Const.TestChangeState) 
							{
								if (mstatemanager.State == E_Object_State.EStatic 
									|| (mstatemanager.State == E_Object_State.EMove && mstatemanager.GetMoveState().mMoveType == Hi5ObjectMoveType.EPlaneMove))
								{
									Hi5_Hand_Visible_Hand handTemp = collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger>().mHand;
									if (mstatemanager != null)
										mstatemanager.SetPlaneMove(collision);
									ChangeState(E_Object_State.EMove);
									{
										Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject, mObjectType, handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight, EEventObjectType.EMove);
										Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
									}
								}
							}
							else
							{
								if (mstatemanager.State == E_Object_State.EStatic || mstatemanager.State == E_Object_State.EMove)
								{
									Hi5_Hand_Visible_Hand handTemp = collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger>().mHand;
									if (mstatemanager != null)
										mstatemanager.SetPlaneMove(collision);
									ChangeState(E_Object_State.EMove);
									{
										Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject, mObjectType, handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight, EEventObjectType.EMove);
										Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
									}
								}
							}
                            
                        }
                    }
                }
            }
           UnityEngine.Profiling.Profiler.EndSample();;
        }




        override protected void OnCollisionStay(Collision collision)
        {
            base.OnCollisionStay(collision);
            if (mObjectType == EObject_Type.ECommon)
            {

				if (collision.gameObject.layer == Hi5_Interaction_Const.ThumbTailLayer()
					|| collision.gameObject.layer == Hi5_Interaction_Const.OtherFingerTailLayer()
					|| collision.gameObject.layer == Hi5_Interaction_Const.PalmRigidbodyLayer())
                {
					if (mstatemanager == null)
						return;
                    // if (collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger>())
                    {
                      
						if (Hi5_Interaction_Const.TestChangeState)
						{
							if (mstatemanager.State == E_Object_State.EStatic
							    || (mstatemanager.State == E_Object_State.EMove && mstatemanager.GetMoveState ().mMoveType == Hi5ObjectMoveType.EPlaneMove)) 
							{
								if (collision.gameObject.layer == Hi5_Interaction_Const.ThumbTailLayer() || collision.gameObject.layer == Hi5_Interaction_Const.OtherFingerTailLayer())
								{
									if (collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Thumb_Finger>() == null)
										return;
									Hi5_Hand_Visible_Finger finger = collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Thumb_Finger>().mFinger;
									if (finger && (finger is Hi5_Hand_Visible_Thumb_Finger))
									{
										if (!(finger as Hi5_Hand_Visible_Thumb_Finger).IsMoveTowardHand())
										{
											if (mstatemanager != null)
												mstatemanager.SetPlaneMove(collision);
											Hi5_Hand_Visible_Hand handTemp = collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger> ().mHand;
											ChangeState(E_Object_State.EMove);
											{
												Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject, 
													mObjectType, 
													handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
													EEventObjectType.EMove);
												Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
											}
										}
									}
								}
								else
								{
									if (collision.gameObject.layer == Hi5_Interaction_Const.PalmRigidbodyLayer())
									{
										if (mstatemanager != null)
											mstatemanager.SetPlaneMove(collision);
										ChangeState(E_Object_State.EMove);
										{
											Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject,
												mObjectType,
												collision.gameObject.GetComponent<Hi5_Hand_Palm_Move>().mHand.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
												EEventObjectType.EMove);
											Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
										}
										// Debug.Log("collision.gameObject.layer == Hi5_Interaction_Const.PalmRigibodyLayer");
									}
								}
							}
						}
						else
                        {
                            if (mstatemanager.State == E_Object_State.EStatic || mstatemanager.State == E_Object_State.EMove)
                            {
								if (collision.gameObject.layer == Hi5_Interaction_Const.ThumbTailLayer() || collision.gameObject.layer == Hi5_Interaction_Const.OtherFingerTailLayer())
                                {
                                    if (collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Thumb_Finger>() == null)
                                        return;
                                    Hi5_Hand_Visible_Finger finger = collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Thumb_Finger>().mFinger;
                                    if (finger && (finger is Hi5_Hand_Visible_Thumb_Finger))
                                    {
                                        if (!(finger as Hi5_Hand_Visible_Thumb_Finger).IsMoveTowardHand())
                                        {
                                            if (mstatemanager != null)
                                                mstatemanager.SetPlaneMove(collision);
											Hi5_Hand_Visible_Hand handTemp = collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger> ().mHand;
											ChangeState(E_Object_State.EMove);
                                            {
                                                Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject, 
                                                    mObjectType, 
													handTemp.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
                                                    EEventObjectType.EMove);
												Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
                                            }
                                        }
                                    }
                                }
                                else
                                {
									if (collision.gameObject.layer == Hi5_Interaction_Const.PalmRigidbodyLayer())
                                    {
                                        if (mstatemanager != null)
                                            mstatemanager.SetPlaneMove(collision);
										ChangeState(E_Object_State.EMove);
                                        {
                                            Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance(idObject,
                                                mObjectType,
                                                collision.gameObject.GetComponent<Hi5_Hand_Palm_Move>().mHand.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
                                                EEventObjectType.EMove);
											Hi5InteractionManager.Instance.GetMessage().DispenseMessage(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
                                        }
                                        // Debug.Log("collision.gameObject.layer == Hi5_Interaction_Const.PalmRigibodyLayer");
                                    }
                                }
                            }
                        }
                    }
                    //if(collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger>())
                }
            }
        }

        override protected void OnCollisionExit(Collision collision)
        {
            base.OnCollisionExit(collision);
           
        }

        internal void SetIsKinematic(bool param)
        {
            GetComponent<Rigidbody>().isKinematic = param;
        }

        internal void SetUseGravity(bool param)
        {
            GetComponent<Rigidbody>().useGravity = param;
        }

		internal void SetAllLock()
		{
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
		}

        internal void SetIsLockYPosition(bool param)
        {
			//GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;

           if (param)
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
            }
            else
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }

		private void NotifyPokeEvent(Collision collision)
		{
			if (collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger> () != null) {
				{
					Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance (idObject,
						                                               mObjectType,
						                                               collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger> ().mFinger.mHand.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
						                                               EEventObjectType.EPoke);
					Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
				}

				{
					Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance (idObject,
						                                             collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Finger> ().mFinger.mHand.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
						                                             EEventHandType.EPoke);
					Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
				}
			}
			else if (collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Thumb_Finger> () != null) {
				{
					Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance (idObject,
						mObjectType,
						collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Thumb_Finger> ().mFinger.mHand.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
						EEventObjectType.EPoke);
					Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
				}

				{
					Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance (idObject,
						collision.gameObject.GetComponent<Hi5_Hand_Collider_Visible_Thumb_Finger> ().mFinger.mHand.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
						EEventHandType.EPoke);
					Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
				}
			}
		}


		private void NotifyPokeEvent(Collider other)
		{
			
			{
				Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance (idObject,
					mObjectType,
					other.gameObject.GetComponent<Hi5_Glove_Collider_Finger> ().mFinger.mHand.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
					EEventObjectType.EPoke);
				Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
			}

			{
				Hi5_Glove_Interaction_Hand_Event_Data data = Hi5_Glove_Interaction_Hand_Event_Data.Instance (idObject,
					other.gameObject.GetComponent<Hi5_Glove_Collider_Finger> ().mFinger.mHand.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
					EEventHandType.EPoke);
				Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageHandEvent, (object)data, null);
			}
		}

        override protected void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (mObjectType == EObject_Type.EButton || mObjectType == EObject_Type.ECommon)
            {
				if (other.gameObject.layer == Hi5_Interaction_Const.ThumbTailLayer()
					|| other.gameObject.layer == Hi5_Interaction_Const.OtherFingerTailLayer())
                {
					//Debug.Log ("item OnTriggerEnter Tail");

                    if (other.gameObject.GetComponent<Hi5_Glove_Collider_Finger>() != null
                        && other.gameObject.GetComponent<Hi5_Glove_Collider_Finger>().mFinger != null
                        && other.gameObject.GetComponent<Hi5_Glove_Collider_Finger>().mFinger.mHand != null)
                    {

						if (mObjectType == EObject_Type.EButton)
						{
							if (state == E_Object_State.EStatic )
							{
								this.ChangeState(E_Object_State.EPoke);
								NotifyPokeEvent (other);
							}	
						}
						else
						{
							if (state == E_Object_State.EStatic || state == E_Object_State.EFlyLift)
							{
								

							}	
						}

                    }
                        //ChangeColor(Color.black);
                        
                }
            }
        }

		internal bool IsLiftTrigger()
		{
			foreach (Collider item in m_Triggers)
			{
				if (item.gameObject.GetComponent<Hi5_Glove_Collider_Finger> () != null) {
					return true;
				}
				if (item.gameObject.GetComponent<Hi5_Glove_Collider_Palm> () != null) {
					return true;
				}

			}
			return false;
		}


        override protected void OnTriggerStay(Collider other)
        {
            base.OnTriggerStay(other);
        }


        override protected void OnTriggerExit(Collider other)
        {

            base.OnTriggerExit(other);
			if (mObjectType == EObject_Type.EButton) {
				if (other.gameObject.layer == Hi5_Interaction_Const.ThumbTailLayer ()
				    || other.gameObject.layer == Hi5_Interaction_Const.OtherFingerTailLayer ()) {

					if (other.gameObject.GetComponent<Hi5_Glove_Collider_Finger> () != null
					    && other.gameObject.GetComponent<Hi5_Glove_Collider_Finger> ().mFinger != null
					    && other.gameObject.GetComponent<Hi5_Glove_Collider_Finger> ().mFinger.mHand != null) {
						if (state == E_Object_State.EPoke) {
							this.ChangeState (E_Object_State.EStatic);
							if (Hi5_Interaction_Const.TestChangeState) {
								Hi5_Glove_Interaction_Object_Event_Data data = Hi5_Glove_Interaction_Object_Event_Data.Instance (idObject,
									                                               mObjectType,
									                                               other.gameObject.GetComponent<Hi5_Glove_Collider_Finger> ().mFinger.mHand.m_IsLeftHand ? EHandType.EHandLeft : EHandType.EHandRight,
									                                               EEventObjectType.EStatic);
								Hi5InteractionManager.Instance.GetMessage ().DispenseMessage (Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent, (object)data, null);
							}

						}	
					}
				}
			}
			else if (mObjectType == EObject_Type.ECommon)
			{
				if (other.gameObject.layer == Hi5_Interaction_Const.ThumbTailLayer ()
				    || other.gameObject.layer == Hi5_Interaction_Const.OtherFingerTailLayer ()) {

					if (other.gameObject.GetComponent<Hi5_Glove_Collider_Finger> () != null
					    && other.gameObject.GetComponent<Hi5_Glove_Collider_Finger> ().mFinger != null
					    && other.gameObject.GetComponent<Hi5_Glove_Collider_Finger> ().mFinger.mHand != null) {
						IsPokeInLoop = false;
					}
				}
			}
        }


		internal float GetMass()
		{
			float mass = 1.0f;
			if(GetComponent<Rigidbody>() != null)
			{
				mass = GetComponent<Rigidbody> ().mass;
			}
			return mass;
		}
       
		internal float GetDrag()
		{
			float drag = 0.0f;
			if(GetComponent<Rigidbody>() != null)
			{
				drag = GetComponent<Rigidbody> ().drag;
				//Debug.Log ("Drag" + drag);
			}
			return drag;
		}

		internal float GetHeight()
		{
			float Height = 1.0f;
			if (GetComponent<BoxCollider> () != null) {
				Height = GetComponent<BoxCollider> ().size.y;
			}
			else if (GetComponent<SphereCollider> () != null) {
				Height = GetComponent<SphereCollider> ().radius*2;
			}

			else if (GetComponent<CapsuleCollider> () != null) {
				Height = GetComponent<CapsuleCollider> ().height;
			}
			return Height;
		}
    }
}
