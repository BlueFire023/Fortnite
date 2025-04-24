using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTimeSeconds = 10f;
    public void Update()
    {
        Destroy(gameObject, lifeTimeSeconds);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("IgnoreBullet"))
        {
            Debug.Log("Destroying");
            Destroy(gameObject);
        }
    }
}
