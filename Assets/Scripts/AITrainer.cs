using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class training {
	public gene brain;
	public float score;
	
	public training(int[] layers) {
		this.score = 0f;
		this.brain = new gene(layers);
	}
	
	public training(gene newGene) {
		this.score = 0f;
		this.brain = newGene;
	}
}


public class AITrainer : MonoBehaviour
{
	public Text textInfo;
	
	private int[] layers={4,5,4,3,2}; // Couches du réseau
	public float cpCooldown = 25f; // Temps passé sans toucher un checkpoint avant de mourrir (secondes)
	public float finalCpCooldown = 5f; // Temps minimal, qui sera progressivement atteint au fil des générations
	public float generationCooldownScale = 0.95f; // Multiplicateur de cooldown après chaque génération (le cooldown sera cappé à la variable finalCpCooldown
	
	public int carsPerBatch=10;
	public int batches=2;
	
	private int currentBatch = 0;
	private int generation = 0;
	
	private int aliveCars;
	
	private training[] genes;
	
	public CarAIControl carPrefab;
	public CheckpointManager cpm;
	
    // Start is called before the first frame update
    public void trainingStart()
    {
		
		// Crée des gènes randoms
		int size=batches*carsPerBatch;
		genes= new training[size];
		for (int i = 0; i < size; i++)
			genes[i]=new training(layers);
		newBatch();
    }

   
	
	void spawnCar(int id, gene brain) {
		CarAIControl car=Instantiate(carPrefab, transform.position, Quaternion.identity);
		car.cpm=this.cpm;
		car.brain = brain;
		car.trainer = this;
		car.id = id;
	}
	
	void updateTextInfo() {
		textInfo.text="Infos:\nGeneration: "+this.generation+"\nBatch: "+(this.currentBatch)+"/"+this.batches+"\nCheckpoint cooldown: "+cpCooldown.ToString("0.00")+"s\nCars alive: "+this.aliveCars+"/"+this.carsPerBatch;
	}
	
	void newBatch(){
		updateTextInfo();
		this.aliveCars = carsPerBatch;
		int startI=currentBatch*carsPerBatch;
		for (int i = startI; i < startI+carsPerBatch; i++)
			spawnCar(i, genes[i].brain);
		currentBatch++;
	}
	
	public void sendDeath(int id, float score) { // Nous informe qu'une voiture est morte
		this.aliveCars--;
		updateTextInfo();
		genes[id].score=score;
		if (this.aliveCars == 0) { // Toutes les voitures sont mortes
			if(currentBatch<batches) {
				newBatch();
				return;
			}
		} else {
			return; // On attend que tout le monde meurt
		}
		// On repère les 3 meilleurs
		int size=batches*carsPerBatch;
		
		int[] bestIds = {0,0,0};
		float[] bestScores = {0f,0f,0f};
		
		
		for (int i = 0; i < size; i++) {
			bool done = false;
			for (int iBest = 0; iBest < 3; iBest++) {
				if (!done && (genes[i].score > bestScores[iBest])) {
					shiftIntArray(bestIds, iBest);
					bestIds[iBest] = i;
					shiftFloatArray(bestScores, iBest);
					bestScores[iBest] = genes[i].score;
					done = true;
				}
				
				
				
				
			}
		}
		
		
		
		// Premiers candidats: les 3 meilleurs de la dernière génération
		training[] nextGen = new training[size];
		
		for (int i = 0; i < 3; i++) {
			nextGen[i] = new training(genes[bestIds[i]].brain);
		}
		
		// Quetrième: gènes avec la moyenne de toutes les voitures, pondérée avec leur score.
		
		// On est obligé de séparer en 2 tableaux
		gene[] genesOnly = new gene[size];
		for (int i = 0; i < size; i++)
			genesOnly[i]=genes[i].brain;
		float[] scoresOnly = new float[size];
		for (int i = 0; i < size; i++)
			scoresOnly[i]=genes[i].score;
		
		
		nextGen[3] = new training(gene.weightedAverage(genesOnly, scoresOnly));
		
		// Cinquième: gènes avec la moyenne non pondérée de toutes les coitures
		float[] one = new float[size];
		for (int i = 0; i < size; i++)
			one[i]=1f;
		
		nextGen[4] = new training(gene.weightedAverage(genesOnly,one));
		
		// Six et septième: idem mais avec les 3 premiers seulement
		gene[] bo3Genes = new gene[3];
		float[] bo3one = {1f,1f,1f};
		for (int i = 0; i < 3; i++)
			bo3Genes[i] = genes[bestIds[i]].brain;
		
		nextGen[5] = new training(gene.weightedAverage(bo3Genes,bestScores));
		nextGen[6] = new training(gene.weightedAverage(bo3Genes,bo3one));
		
		// Pour les autres, soit on:
		// - Met un nouveau gène aléatoire
		// - Fait une moyenne sur tous les gènes pondérée aléatoirement
		
		for (int i = 7; i < size; i++) {
			// Pour les 2 cas, on regarde la parité de i
			if ((i%2)==1) {
				// Nouveau gène aléatoire
				nextGen[i]=new training(layers);
			} else {
				// Moyenne pondérée aléatoire
				float[] randomWeights=new float[size];
				for (int iWeight = 0; iWeight < size; iWeight++)
					randomWeights[iWeight]=Random.Range(0f,100f);
				nextGen[i] = new training(gene.weightedAverage(genesOnly,randomWeights));
			}
		}
		
		generation++;
		genes = nextGen;
		currentBatch=0;
		
		// On scale le cooldown checkpoint
		cpCooldown*=generationCooldownScale;
		if (cpCooldown < finalCpCooldown)
			cpCooldown = finalCpCooldown;
		newBatch();
		
	}
	

	
	
	// Ces 2 méthodes font la même chose, mais pour des arrays de types différents. J'ai pas trouvé comment on passe un array de n'importe quel type en paramètre
	private void shiftIntArray (int[] toShift, int iShift) {
		int size = toShift.Length;
		for (int i = size-2; i >= iShift; i--)
			toShift[i+1]=toShift[i];
	}
	
	private void shiftFloatArray (float[] toShift, int iShift) {
		int size = toShift.Length;
		for (int i = size-2; i >= iShift; i--)
			toShift[i+1]=toShift[i];
	}
	
}
