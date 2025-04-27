using System.Collections;
using UnityEngine;

public class Bomba : MonoBehaviour
{
    [SerializeField] private float timeToExplode = 3f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private int maxDamage = 100;
    [SerializeField] private GameObject[] explosionEffect;
    [SerializeField] private Renderer bombRenderer;

    private readonly Color _color1 = Color.red;
    private readonly Color _color2 = Color.black;

    void Start()
    {
        StartCoroutine(BlinkAndExplode());
    }

    private IEnumerator BlinkAndExplode()
    {
        var elapsedTime = 0f;
        var blinkInterval = 0.5f;

        while (elapsedTime < timeToExplode)
        {
            bombRenderer.material.color = bombRenderer.material.color == _color1 ? _color2 : _color1;

            yield return new WaitForSeconds(blinkInterval);

            blinkInterval = Mathf.Max(0.05f, blinkInterval - 0.05f);

            elapsedTime += blinkInterval;
        }

        Explode();
    }

    private void Explode()
    {
        if (explosionEffect.Length > 0)
        {
            var randomIndex = Random.Range(0, explosionEffect.Length);
            var selectedExplosion = explosionEffect[randomIndex];

            Instantiate(selectedExplosion, transform.position, Quaternion.identity);
        }

        var colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var nearbyObject in colliders)
        {
            var distance = Vector3.Distance(transform.position, nearbyObject.transform.position);
            var damage = Mathf.Max(0, maxDamage * (1 - (distance / explosionRadius)));

            var building = nearbyObject.GetComponent<BuildingHealth>();
            if (building != null)
            {
                building.OnBulletHit((int)damage);
            }

            var bot = nearbyObject.GetComponentInParent<Bot>();
            if (bot != null)
            {
                bot.OnBulletHit((int)damage);
            }

            var rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
