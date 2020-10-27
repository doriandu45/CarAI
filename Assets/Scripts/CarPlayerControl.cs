using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPlayerControl : MonoBehaviour
{
	
	public CarControl cc;
	public CheckpointManager cpm;
	
	private int cp=0;
	private int lap=0;
	private int totalCp=0;
	
	private float score=0;
	private float startTime;
	
	private Collider2D carCollider;
	
	void Start()
    {
		carCollider = GetComponent<BoxCollider2D>();
		startTime = Time.time;
    }
	
	void FixedUpdate()
    {
		 
        cc.thrust = Input.GetAxis("Acceleration");
		cc.direction = Input.GetAxis("Direction");
    }
	
	
	// Quand la voiture touche un checkpoint
	void OnTriggerEnter2D(Collider2D col) {
		int touchedCp=cpm.getCpID(col);
		Debug.Log("Checkpoint touché: "+touchedCp+"\nAncien checkpoint: "+this.cp);
		// Si on a touché le même checkpoint qu'actuellement
		if (touchedCp == this.cp)
			return;
		// Si on a fait un tour
		if (touchedCp == 0 && this.cp == cpm.getCpNb() -1) {
			this.cp = 0;
			this.lap++;
			totalCp++;
			return;
		}
		// Si on touche un checkpoint précédent, c'est game over (Oui si on a touché le dernier checkpoint alors qu'on est au cp0)
		if (touchedCp < this.cp || (touchedCp!=1 && this.cp==0)) {
			killCar();
		}
		totalCp++; // On a touché un checkpoint différent
		this.cp = touchedCp;
	}
	
	// Tue la voiture et calcule son score pour préparer la reproduction
	void killCar() {
		// Calcul du score
		this.score = totalCp*100;
		int nextCp = this.cp+1;
		if (nextCp == cpm.getCpNb())
			nextCp=0;
		Debug.Log(nextCp+"\n"+cpm.getCpNb());
		float distCpCar = carCollider.Distance(cpm.getCpCol(nextCp)).distance; // Distance entre le checkpoint précédent et la voiture
		float distCpCp = cpm.getCpCol(this.cp).Distance(cpm.getCpCol(nextCp)).distance; // Distance entre le checkpoint précédent et le suivant
		
		float normalisedDist = (distCpCar/distCpCp)*100;
		Debug.Log("Score CP: "+score+"\nDistance Cp-voiture: "+distCpCar+"\nDistance Cp-Cp: "+distCpCp+"\nDistance normanisée: "+normalisedDist);
		this.score+= normalisedDist;
		
		float deltaT = Time.time - startTime;
		float timeScore = 600/deltaT; // En considérant que la voiture met une minute à faire le tour. Cela donne un score dans l'ordre de grandeur de 10.
		// ATTENTION! Il faudrait scale avec le nombre de checkpoints, sinon la voiture aura un meilleur score en se crashant direct dans le mur!
		timeScore*= (float)this.totalCp/(cpm.getCpNb()*3);
		score+=timeScore;
		
		
		Debug.Log("Temps: "+deltaT+"\nScore temps:"+timeScore+"\nScore TOTAL:"+score);
	}
}
