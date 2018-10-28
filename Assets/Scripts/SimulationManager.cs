using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour {

	public static SimulationManager Instance;
	public GenerationSpawner spawner;

	public int destinationThreshold;
	public float moveSpeed;
	public float wanderWaitTime;
	public int[] fitnessWeights = new int[5];

	public int generationSize;
	public float mutationChance;
	public float asexReproChance;
	public float asexTopOrganisms;
	public int idealError;
	public string[] idealColorAllele;

	[SerializeField] private Slider mutationSlider;
	[SerializeField] private Text mutationSliderVal;
	[SerializeField] private Slider asexSlider;
	[SerializeField] private Text asexSliderVal;

	[SerializeField] private Image colorPreview;
	[SerializeField] private Slider redSlider;
	[SerializeField] private Text redSliderVal;
	[SerializeField] private Slider greenSlider;
	[SerializeField] private Text greenSliderVal;
	[SerializeField] private Slider blueSlider;
	[SerializeField] private Text blueSliderVal;

	private void Awake(){
		Instance = this;
	}

	private void Start(){
		mutationSlider.value = mutationChance;
		asexSlider.value = asexReproChance;
		for(int i = 0; i < idealColorAllele.Length; i++){
			idealColorAllele[i] = "00000000";
		}
		redSlider.value = PopGen.BinaryToDecimal(idealColorAllele[0]);
		greenSlider.value = PopGen.BinaryToDecimal(idealColorAllele[1]);
		blueSlider.value = PopGen.BinaryToDecimal(idealColorAllele[2]);
		redSliderVal.text = redSlider.value.ToString();
		greenSliderVal.text = greenSlider.value.ToString();
		blueSliderVal.text = blueSlider.value.ToString();
		UpdateColorAllele();
	}

	public void UpdateMutationChance(){
		mutationSliderVal.text = mutationSlider.value.ToString("0.0000");
		mutationChance = mutationSlider.value;
	}

	public void UpdateAsexReproChance(){
		asexSliderVal.text = asexSlider.value.ToString("0.0000");
		asexReproChance = asexSlider.value;
	}

	public void UpdateRed(){
		redSliderVal.text = redSlider.value.ToString();
	}
	public void UpdateBlue(){
		blueSliderVal.text = blueSlider.value.ToString();
	}
	public void UpdateGreen(){
		greenSliderVal.text = greenSlider.value.ToString();
	}
	public void UpdateColorAllele(){
		float red = redSlider.value;
		float green = greenSlider.value;
		float blue = blueSlider.value;
		
		colorPreview.color = new Color(red/255f,green/255f,blue/255f,255f);
		idealColorAllele[0] = PopGen.DecimalToBinary((int)red);
		idealColorAllele[1] = PopGen.DecimalToBinary((int)green);
		idealColorAllele[2] = PopGen.DecimalToBinary((int)blue);
	}
}
