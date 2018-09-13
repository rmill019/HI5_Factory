using UnityEngine;
using Hi5_Interaction_Core;

public class FactoryContainer : MonoBehaviour, IConveyerMover
{
    public FloatVariable m_conveyerSpeed;
    public float m_delay = 0.5f;
    public Transform m_bottomContainer;

    private float m_activationTime;
    private bool b_canMove = false;
    private bool b_isInUse = false;
    private bool b_timerActive = false;
    private bool b_isFalling = false;
    private Vector3 m_spawnPos;
    private Rigidbody m_rigid;
    private Hi5_Glove_Interaction_Item m_interaction;

	// Use this for initialization
	void Awake () {
        m_rigid = GetComponent<Rigidbody>();
        m_interaction = GetComponent<Hi5_Glove_Interaction_Item>();
        m_interaction.enabled = false;
        m_spawnPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        CheckState();

        if (Time.time >= m_activationTime && b_timerActive)
            b_canMove = true;

        if (b_canMove)
            ConveyerMovement();
		
	}

    private void FixedUpdate()
    {
        if (b_isFalling)
            CheckForGround(m_bottomContainer.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("Factory Object Collision Detected");
        if (collision.gameObject.tag == "Conveyer")
        {
            b_timerActive = true;
            m_activationTime = Time.time + m_delay;
        }
    }

    void CheckForGround(Vector3 castPosition)
    {
        int layerMask = 1;
        Ray ray = new Ray(castPosition, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 20f, layerMask, QueryTriggerInteraction.Collide))
        {
            //Debug.LogWarning("Container hit ground");
            if (Mathf.Abs(hit.distance) < 0.15f)
            {
                m_interaction.mstatemanager.ChangeState(E_Object_State.EStatic);
                m_interaction.mstatemanager.StopThrowMove();
                transform.rotation = Quaternion.Euler(0, 0, 0);
                m_rigid.useGravity = true;
                m_rigid.isKinematic = false;
            }
        }
    }

    public void ConveyerMovement()
    {
        if (m_rigid)
        {
            m_rigid.velocity = new Vector3(m_conveyerSpeed.Value, 0f, 0f);
        }
    }

    void CheckState()
    {
        if (m_interaction.state == E_Object_State.EMove && m_interaction.moveType == Hi5ObjectMoveType.EThrowMove)
            b_isFalling = true;
        else
            b_isFalling = false;
    }

    #region Properties
    public bool CanMove
    {
        get { return b_canMove; }
        set { b_canMove = value; }
    }

    public bool IsFalling
    {
        get { return b_isFalling; }
        set { b_isFalling = value; }
    }

    // TODO Figure out why This container jitters upon contact with the track. It's too visually jarring to
    // be put in the final experience
    public bool TimerActive
    {
        get { return b_timerActive; }
        set { b_timerActive = value; }
    }

    public bool IsInUse
    {
        get { return b_isInUse; }
        set { b_isInUse = value; }
    }
    #endregion
}
