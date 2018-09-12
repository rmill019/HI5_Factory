using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Hi5_Interaction_Core
{
    public class Hi5_Glove_Collider_Palm : Hi5_Glove_Interaction_Collider
    {
        #region message
        internal Transform mAnchor;
        //Hi5_Glove_Interaction_Message mMessage = null;
        internal protected Hi5_Glove_Interaction_Hand mHand = null;
		internal Hi5_Glove_Interaction_Collider mChildCollider = null;
        internal protected void SetHi5Message(Hi5_Glove_Interaction_Message message)
        {
            //mMessage = message;
        }
        #endregion
        /// <summary>
        /// judge whether the object is on the inside or outside
        /// </summary>
        protected internal bool JudgeObjectHandInside(Transform objectTransform)
        {
            if(mAnchor == null)
                mAnchor = transform.GetChild(0);
            Vector3 pamlUp = -mAnchor.up;
            Vector3 v1 = objectTransform.position - mAnchor.position;
            //float dotValue = Vector3.Dot(v1, pamlUp);
            float angle = Vector3.Angle(v1, pamlUp);
            if (angle > 30.0f && angle < 75.0f)
            {
                //Debug.Log("angle" + angle);
                return true;
            }
            else
            {
                //Debug.Log("angle" + angle);
                return false;
            }
        }

        #region unity system
        protected override void Awake()
        {
            base.Awake();
            mAnchor = transform.GetChild(0);
			mChildCollider = gameObject.GetComponentInChildren<Hi5_Glove_Interaction_Collider> ();
        }
        override protected void OnEnable()
        {
            base.OnEnable();
            
        }
        override protected void OnDisable()
        {
            base.OnDisable();
        }
        #endregion

		internal bool IsLift(out List<int> collisions)
		{
			if (mChildCollider != null) {
				List<Collision> colliders = mChildCollider.GetCollisions();
				if (colliders != null && colliders.Count > 0)
				{
					collisions = GetObjectIdByCollision(colliders);
					return true;
				}
				else
				{
					collisions = null;
					return false;
				}
			}
			else {
				collisions = null;
				return false;
			}

		}

        internal bool IsClap(out List<int> collisions)
        {
            List<Collider> colliders = GetTriggers();
            if (colliders != null && colliders.Count > 0)
            {
                collisions = GetObjectIdByCollider(colliders);
                return true;
            }
            else
            {
                collisions = null;
                return false;
            }
        }

        private List<int> GetObjectIdByCollider(List<Collider> colliders)
        {
            if (colliders == null)
                return null;
            List<int> colliderStrings = new List<int>();
            foreach (Collider item in colliders)
            {
                if (item.GetComponent<Hi5_Glove_Interaction_Item>() != null)
                {
                    colliderStrings.Add(item.GetComponent<Hi5_Glove_Interaction_Item>().idObject);
                }
            }
            if (colliderStrings.Count == 0)
                return null;
            List<int> ListResult = colliderStrings.Distinct().ToList();
            return ListResult;
        }

		private List<int> GetObjectIdByCollision(List<Collision> colliders)
		{
			if (colliders == null)
				return null;
			List<int> colliderStrings = new List<int>();
			foreach (Collision item in colliders)
			{
				if (item.collider.GetComponent<Hi5_Glove_Interaction_Item>() != null)
				{
					colliderStrings.Add(item.collider.GetComponent<Hi5_Glove_Interaction_Item>().idObject);
				}
			}
			if (colliderStrings.Count == 0)
				return null;
			List<int> ListResult = colliderStrings.Distinct().ToList();
			return ListResult;
		}
		internal void OpenPhyCollider(bool open)
		{
			if (mAnchor == null)
				mAnchor = transform.GetChild (0);
			if (mAnchor != null) {
				if (open)
					mAnchor.gameObject.SetActive (true);
				else
					mAnchor.gameObject.SetActive (false);
			}	
		}
    }
}
