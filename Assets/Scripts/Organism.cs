using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Organism : MonoBehaviour{

    private string id;
    private int fitness;
    private long generation;
    private int matingChance;   
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
    private bool hasTopMarker;


#region Properties
    public bool CanWander {
        get{ return this.canWander;  }
        set{ this.canWander = value; }
    }
    public string Id {
        get{ return this.id;  }
        set{ this.id = value; }
    }
    public long Generation {
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
    public bool HasTopMarker {
        get{ return this.hasTopMarker;  }
        set{ this.hasTopMarker = value; }
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
        hasTopMarker = false;
        this.transform.eulerAngles = new Vector3(0.0f, UnityEngine.Random.Range(0f, 360f), 0f);
        StartCoroutine(StartWanderDelay());
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

    public void SetNewOrganism(Organism other){
        this.generation = other.Generation;
		this.id = other.Id;
		this.name = other.name;
		this.DNA = other.DNA;
		Renderer rend = GetComponent<Renderer>();
        rend.material.color = PopGen.ColorAlleleToColor(dna["color"]);
		fitness = other.fitness;
		parents = other.parents; 
    }
}