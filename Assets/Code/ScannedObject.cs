using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/ScannedObject")]
public class ScannedObject : ScriptableObject
{
    public bool b_isBoxed;
    public bool b_isSealed;
    public bool b_isStamped;
}
