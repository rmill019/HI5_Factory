using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hi5_Interaction_Core;

public class FactoryObject : MonoBehaviour, IConveyerMover
{

    public bool b_isBoxed = false;
    public bool b_isSealed = false;
    public bool b_isStamped = false;
    public FloatVariable m_conveyerSpeed;

    public Vector3 m_spawnPos;
    private bool b_canMove = false;
    public bool b_isInUse = false;
    // This will hold all the bool values of our FactoryObject to make it easier to iterate over
    private bool[] checkList = new bool[3];
    private bool b_isFalling = false;
    private Rigidbody m_rigid;
    private Hi5_Glove_Interaction_Item m_interaction;
    // Layers
    private int m_defaultLayer = 0;
    private int m_HI5GraspLayer = 15;

    //private void OnEnable()
    //{
    //    m_rigid = GetComponent<Rigidbody>();
    //    Initialize();
    //}

    private void Awake()
    {
        m_interaction = GetComponent<Hi5_Glove_Interaction_Item>();
        m_rigid = GetComponent<Rigidbody>();
        m_spawnPos = transform.position;
        Initialize();
    }

    private void Update()
    {
        CheckState();

        if (b_canMove)
        {
            ConveyerMovement();
            //Vector3 translationVector = new Vector3(m_conveyerSpeed.Value, 0, 0);
            //transform.Translate(translationVector * Time.deltaTime, Space.World);
        }
    }

    private void FixedUpdate()
    {
        if (b_isFalling)
            CheckForGround(transform.position);
    }

    private void OnDisable()
    {
        b_canMove = false;
        m_rigid.velocity = Vector3.zero;
        m_rigid.angularVelocity = Vector3.zero;
        m_rigid.useGravity = false;
    }

    // Custom Functions
    // ------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------
    void UpdateCheckList()
    {
        checkList[0] = b_isBoxed;
        checkList[1] = b_isSealed;
        checkList[2] = b_isStamped;
    }

    void AssignCorrectLayer()
    {
        if (gameObject.layer == m_defaultLayer)
            gameObject.layer = m_HI5GraspLayer;
        else
        {
            //Debug.LogWarning("switch to default");
            gameObject.layer = m_defaultLayer;
        }
    }

    public void Initialize()
    {
        b_canMove = false;
        m_interaction.enabled = false;
        m_rigid.useGravity = false;
        IsInUse = false;
        transform.position = m_spawnPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("Factory Object Collision Detected");
        if (collision.gameObject.tag == "Conveyer")
        {
            b_canMove = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Once we pass through the LayerConverter we are in the box. So after setting a few key parameter
        // we then parent the FactoryObject to the container and make our RB kinematic.
        // This ensures that the ball will not fly out of the box if moved too quickly for the physics simulation
        if (other.gameObject.tag == "LayerConverter")
        {
            // Added for when the Object is dropped in a container
            FactoryContainer fCon = other.transform.parent.parent.GetComponent<FactoryContainer>();
            if (fCon != null)
                DropObjectInContainer(fCon);
            //if (fCon != null)
            //{
            //    Debug.LogWarning("Factory Container Detected");
            //    fCon.ActivatePlaceHolderFactoryObj();
            //    fCon.IsFilled = true;
            //    fCon.UpdateCheckList();
            //    // Send this Factory Object back to the Factory object "pool"
            //    this.Initialize();
            //}
            //// todo play sound?
            //// Disable Interaction item
            //m_interaction.enabled = false;
            //m_rigid.useGravity = false;
            //m_rigid.isKinematic = true;
            //m_rigid.constraints = RigidbodyConstraints.None;

            ////UpdateCheckList();
        }
    }

    void DropObjectInContainer (FactoryContainer targetContainer)
    {
        targetContainer.ActivatePlaceHolderFactoryObj();
        targetContainer.IsFilled = true;
        targetContainer.UpdateCheckList();
        // Send this Factory Object back to the Factory object "pool"
        Initialize();

        // todo play sound?
        // Disable Interaction item
        m_interaction.enabled = false;
        m_rigid.useGravity = false;
        m_rigid.isKinematic = true;
        m_rigid.constraints = RigidbodyConstraints.None;

        //UpdateCheckList();
    }



    #region IConveyerMover Implementation
    public void ConveyerMovement ()
    {
        if (m_rigid)
        {
            transform.Translate(Vector3.right * m_conveyerSpeed.Value * Time.deltaTime);
            //m_rigid.velocity = new Vector3(m_conveyerSpeed.Value, 0f, 0f);
            //print("Velocity: " + m_rigid.velocity);
        }
    }
    #endregion

    #region TestingFunctions

    // TODO Refactor
    void CheckState ()
    {
        if (m_interaction.state == E_Object_State.EMove && m_interaction.moveType == Hi5ObjectMoveType.EThrowMove)
            b_isFalling = true;
        else
            b_isFalling = false;
    }

    void CheckForGround (Vector3 castPosition)
    {
        int layerMask = 1;
        Ray ray = new Ray(castPosition, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 20f, layerMask, QueryTriggerInteraction.Collide))
        {
            if (Mathf.Abs(hit.distance) < 0.15f)
            {
                m_interaction.mstatemanager.ChangeState(E_Object_State.EStatic);
                m_interaction.mstatemanager.StopThrowMove();
                m_rigid.useGravity = true;
                m_rigid.isKinematic = false;
            }
        }
    }

    #endregion

    #region Properties
    public bool[] CheckList
    {
        get { return checkList; }
    }

    public bool CanMove
    {
        get { return b_canMove; }
        set { b_canMove = value; }
    }

    public bool IsInUse
    {
        get { return b_isInUse; }
        set { b_isInUse = value; }
    }

    public bool IsFalling
    {
        get { return b_isFalling; }
        set { b_isFalling = value; }
    }

    public Vector3 SpawnPos
    {
        get { return m_spawnPos; }
        set { m_spawnPos = value; }
    }
    #endregion
}
