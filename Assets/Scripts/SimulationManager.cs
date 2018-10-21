using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {

	public static SimulationManager Instance;

	public int destinationThreshold;
	public float moveSpeed;
	public float wanderWaitTime;
	public int[] fitnessWeights = new int[5];

	public int generationSize;
	public float mutationChance;
	public float mysteryDnaChance;
	public float asexReproChance;
	public float asexTopOrganisms;
	public int idealError;
	public string[] idealColorAllele = new string[3];

	public GenerationSpawner spawner;

	//TODO: Make gets/sets
	//  generationSize >= 50


	private void Awake(){
		Instance = this;
	}


	private void Update(){
		//TODO: spawner.StartNewGeneration(); //if time threshold passed
	}


}
