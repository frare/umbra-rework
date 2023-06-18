using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float spawnTimeMin;
    [SerializeField] private float spawnTimeMax;
    [SerializeField] private float spawnDistance;
    [SerializeField, ReadOnly] private bool canSpawn; 
    [SerializeField] private Enemy enemy;

    private float spawnCurrentTime;
    private float spawnTargetTime;



    private void Awake()
    {
        spawnCurrentTime = 0f;
        spawnTargetTime = Random.Range(spawnTimeMin, spawnTimeMax);
        canSpawn = false;
        enemy.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (canSpawn && !enemy.gameObject.activeSelf)
        {
            if (spawnCurrentTime < spawnTargetTime) { spawnCurrentTime += Time.deltaTime; }
            else { Spawn(); }
        }
    }

    [ContextMenu("Spawn")]
    private void Spawn()
    {
        spawnCurrentTime = 0f;
        spawnTargetTime = Random.Range(spawnTimeMin, spawnTimeMax);
        canSpawn = false;

        Vector3 randomDirection = Random.insideUnitSphere * spawnDistance;
        randomDirection += Player.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, spawnDistance, 1);

        enemy.transform.position = hit.position;
        enemy.gameObject.SetActive(true);
    }
}