using UnityEngine;
using UnityEngine.AI;

namespace Assets
{
    public class Target : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] private NavMeshAgent _agent;
        public void Update()
        {
            _agent.SetDestination(transform.position);
        }
    }
}
