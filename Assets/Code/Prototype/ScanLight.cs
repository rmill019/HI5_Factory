using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanLight : MonoBehaviour {

    [Header("Information regarding the light in the scanner")]
    public Color m_lightStartColor;
    public Color m_scanPassedColor;
    public Color m_scanFailedColor;
    public float m_lightDuration;

    private Light m_scanLight = null;

    // Use this for initialization
    void Start () {
        m_scanLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // This function is a needed because a Color can't be passed into a Unity Event
    // Fail = 0 Pass = 1
    public void ChangeLight (int status)
    {
        Color targetLightColor = Color.white;
        if (status == (int)E_ScanStatus.Fail)
            targetLightColor = m_scanFailedColor;
        else
            targetLightColor = m_scanPassedColor;
        // This call does all the work
        StartCoroutine(ChangeLightRoutine(targetLightColor));
    }

    IEnumerator ChangeLightRoutine(Color targetColor)
    {
        float timeToSwitchBack = Time.time + m_lightDuration;
        m_scanLight.color = targetColor;
        while (Time.time < timeToSwitchBack)
        {
            yield return null;
        }

        m_scanLight.color = m_lightStartColor;
    }
}
