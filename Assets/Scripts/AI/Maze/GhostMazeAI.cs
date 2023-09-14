using System;
using Managers;
using Service;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class GhostMazeAI : MonoBehaviour
{
    public Transform gem;
    public Transform player; 
    public float patrolSpeed = 2f; 
    public float visionRange = 5f; 
    private Vector3 initialPosition; 
    private bool isChasing = false;
    private Vector3 patrolDestination;
    private bool isConfused;
    private MazeGenerator mazeGenerator;
    private bool _canStart;

    private void OnDisable()
    {
        mazeGenerator.OnMazeGenerated -= OnMazeGenerated;
    }

    private void Start()
    {
        mazeGenerator = FindObjectOfType<MazeGenerator>();
        if (mazeGenerator != null)
        {
            mazeGenerator.OnMazeGenerated += OnMazeGenerated;
        }
      
    }

    private void OnMazeGenerated()
    {
        ServiceLocator.GetService<MyLevelManager>().StartLevel();
        CanStart();
    }

    private void Update()
    {
        if (!_canStart) return;
        
        if (!isChasing)
        {
            Patrol();
        }
        else if (isConfused)
        {
            transform.Rotate(Vector3.up * 360f * Time.deltaTime);
        }
        else
        {
            ChasePlayer();
        }
    }

    private void CanStart()
    {
        player = GameObject.FindGameObjectWithTag(Constants.Player).GetComponent<Transform>();
        initialPosition = new Vector3(mazeGenerator.Width, 0f, mazeGenerator.Height);
        transform.position = initialPosition;
        SetRandomPatrolDestination();
        _canStart = true;
    }
    
    private void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, patrolDestination, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.position) <= visionRange)
        {
            isChasing = true;
            //Debug.Log("¡El fantasma ha detectado al jugador y lo está persiguiendo!");
        }
        
        if (Vector3.Distance(transform.position, patrolDestination) < 0.1f)
        {
            SetRandomPatrolDestination();
        }
        
        transform.LookAt(patrolDestination);
    }

    private void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.position) > visionRange)
        {
            isChasing = false;
            //Debug.Log("El jugador ha escapado del rango de visión del fantasma");
        }
        
        transform.LookAt(player.position);
        
        /*if (BoyController.Instance.IsDefending)
        {
            isConfused = true;
            Invoke("ResumePatrol", 2f);
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
       /* if (other.CompareTag("Player") && !isReset && !BoyController.Instance.IsDefending)
        {
            Debug.Log("Entro");
            isReset = true;
            ResetGame();
        }*/
    }

    private void ResetGame()
    {
        SetRandomPatrolDestination();
        ServiceLocator.GetService<MyGameManager>().GameOver();
    }

    private void SetRandomPatrolDestination()
    {
        patrolDestination = new Vector3(Random.Range(0f, mazeGenerator.Width), 0f, Random.Range(0f, mazeGenerator.Height));
    }
    
    
    private void ResumePatrol()
    {
        isConfused = false;
        SetRandomPatrolDestination();
    }
}
