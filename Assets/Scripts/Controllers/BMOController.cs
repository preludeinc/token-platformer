using UnityEngine;

public class BMOController : MonoBehaviour
{
    [SerializeField] Animator BMOanim;

    private bool winCondition = false;
    private Vector2 BMOPos;
    private Vector2 targetPos;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.DANCE_PARTY, OnDanceParty);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.DANCE_PARTY, OnDanceParty);
    }

    private void Update()
    {
        BMOPos = transform.position;
        targetPos = new Vector2(BMOPos.x, (float)BMOPos.y + 0.001f);
    }

    public void OnDanceParty()
    {
        winCondition = true;
        // BMO is shifted up
        transform.position = Vector2.MoveTowards(BMOPos, targetPos, Time.deltaTime);
        BMOanim.SetBool("winCondition", winCondition);
    }

    public Vector2 BMOLocation
    {
        get { return transform.position; }
    }
}
