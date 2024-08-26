using UnityEngine;

public class BasePopup : MonoBehaviour
{
    virtual public void Open()
    {
        if (!IsActive())
        {
            this.gameObject.SetActive(true);
            Messenger.Broadcast(GameEvent.MENU_OPENED);
        }
        else
        {
            Debug.LogError(this + ".Open() - trying to open the menu while it is active!");
        } 
    }

    virtual public void Close()
    {
        if (IsActive())
        {
            this.gameObject.SetActive(false);
            Messenger.Broadcast(GameEvent.MENU_CLOSED);
        }
        else
        {
            Debug.LogError(this + ".Closed() - trying to close the menu while it is not open!");
        }
        gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
