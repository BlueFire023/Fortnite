using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Bot : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPosition;
    private bool _isWaiting;
    private bool _isDead;
    [SerializeField] private float idleTime;
    [SerializeField] private float idleRadius;
    [SerializeField] private float aggroRadius;
    [SerializeField] private float shootingRadius;

    [SerializeField] private bool aggro;

    [SerializeField] private Vector3 targetPosition;

    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float timeBetweenShots = 0.2f;
    [SerializeField] private float shootForce = 100f;
    private float _nextShootTime;

    private NavMeshAgent _self;
    private GameObject _player;

    [SerializeField] private BotAnimator botAnimator;

    [SerializeField] private float healthPoints = 100;

    public void Start()
    {
        _self = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");

        spawnPosition = transform.position;
        botAnimator.SetAnimation(BotAnimator.AnimationState.Idle);
        _self.SetDestination(spawnPosition);
    }

    public void Update()
    {
        if (_isDead)
            return;
        if (!_self.pathPending && _self.remainingDistance <= _self.stoppingDistance && !aggro && !_isWaiting)
        {
            StartIdleWait();
        }
        IsPlayerInAggroRadius();
        IsPlayerInShootingRadius();
    }

    private void StartIdleWait()
    {
        _isWaiting = true;
        botAnimator.SetAnimation(BotAnimator.AnimationState.Idle);

        Invoke(nameof(GenerateIdlePosition), idleTime);
    }

    private void GenerateIdlePosition()
    {
        _isWaiting = false;
        var randomOffset = Random.insideUnitSphere * idleRadius;
        randomOffset.y = 0;
        targetPosition = spawnPosition + randomOffset;

        _self.SetDestination(targetPosition);
        botAnimator.SetAnimation(BotAnimator.AnimationState.Walking);
    }

    private void IsPlayerInAggroRadius()
    {
        var distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
        if (distanceToPlayer <= aggroRadius)
        {
            aggro = true;
            targetPosition = _player.transform.position;
            _self.SetDestination(targetPosition);
            botAnimator.SetAnimation(BotAnimator.AnimationState.Walking);
            _isWaiting = false;
            CancelInvoke(nameof(GenerateIdlePosition));
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
            //gun.transform.LookAt(new Vector3(_player.transform.position.x, _player.transform.position.y + 0.5f, _player.transform.position.z));
            var direction = _player.transform.position - transform.position;

            direction.y = 0;

            if (direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
            Shoot();
            botAnimator.SetAnimation(
                distanceToPlayer <= _self.stoppingDistance
                    ? BotAnimator.AnimationState.Firing
                    : BotAnimator.AnimationState.WalkFire);
        }
    }

    private void Shoot()
    {
        if (Time.time < _nextShootTime)
            return;

        var projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.Euler(
            projectilePrefab.transform.rotation.eulerAngles.x,
            shootingPoint.rotation.eulerAngles.y,
            shootingPoint.rotation.eulerAngles.z
        ));
        projectile.GetComponent<Bullet>().SetOrigin();
        var rb = projectile.GetComponent<Rigidbody>();

        rb.AddForce(shootingPoint.forward * shootForce, ForceMode.Impulse);
        _nextShootTime = Time.time + timeBetweenShots;
    }

    public void OnBulletHit(int bulletDamage)
    {
        healthPoints -= bulletDamage;

        if (healthPoints <= 0)
        {
            _isDead = true;
            botAnimator.SetAnimation(BotAnimator.AnimationState.Death);
            GetComponentInChildren<CapsuleCollider>().enabled = false;
            _self.isStopped = true;
            _self.enabled = false;
            StartCoroutine(FallOver());
        }
    }

    private IEnumerator FallOver()
    {
        yield return new WaitForSeconds(0.4f);
        var duration = 0.5f;
        var elapsedTime = 0.0f;

        var startPosition = transform.position;
        var endPosition = new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z);

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRadius);
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }
}
