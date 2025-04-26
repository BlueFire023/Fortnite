using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private float idleRadius;
    [SerializeField] private float aggroRadius;
    [SerializeField] private float shootingRadius;

    [SerializeField] private bool aggro;
    [SerializeField] private GameObject alertObj;

    [SerializeField] private Vector3 targetPosition;

    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float timeBetweenShots = 0.2f;
    [SerializeField] private float shootForce = 100f;
    private float nextShootTime;

    private NavMeshAgent _self;
    private GameObject _player;

    public void Start()
    {
        _self = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");

        spawnPosition = transform.position;
        GenerateIdlePosition();
    }

    public void Update()
    {
        if (!_self.pathPending && _self.remainingDistance <= _self.stoppingDistance && !aggro)
        {
            GenerateIdlePosition();
        }
        alertObj.SetActive(aggro);
        IsPlayerInAggroRadius();
        IsPlayerInShootingRadius();
    }

    private void GenerateIdlePosition()
    {
        var randomOffset = Random.insideUnitSphere * idleRadius;
        randomOffset.y = 0;
        targetPosition = spawnPosition + randomOffset;

        _self.SetDestination(targetPosition);
    }

    private void IsPlayerInAggroRadius()
    {
        var distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
        if (distanceToPlayer <= aggroRadius)
        {
            aggro = true;
            targetPosition = _player.transform.position;
            _self.SetDestination(targetPosition);
        }
        else
        {
            aggro = false;
        }
    }

    private void IsPlayerInShootingRadius()
    {
        var distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
        if (distanceToPlayer <= shootingRadius)
        {
            gun.transform.LookAt(new Vector3(_player.transform.position.x, _player.transform.position.y + 0.5f,
                _player.transform.position.z));
            Shoot();
        }
    }

    private void Shoot()
    {
        if (Time.time < nextShootTime)
            return;

        var projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.Euler(
            projectilePrefab.transform.rotation.eulerAngles.x,
            shootingPoint.rotation.eulerAngles.y,
            shootingPoint.rotation.eulerAngles.z
        ));
        var rb = projectile.GetComponent<Rigidbody>();

        rb.AddForce(shootingPoint.forward * shootForce, ForceMode.Impulse);
        nextShootTime = Time.time + timeBetweenShots;
    }
}
