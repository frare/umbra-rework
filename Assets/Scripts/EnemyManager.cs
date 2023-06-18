using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Enemy enemy;



    private void Spawn(Vector3 position)
    {
        enemy.transform.position = position;
        enemy.gameObject.SetActive(true);
    }
}