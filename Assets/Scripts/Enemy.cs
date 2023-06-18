using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float minimumDangerDistance;

    [SerializeField] private NavMeshAgent navMeshAgent;



    private void Update()
    {
        navMeshAgent.destination = Player.position;

        if (Vector3.Distance(transform.position, Player.position) <= minimumDangerDistance)
        {
            
        }
    }
}
