using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class AutoObjectParenter : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public enum ParentEventTarget
    {
        SLENDERMAN = 0,
        PAGE = 1
    }

    public ParentEventTarget parentEventTarget;
    private const byte ParentToObjectEventCode = 9;
    private Transform _parent = null;
    private bool CanParentChangeAgain = true;

    public static void SendParentEvent(ParentEventTarget objectsWhichToParent, GameObject objectToParent)
    {
        int id = objectToParent == null ? -1 : objectToParent.GetComponent<PhotonView>().ViewID;
        object[] content = {(int) objectsWhichToParent, id};
        RaiseEventOptions raiseEventOptions = new() {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(ParentToObjectEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == ParentToObjectEventCode && _parent == null)
        {
            object[] data = (object[]) photonEvent.CustomData;
            if ((int) parentEventTarget == (int) data[0] && CanParentChangeAgain)
            {
                if ((int) data[1] == -1)
                {
                    _parent = null;
                    gameObject.transform.parent = _parent;
                    CanParentChangeAgain = false;
                }
                else
                {
                    _parent = PhotonView.Find((int) data[1]).gameObject.transform;
                    gameObject.transform.parent = _parent;
                    CanParentChangeAgain = false;
                }
            }
        }
    }
}