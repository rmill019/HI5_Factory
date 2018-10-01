using Hi5_Interaction_Core;
using UnityEngine;

public class ActivationButton : MonoBehaviour
{
    public GameObject AffectedDoor;

    private AudioSource _AudioSource;

	// Use this for initialization
	void Start () {
        _AudioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains ("Finger"))
        {
            AffectedDoor.GetComponent<Animator>().SetTrigger("Open");
            print("Opening Door");
        }
    }
}
