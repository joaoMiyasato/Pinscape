using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyUI : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent enemy;
    private Transform player;
    public LayerMask whatIsPlayer;

    public bool playerInRange;
    public float attackRange;

    public Vector3 point1;
    public Vector3 point2;
    public Vector3 startPoint;

    public GameObject patrolArea;

    void Start()
    {
        enemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;


        point1 = new Vector3 (transform.position.x + 5, transform.position.y, transform.position.y + 2);
        point2 = new Vector3 (transform.position.x - 5, transform.position.y, transform.position.y - 2);
        startPoint = transform.position;
        
        //AleatoryWalk();
        
        //Debug.Log(point1);
        //inimigo = Inimigo.parado;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(patrolArea.GetComponent<PatrolArea>().onPatrolArea);


        //if(!playerInRange && inimigo == Inimigo.atacando) inimigo = Inimigo.voltando; AleatoryWalk();



        if(patrolArea.GetComponent<PatrolArea>().onPatrolArea == true)
        {
            enemy.SetDestination(player.position);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
         if(other.transform.tag == "Player")
        {
            Destroy(other.gameObject);
            Invoke(nameof(GameOver), 1.5f); 
        }
    }

    void GameOver()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
