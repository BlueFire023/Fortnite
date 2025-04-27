using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 300f;
    [SerializeField] private GameObject explosion;
    [SerializeField] private TextMeshProUGUI healthText;
    private bool _isDead;

    private void Start()
    {
        healthText.text = "Health: " + hitPoints.ToString();
    }

    public void OnBulletHit(int damage)
    {
        if (!_isDead)
        {
            hitPoints -= damage;
            if(hitPoints < 0)
            {
                hitPoints = 0;
            }
            healthText.text = "Health: " + hitPoints.ToString();
            Debug.Log("Getting hit");
        }
        if (hitPoints <= 0 && !_isDead)
        {
            Debug.Log("Player is dead");
            _isDead = true;
            explosion.transform.localScale = new Vector3(5f, 5f, 5f);
            Instantiate(explosion, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
            GetComponentInChildren<PlayerInput>().enabled = false;
            GetComponentInChildren<CapsuleCollider>().enabled = false;
        }
    }
}
