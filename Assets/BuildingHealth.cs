using System;
using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    public int healthPoints = 100;
    private BookScript bookScript;
    public void SetBookScript(BookScript book)
    {
        bookScript = book;
    }

    public void OnBulletHit(int bulletDamage)
    {
        healthPoints -= bulletDamage;

        if (healthPoints <= 0)
        {
            bookScript.RemoveBuilding(gameObject);
            Destroy(gameObject);
        }
    }
}
