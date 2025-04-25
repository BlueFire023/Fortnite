using System;
using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    public int healthPoints = 100;

    public void OnBulletHit(int bulletDamage)
    {
        Debug.Log($"Hit {gameObject.name} with {bulletDamage} damage.");
        healthPoints -= bulletDamage;

        if (healthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
