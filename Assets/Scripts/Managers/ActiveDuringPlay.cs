using UnityEngine;

public class ActiveDuringPlay : MonoBehaviour
{
    public virtual void Awake()
    {
        Messenger.AddListener(GameEvent.GAME_ACTIVE, OnGameActive);
        Messenger.AddListener(GameEvent.GAME_INACTIVE, OnGameInactive);
    }

    public virtual void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.GAME_ACTIVE, OnGameActive);
        Messenger.RemoveListener(GameEvent.GAME_INACTIVE, OnGameInactive);
    }

    public virtual void OnGameActive()
    {
        this.enabled = true;
    }

    public virtual void OnGameInactive()
    {
        this.enabled = false;
    }
}

