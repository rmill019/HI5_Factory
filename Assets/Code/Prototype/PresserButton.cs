using UnityEngine;
using Hi5_Interaction_Interface;
using Hi5_Interaction_Core;
using CustomEvents;

public class PresserButton : Hi5_Reset_Button
{

    public GameEvent ActivatePresserEvent;
    public GameEvent BeginGameEvent;

    bool b_FirstPress = true;
    bool isTrigger = false;
    float cd = 0.8f;

    override protected void MessageFun(string messageKey, object param1, object param2)
    {
        if (messageKey.CompareTo(Hi5_Glove_Interaction_Message.Hi5_MessageMessageKey.messageObjectEvent) == 0)
        {
            Hi5_Glove_Interaction_Object_Event_Data data = param1 as Hi5_Glove_Interaction_Object_Event_Data;
            if (data.mObjectId == ObjectItem.idObject)
            {
                if (data.mEventType == EEventObjectType.EClap)
                {
                    ObjectItem.ChangeColor(Color.gray);
                    if (!isTrigger)
                    {
                        Hi5_Interaction_Message.GetInstance().DispenseMessage(Hi5_MessageKey.messageObjectReset, null, null);
                        isTrigger = true;
                    }

                }
                else if (data.mEventType == EEventObjectType.EPoke)
                {
                    ObjectItem.ChangeColor(Color.red);
                    // If this is the first time the button is pressed then Start the Game
                    // Otherwise this is the BoxPresser Button
                    if (b_FirstPress)
                    {
                        b_FirstPress = false;
                        BeginGameEvent.Raise();
                    }
                    else
                        ActivatePresserEvent.Raise();
                }
                else if (data.mEventType == EEventObjectType.EStatic)
                {

                }
            }
        }
    }

    private new void Update()
    {
        base.Update();
        if (isTrigger)
        {
            cd -= Time.deltaTime;
            if (cd <= 0.0f)
            {
                cd = 0.5f;
                isTrigger = false;
            }
        }
    }
}
