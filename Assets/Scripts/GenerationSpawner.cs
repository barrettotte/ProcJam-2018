using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerationSpawner : MonoBehaviour {

	[SerializeField] public float randPosRange;
	ObjectPool objectPool;
	private int currentSize;
	private long currentGeneration;
	private List<GameObject> organisms;
	private List<GameObject> matingPool;
	private GameObject[] waypoints;
	private SimulationManager simManager;
	private bool doneGenProcessing;
	private Organism bestOrganism;

	//UI:
	[SerializeField] private Text genLabel;
	[SerializeField] private Text bestOrgName;
	[SerializeField] private Text bestOrgId;
	[SerializeField] private Text bestOrgColor;
	[SerializeField] private Text bestOrgFitness;
	[SerializeField] private Text bestOrgParents;
	[SerializeField] private GameObject generationButton;
	[SerializeField] private GameObject resetButton;
	


	private void Start(){
		objectPool = ObjectPool.Instance;
		waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		simManager = SimulationManager.Instance;
		Reset();
		MakeInitPopulation();
	}

	private void FixedUpdate() {
		if(currentGeneration == 0 && currentSize <= simManager.generationSize){
			MakeInitPopulation();
		}	
	}

	public void Reset(){
		currentGeneration = 0;
		doneGenProcessing = false;
		currentSize = 0;
		bestOrganism = null;
		generationButton.SetActive(false);
		resetButton.SetActive(false);

		if(organisms != null){
			for(int i = 0; i < organisms.Count; i++){
				organisms[i].SetActive(false);
			}
		}
		organisms = new List<GameObject>();
		UpdateUI();
	}

	private void MakeInitPopulation(){
		if(currentSize < simManager.generationSize){
			GameObject obj = SpawnOrganism();
			obj.AddComponent<Organism>();
			SetupOrganism(obj.GetComponent<Organism>());
			organisms.Add(obj);
			obj.GetComponent<Organism>().InitOrganism();
			currentSize++;
		} else if(currentSize == simManager.generationSize && !doneGenProcessing){
			organisms = PopGen.SortByFitness(organisms);
			organisms = PopGen.SetMatingChances(organisms, simManager.generationSize);
			doneGenProcessing = true;
			UpdateBestOrganism();
			matingPool = PopGen.GenerateMatingPool(organisms);
			generationButton.SetActive(true);
			resetButton.SetActive(true);
		}
	}

	private void UpdateBestOrganism(){
		Organism top = organisms[organisms.Count-1].GetComponent<Organism>();
		if(bestOrganism == null || top.Id != bestOrganism.Id){
			bestOrganism = top;
			UpdateUI();
		}
	}

	public void MakeNewGeneration(){
		currentGeneration++;
		currentSize = 0;
		int poolSize = matingPool.Count;
		List<GameObject> nextGeneration = new List<GameObject>();
		for(int i = 0; i < simManager.generationSize; i++){
			GameObject parent1 = matingPool[Random.Range(0, poolSize-1)];
			GameObject parent2 = matingPool[Random.Range(0, poolSize-1)];
			nextGeneration.Add(organisms[i]);

			if(parent1.GetComponent<Organism>().HasTopMarker){
				parent2 = parent1;
			} else if(parent2.GetComponent<Organism>().HasTopMarker){
				parent1 = parent2;
			}
			GameObject offspring = PopGen.Reproduce(parent1, parent2, currentGeneration, i);
			nextGeneration[i].GetComponent<Organism>().SetNewOrganism(offspring.GetComponent<Organism>());
			Destroy(offspring);
		}
		organisms = PopGen.SortByFitness(nextGeneration);
		organisms = PopGen.SetMatingChances(organisms, simManager.generationSize);
		UpdateBestOrganism();
		matingPool = PopGen.GenerateMatingPool(organisms);
	}

	//Instatiate Organism in game world from spawn pool
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

	//Generate organisms "DNA" and other attributes
	private Organism SetupOrganism(Organism organism){
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

	private void UpdateUI(){
		genLabel.text = "Current Generation: " + currentGeneration.ToString().PadLeft(5, '0');
		bestOrgName.text = "Name: ";
		bestOrgId.text = "ID: ";
		bestOrgColor.text = "Color Allele: ";
		bestOrgFitness.text = "Fitness: ";
		bestOrgParents.text = "Parents: {";
		if(bestOrganism != null){
			bestOrgName.text += bestOrganism.name;
			bestOrgId.text += bestOrganism.Id;
			bestOrgColor.text += PopGen.AlleleToString(bestOrganism.DNA["color"]);
			bestOrgFitness.text += bestOrganism.Fitness.ToString();
			bestOrgParents.text += bestOrganism.Parents[0] + ", " + bestOrganism.Parents[1];
		}
		bestOrgParents.text += "}";
	}
}
