using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Organism : MonoBehaviour{

    [SerializeField]
    private string id;
    [SerializeField] //TODO: DEBUG
    private string organismName;
    [SerializeField] //TODO: DEBUG
    private int fitness;
    [SerializeField] //TODO: DEBUG
    private int generation;
    [SerializeField]
    private int matingChance;
    [SerializeField] //TODO: DEBUG
    private string[] colorAllele;   
    [SerializeField] //TODO: DEBUG
    private float moveSpeed;
    private string[] parents = new string[2];

    private bool isAlive;
    private bool canWander;
    private float destinationThreshold;
    private NavMeshAgent navAgent;
    private float wanderWaitTime;
    private GameObject[] waypoints;
    private int currentWaypoint;
    private string destination;
    private Dictionary<string, string[]> dna;
    private bool canAsexReproduce;


#region Properties
    public bool CanWander {
        get{ return this.canWander;  }
        set{ this.canWander = value; }
    }
    public string Id {
        get{ return this.id;  }
        set{ this.id = value; }
    }
    public int Generation {
        get{ return this.generation;  }
        set{ this.generation = value; }
    }
    public GameObject[] Waypoints {
        get{ return this.waypoints;  }
        set{ this.waypoints = value; }
    }
    public float DestinationThreshold {
        get{ return this.destinationThreshold;  }
        set{ this.destinationThreshold = value; }
    }
    public Dictionary<string, string[]> DNA {
        get{ return this.dna;  }
        set{ this.dna = value; }
    }
    public int Fitness {
        get{ return this.fitness;  }
        set{ this.fitness = value; }
    }
    public float MoveSpeed {
        get{ return this.moveSpeed;  }
        set{ this.moveSpeed = value; }
    }
    public float WanderWaitTime {
        get{ return this.wanderWaitTime;  }
        set{ this.wanderWaitTime = value; }
    }
    public int MatingChance {
        get{ return this.matingChance;  }
        set{ this.matingChance = value; }
    }
    public string[] Parents {
        get{ return this.parents;  }
        set{ this.parents = value; }
    }
    public bool CanAsexReproduce {
        get{ return this.canAsexReproduce;  }
        set{ this.canAsexReproduce = value; }
    }
#endregion


    private void Update() {
        if(isAlive && canWander){
            Wander();
        }    
    }

    public void InitOrganism(){
        isAlive = true;
        canWander = false;
        canAsexReproduce = false;
        this.transform.eulerAngles = new Vector3(0.0f, UnityEngine.Random.Range(0f, 360f), 0f);
        StartCoroutine(StartWanderDelay());
        organismName = this.name;
        colorAllele = DNA["color"]; //TODO: DEBUG 
    }

    private void Wander(){
        bool needsWaypoint = (
            (navAgent.enabled && !navAgent.hasPath) || 
            (navAgent.remainingDistance < destinationThreshold)
        );
        if(navAgent.isOnNavMesh && needsWaypoint){
            SetWaypoint();
        }
    }

    private void SetWaypoint(){
        if(navAgent.isOnNavMesh){
            currentWaypoint = UnityEngine.Random.Range(0, waypoints.Length);
            navAgent.SetDestination(waypoints[currentWaypoint].transform.position);
            destination = waypoints[currentWaypoint].name;
        }
    }

    IEnumerator StartWanderDelay(){
        yield return new WaitForSeconds(wanderWaitTime);
        canWander = true;
        navAgent = this.GetComponent<NavMeshAgent>();
        navAgent.enabled = true;
        navAgent.speed = moveSpeed;
        SetWaypoint();
    }

    private void OnTriggerEnter(Collider other) {
        if(canWander && other.tag == "Waypoint" && other.name == destination){
            SetWaypoint();
        }
    }
}