using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationSpawner : MonoBehaviour {

	[SerializeField]
	public float randPosRange;
	
	ObjectPool objectPool;
	private int currentSize;
	private int currentGeneration;
	private List<GameObject> organisms;
	private List<Organism> matingPool;
	private GameObject[] waypoints;

	private SimulationManager simManager;
	private bool doneGenProcessing;
	[SerializeField] //TODO: DEBVUG
	private bool idealFound;
	[SerializeField]//TODO:DEBUG
	private List<string> bestOrganism;//TODO:DEBUG
	


	private void Start(){
		currentGeneration = 0;
		doneGenProcessing = false;
		objectPool = ObjectPool.Instance;
		waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		organisms = new List<GameObject>();
		simManager = SimulationManager.Instance;
		bestOrganism = new List<string>(); //TODO:DEBUG
	}

	private void FixedUpdate(){	
		if(currentGeneration == 0){
			MakeInitPopulation();
		} else{

		}
	}

	public void StartNewGeneration(){
		currentGeneration++;
		currentSize = 0;
	}

	private void MakeInitPopulation(){
		if(currentSize < simManager.generationSize){
			GameObject obj = SpawnOrganism();
			MakeOrganism(obj.GetComponent<Organism>());
			organisms.Add(obj);
			obj.GetComponent<Organism>().InitOrganism();
			currentSize++;
		} else if(currentSize == simManager.generationSize && !doneGenProcessing){
			/*organisms = PopGen.SortByFitness(organisms);
			for(int i = 0; i < simManager.generationSize; i++){
				organisms = PopGen.SetMatingChances(organisms, simManager.generationSize);
			}
			doneGenProcessing = true;
			idealFound = PopGen.CheckForIdeal(organisms);
			bestOrganism.Add("Name: " + organisms[organisms.Count-1].name);//TODO:DEBUG
			bestOrganism.Add("Fitness: " + organisms[organisms.Count-1].Fitness.ToString());//TODO:DEBUG
			matingPool = PopGen.GenerateMatingPool(organisms);*/
		}
	}

	private void MakeNewGeneration(){
		int poolSize = matingPool.Count;
		List<Organism> nextGeneration = new List<Organism>();
		for(int i = 0; i < simManager.generationSize; i++){
			Organism parent1 = matingPool[Random.Range(0, poolSize-1)];
			Organism parent2 = matingPool[Random.Range(0, poolSize-1)];
			
		}
	}

	private GameObject SpawnOrganism(){
		Vector3 randPos = new Vector3(
			Random.Range(-randPosRange, randPosRange),
			0f,
			Random.Range(-randPosRange, randPosRange)
		);
		Quaternion randRot = new Quaternion(
			Random.Range(0f,360f),
			Random.Range(0f,360f), 
			Random.Range(0f,360f), 
			Random.Range(0f,360f)
		);
		return objectPool.SpawnFromPool("Organism", this.transform.position + randPos, randRot);
	}

	private Organism MakeOrganism(Organism organism){
		organism.Waypoints = waypoints;
		organism.DestinationThreshold = simManager.destinationThreshold;
		organism.MoveSpeed = simManager.moveSpeed;
		organism.WanderWaitTime = simManager.wanderWaitTime;
		
		organism.Generation = currentGeneration;
		organism.Id = PopGen.MakeId(currentGeneration, currentSize);
		organism.name = PopGen.MakeOrganismName();
		organism.DNA = PopGen.MakeOrganismDNA();

		Renderer rend = organism.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Standard"));
        rend.material.color = PopGen.ColorAlleleToColor(organism.DNA["color"]);

		organism.Fitness = PopGen.EvaluateFitness("color", organism.DNA["color"]);
		organism.Parents = new string[] {"?????-?????", "?????-?????"};
		return organism;
	}
}
