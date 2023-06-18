using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;
    public static int layer { get { return 13;} }
    public static Vector3 position { get { return instance.enemy.transform.position; } }

    [SerializeField] private float[] spawnTime;
    [SerializeField] private float[] despawnTime;
    [SerializeField] private float spawnTimeRandomModifier;
    [SerializeField] private float spawnDistance;
    [SerializeField, ReadOnly] private bool canSpawn;
    [SerializeField] private Enemy enemy;
    private float spawnCurrentTime;
    private float spawnTargetTime;
    private Coroutine despawnCoroutine;



    private void Awake()
    {
        EnemyManager.instance = this;

        spawnCurrentTime = 0f;
        canSpawn = false;
    }

    private void Start()
    {
        enemy.onFade += () => { canSpawn = true; };
        LevelManager.onPageCollected += () => { if (LevelManager.pagesCollected == 1) Spawn(); };
        FindObjectOfType<NightVision>().onEnemyCaught += () => { enemy.FadeOut(); };
    }

    private void Update()
    {
        if (canSpawn)
        {
            if (spawnCurrentTime < spawnTargetTime) { spawnCurrentTime += Time.deltaTime; }
            else { Spawn(); }
        }
    }

    [ContextMenu("Spawn")]
    private void Spawn()
    {
        spawnCurrentTime = 0f;
        spawnTargetTime = GenerateRandomSpawnTime();

        Vector3 randomDirection = Random.insideUnitSphere.normalized * spawnDistance;
        randomDirection += Player.position;
        NavMeshHit hit;
        while (NavMesh.SamplePosition(randomDirection, out hit, spawnDistance, 1) == false);

        enemy.transform.position = hit.position;
        enemy.gameObject.SetActive(true);

        if (despawnCoroutine != null) StopCoroutine(despawnCoroutine);
        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime[LevelManager.pagesCollected]);

        enemy.FadeOut();
    }

    private float GenerateRandomSpawnTime()
    {
        return Random.Range(
            spawnTime[LevelManager.pagesCollected] - spawnTimeRandomModifier, 
            spawnTime[LevelManager.pagesCollected] + spawnTimeRandomModifier
        );
    }
}