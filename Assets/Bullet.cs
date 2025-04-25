using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTimeSeconds = 10f;
    public int bulletDamage = 25;
    public void Start()
    {
        Destroy(gameObject, lifeTimeSeconds);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("IgnoreBullet"))
        {
            if (collision.gameObject.CompareTag("Wall") ||
                collision.gameObject.CompareTag("Floor") ||
                collision.gameObject.CompareTag("Ramp"))
            {
                var buildingHealth = collision.gameObject.GetComponent<BuildingHealth>();
                if (buildingHealth != null)
                {
                    buildingHealth.OnBulletHit(bulletDamage);
                }
            }
            Destroy(gameObject);
        }
    }
}
