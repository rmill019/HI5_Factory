using UnityEngine;
using Hi5_Interaction_Core;

public class ConveyerBelt : MonoBehaviour {
    public FloatVariable m_speed;

    private Renderer rend;
    // Layers
    private int m_defaultLayer = 0;
    private int m_HI5GraspLayer = 15;

    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        // Using the negative m_speed.Value because we need our texture offset to move in the desired direction. 
        // A positive value makes the conveyer belt appear to move in the wrong direction
        float offset = Time.time * -m_speed.Value;
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
	}

}
