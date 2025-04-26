using System;
using Unity.AI.Navigation;
using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    public int healthPoints = 100;
    private BookScript _bookScript;
    public void SetBookScript(BookScript book)
    {
        _bookScript = book;
    }

    public void OnBulletHit(int bulletDamage) 
    {
        healthPoints -= bulletDamage;

        if (healthPoints <= 0)
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            _bookScript.RemoveBuilding(gameObject);
            Destroy(gameObject);
        }
    }
}
