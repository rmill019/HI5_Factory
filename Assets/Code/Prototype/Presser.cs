using UnityEngine;

public class Presser : MonoBehaviour {

    public AnimationClip pressClip;
    private Animator m_anim;

	// Use this for initialization
	void Start () {

        m_anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P))
        {
            m_anim.SetTrigger("Press");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            print("Increase animation speed");
            m_anim.speed += 0.5f;
        }
	}

    public void ReplenishContainer()
    {
        print("Container Replenished");
        GameManager.S.GrabUnusedFactoryContainer();
    }
}
