using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip coinSound;

    private int value = 1;
    private int coinValue = 1;

    private void Update()
    {
        Vector3 rotation = Vector3.up * 180 * Time.deltaTime;
        transform.Rotate(rotation, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (tag == "Health")
            {
                Messenger<int>.Broadcast(GameEvent.PICKUP_HEALTH, value);
                Destroy(this.gameObject);
            }
            if (tag == "Coin")
            {
                Messenger<int>.Broadcast(GameEvent.PICKUP_COIN, coinValue);
                soundManager.PlaySoundClip(coinSound, transform, 1f);
                Destroy(this.gameObject);
            }
        }
    }
}
