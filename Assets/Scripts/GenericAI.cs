using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class neurone {
	// Un neurone avec plusieurs entrées
	static float weightMax=25f;
	static float weightMin=-25f;
	static public int mutationRate=1000;
	
	private float[] weight;
	private float totalWeight;
	private int inputSize;
	
	// Constructeurs
	public neurone(int inputSize) {
		this.inputSize = inputSize;
		weight = new float[inputSize];
		// On affecte des valeurs aléatoires
		for (int i = 0; i < inputSize; i++)
			this.weight[i]=Random.Range(weightMin,weightMax);
		computeTotalWeight();
	}
	
	public neurone(float[] weight) {
		this.inputSize = weight.Length;
		this.weight = new float[inputSize];
		for (int i = 0; i < inputSize; i++)
			this.weight[i]=weight[i];
		computeTotalWeight();
	}
	
	private void computeTotalWeight() {
		this.totalWeight=0f;
		for (int i = 0; i < inputSize; i++)
			this.totalWeight+=this.weight[i];
	}
	
	// Getters
	
	public float getOutput(float[] input){
		if (input.Length != inputSize) {
			Debug.LogError ("La taille de l'entrée ne correspond pas à ce qui est attendu [neurone]. Attendu: "+inputSize+" au lieu de "+input.Length);
			return 0f;
		}
		float total=0f;
		for (int i = 0; i < inputSize; i++)
			total+= input[i]*this.weight[i];
		return total/this.totalWeight;
	}
	
	public float[] getWeights() {
		return this.weight;
	}
	
	public static neurone weightedAverage(neurone[] input, float[] averageWeight) {
		// On considère que tous les neurones ont la même taille
		int size = input[0].inputSize;
		// Calcul du poids total pour la moyenne
		float totalAverageWeight = 0f;
		int arrayLen=averageWeight.Length;
		for (int i = 0; i < arrayLen; i++)
			totalAverageWeight+= averageWeight[i];
		
		float[] newWeights = new float[size];
		
		// On calcule la moyenne de chaque entrée
		for (int iInput = 0; iInput < size; iInput++) {
			float totalWeight=0f;
			for (int iNeurone = 0; iNeurone < arrayLen; iNeurone++)
				totalWeight+=input[iNeurone].weight[iInput];
			newWeights[iInput]=totalWeight/totalAverageWeight;
			// Mutation
			if (Random.Range(0,mutationRate)==0) {
				newWeights[iInput] = Random.Range(weightMin,weightMax);
				Debug.Log("Mutation");
			}
		}
		
		return new neurone(newWeights);
	}
	
}

public class geneLayer {
	private int inputSize;
	private int outputSize;
	private neurone[] content;
	
	// Constructeurs
	
	public geneLayer(int inputSize, int outputSize) {
		// On crée des neurones aléatoires
		this.content=new neurone[outputSize];
		this.outputSize=outputSize;
		this.inputSize=inputSize;
		
		for (int i = 0; i < outputSize; i++)
			this.content[i] = new neurone(inputSize);
	}
	
	public geneLayer(neurone[] neurones) {
		this.outputSize = neurones.Length;
		this.inputSize = neurones[0].getWeights().Length; // On considère que tous les neurones ont la même taille, vérifier demanderait plus de ressources.
		this.content = neurones;
	}
	
	// Getters
	
	public neurone[] getNeurones() {
		return content;
	}
	
	public float[] getOutput(float[] input) {
		float[] output = new float[outputSize];
		for (int i = 0; i < outputSize; i++)
			output[i] = content[i].getOutput(input);
		return output;
	}
	
	public int getOutputSize() {
		return this.outputSize;
	}
	
	public int getInputSize() {
		return this.inputSize;
	}
	
	
	public static geneLayer weightedAverage (geneLayer[] layers, float[] weights) {
		// On considère que toutes les couches ont la même taille
		int outputSizeLayer=layers[0].outputSize;
		int layersNb = weights.Length;
		
		neurone[] newNeurones = new neurone[outputSizeLayer];
		for (int iNeurone = 0; iNeurone<outputSizeLayer ; iNeurone++){
			neurone[] toAverage = new neurone[layersNb];
			for (int iLayer = 0; iLayer < layersNb; iLayer++) {
				toAverage[iLayer]=layers[iLayer].content[iNeurone];
			}
			newNeurones[iNeurone]=neurone.weightedAverage(toAverage, weights);
		}
		return new geneLayer(newNeurones);
	}
	
}

public class gene {
	private int[] sizes;
	private geneLayer[] layers;
	private int inputSize;
	private int outputSize;
	private int layersNumber;
	
	// Constructeurs
	public gene(int[] sizes) {
		this.sizes = sizes;
		this.layersNumber = sizes.Length-1;
		this.inputSize = sizes[0];
		this.outputSize = sizes[layersNumber];
		this.layers = new geneLayer[layersNumber];
		// On génère des couches aléatoires
		for (int i = 0; i < layersNumber; i++)
			layers[i] = new geneLayer(sizes[i], sizes[i+1]);
	}
	
	public gene(geneLayer[] layers) {
		this.layers = layers;
		this.layersNumber = layers.Length;
		this.sizes = new int[layersNumber+1];
		
		this.inputSize = layers[0].getInputSize();
		this.outputSize = layers[layersNumber-1].getOutputSize();
	}
	
	// Getters
	public float[] compute(float[] input) {
		if (input.Length != inputSize) {
			Debug.LogError ("La taille de l'entrée ne correspond pas à ce qui est attendu [gene]");
			return new float[0];
		}
		for (int i = 0; i < layersNumber; i++)
			input = layers[i].getOutput(input);
		return input;
	}
	
	public static gene weightedAverage (gene[] genes, float[] weights) {
		// On moyenne toutes les couches
		// On considère que tous les gènes ont la même structure
		int layersNb = genes[0].layersNumber;
		int genesNb = genes.Length;
		
		geneLayer[] newLayers = new geneLayer[layersNb];
		for (int iLayer = 0; iLayer < layersNb; iLayer++) {
			geneLayer[] toAverage = new geneLayer[genesNb];
			for (int iGene = 0; iGene < genesNb; iGene++) {
				toAverage[iGene]=genes[iGene].layers[iLayer];
			}
			newLayers[iLayer]=geneLayer.weightedAverage(toAverage, weights);
		}
		return new gene(newLayers);
		
	}
}

