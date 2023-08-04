using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float gravityAcceleration;
    public Vector3 velocity;
    [SerializeField]private float bulletSpeed;
    [SerializeField][Range(0f, 1f)]private float ricochetteChance;
    [SerializeField]private AudioClip ricochetteSound;
    [SerializeField] private LayerMask playerLayer;

    private void Start()
    {
        gravityAcceleration = -4f * Time.fixedDeltaTime * Time.fixedDeltaTime;
        velocity = transform.forward * bulletSpeed;
    }

    private void FixedUpdate()
    {
        if (velocity == null)
            return;
        velocity.y += gravityAcceleration;
        Vector3 nextPosition = transform.position + velocity;

        if (Physics.Raycast(transform.position, velocity.normalized, out RaycastHit hit, velocity.magnitude, ~playerLayer))
        {
            Target hitTarget;
            if (hitTarget = hit.collider.GetComponent<Target>())
            {
                hitTarget.TakeHit();
                BulletDestroy();
            }
            else
            {
                if (DoesRicochette())
                {
                    nextPosition = Vector3.Reflect(nextPosition, hit.normal);
                    AudioManager.PlaySoundOnPlayer(ricochetteSound);
                    velocity /= 3f;
                }
                else
                    BulletDestroy();
            }
        }

        transform.position = nextPosition;
    }

    private bool DoesRicochette()
    {
        float randNumb = Random.Range(0f, 1f);
        if (randNumb <= ricochetteChance)
            return true;
        else return false;
    }

    private void BulletDestroy()
    {
        Destroy(gameObject);
    }
}
