using UnityEngine;
using Hi5_Interaction_Core;

public class FactoryContainer : MonoBehaviour, IConveyerMover
{
    public FloatVariable m_conveyerSpeed;
    public float m_delay = 0.5f;
    public Transform m_bottomContainer;

    private float m_activationTime;
    public bool b_canMove = false;
    private bool b_isInUse = false; // Are we using this variable?
    private bool b_timerActive = false;
    private bool b_isFalling = false;
    private bool [] checkList = new bool[2];
    private Quaternion m_starRot;
    // Variables that determine the scan status
    public bool b_isFilled = false;
    public bool b_isTopped = false;

    private Vector3 m_spawnPos;
    private Rigidbody m_rigid;
    private Hi5_Glove_Interaction_Item m_interaction;

	// Use this for initialization
	void Awake () {
        m_rigid = GetComponent<Rigidbody>();
        m_starRot = transform.rotation;
        m_interaction = GetComponent<Hi5_Glove_Interaction_Item>();
        m_spawnPos = transform.position;
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
        CheckState();

        if (Time.time >= m_activationTime && b_timerActive && !b_canMove)
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
                transform.rotation = Quaternion.Euler(0, 90f, 0);
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

    // Factory Object that's "inside" of the Factory Container
    public void ActivatePlaceHolderFactoryObj ()
    {
        transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    void CheckState()
    {
        if (m_interaction.state == E_Object_State.EMove && m_interaction.moveType == Hi5ObjectMoveType.EThrowMove)
            b_isFalling = true;
        else
            b_isFalling = false;
    }


    public void UpdateCheckList()
    {
        checkList[0] = IsFilled;
        checkList[1] = IsTopped;
    }

    // Used to Reset / Initialize Values back to default once it has been scanned
    public void Initialize()
    {
        transform.rotation = m_starRot;
        b_canMove = false;
        b_timerActive = false;
        m_interaction.enabled = false;
        IsTopped = false;
        IsFilled = false;
        // Do we need to turn off any mesh renderers and make colliders back into triggers
        transform.GetChild(2).GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(2).GetComponent<MeshCollider>().isTrigger = true;

        UpdateCheckList();
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

    // Properties that determine Scan Status
    // Have we placed a Factory Object in the box
    public bool IsFilled
    {
        get { return b_isFilled; }
        set { b_isFilled = value; }
    }

    // Have we successfully put a top on the box
    public bool IsTopped
    {
        get { return b_isTopped; }
        set { b_isTopped = value; }
    }

    public Vector3 SpawnPos
    {
        get { return m_spawnPos; }
        set { m_spawnPos = value; }
    }

    public bool[] CheckList
    {
        get { return checkList; }
    }

    #endregion
}
