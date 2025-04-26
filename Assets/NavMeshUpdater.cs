using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUpdater : MonoBehaviour
{
    private NavMeshSurface navSurface;
    public void Awake()
    {
        navSurface = GetComponent<NavMeshSurface>();
    }

    public void UpdateNavMesh()
    {
        navSurface.BuildNavMesh();
    }
}
