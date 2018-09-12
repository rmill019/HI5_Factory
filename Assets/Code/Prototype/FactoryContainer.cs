using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryContainer : MonoBehaviour, IConveyerMover
{
    public FloatVariable m_conveyerSpeed;
    public float m_delay = 0.5f;

    private float m_activationTime;
    private bool b_canMove = false;
    private bool b_timerActive = false;
    private Rigidbody m_rigid;

	// Use this for initialization
	void Start () {
        m_rigid = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time >= m_activationTime && b_timerActive)
            b_canMove = true;

        if (b_canMove)
            ConveyerMovement();
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        print("Factory Object Collision Detected");
        if (collision.gameObject.tag == "Conveyer")
        {
            b_timerActive = true;
            m_activationTime = Time.time + m_delay;
        }
    }

    public void ConveyerMovement()
    {
        if (m_rigid)
        {
            m_rigid.velocity = new Vector3(m_conveyerSpeed.Value, 0f, 0f);
            //print("Velocity: " + m_rigid.velocity);
        }
    }

    #region Properties
    public bool CanMove
    {
        get { return b_canMove; }
        set { b_canMove = value; }
    }

    // TODO Figure out why This container jitters upon contact with the track. It's too visually jarring to
    // be put in the final experience
    public bool TimerActive
    {
        get { return b_timerActive; }
        set { b_timerActive = value; }
    }
    #endregion
}
