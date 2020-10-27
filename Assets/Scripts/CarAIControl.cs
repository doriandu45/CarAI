using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAIControl : MonoBehaviour
{
	public float cpCooldown=25f;
	int maxLap = 3;
	
	public gene brain=null;
	public CarControl cc;
	public CheckpointManager cpm;
	public CollisionCastDetector detectLeft;
	public CollisionCastDetector detectFront;
	public CollisionCastDetector detectRight;
	public AITrainer trainer=null;
	public int id;
	
	private int cp=0;
	private int lap=0;
	private int totalCp=0;
	
	private float score=0;
	private float startTime;
	
	private float lastCpTime;
	
	private Collider2D carCollider;
	
	float[] brainInput;
	float[] results; 
	
    // Start is called before the first frame update
    void Start()
    {
		startTime = Time.time;
		lastCpTime = Time.time;
		carCollider = GetComponent<BoxCollider2D>();
		if (brain is null) {
			int[] layers={4,5,4,3,2};
			brain = new gene(layers);
		}
		
		brainInput = new float[4];
		results = new float[2];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		brainInput[0]=detectLeft.dist()/detectLeft.distMax;
		brainInput[1]=detectFront.dist()/detectFront.distMax;
		brainInput[2]=detectRight.dist()/detectRight.distMax;
		brainInput[3]=cc.getSpeed();
        float[] results = brain.compute(brainInput);
		cc.thrust = results[0];
		cc.direction = results[1];
		
		if (Time.time - lastCpTime > cpCooldown)
			killCar();
    }
	
	// Quand la voiture touche un checkpoint
	void OnTriggerEnter2D(Collider2D col) {
		int touchedCp=cpm.getCpID(col);
		// Si on a touché le même checkpoint qu'actuellement
		if (touchedCp == this.cp)
			return;
		// Si on a fait un tour
		if (touchedCp == 0 && this.cp == cpm.getCpNb() -1) {
			this.cp = 0;
			this.lap++;
			totalCp++;
			lastCpTime=Time.time;
			if (this.lap >= maxLap)
				killCar();
			return;
		}
		
		// Si on touche un checkpoint précédent, c'est game over (Oui si on a touché le dernier checkpoint alors qu'on est au cp0)
		if (touchedCp < this.cp || (touchedCp!=1 && this.cp==0)) {
			killCar();
			return;
		}
		totalCp++;
		lastCpTime=Time.time;
		
		// On a touché un checkpoint différent
		this.cp = touchedCp;
	}
	
	// Tue la voiture et calcule son score pour préparer la reproduction
	void killCar() {
		// Calcul du score
		this.score = totalCp*100;
		int nextCp = this.cp+1;
		if (nextCp == cpm.getCpNb())
			nextCp=0;
		float distCpCar = carCollider.Distance(cpm.getCpCol(nextCp)).distance; // Distance entre le checkpoint précédent et la voiture
		float distCpCp = cpm.getCpCol(this.cp).Distance(cpm.getCpCol(nextCp)).distance; // Distance entre le checkpoint précédent et le suivant
		
		float normalisedDist = (distCpCar/distCpCp)*100;
		this.score+= normalisedDist;
		
		//Debug.Log("Time: "+Time.time+"\nStart time: "+startTime);
		float deltaT = Time.time - startTime;
		float timeScore = 600/deltaT; // En considérant que la voiture met une minute à faire le tour. Cela donne un score dans l'ordre de grandeur de 10.
		//Debug.Log("timeScore raw: "+timeScore+"\nCheckpoints: "+totalCp+"\nPoids: "+(float)this.totalCp/(cpm.getCpNb()*3));
		// ATTENTION! Il faudrait scale avec le nombre de checkpoints, sinon la voiture aura un meilleur score en se crashant direct dans le mur!
		timeScore*= (float)this.totalCp/(cpm.getCpNb()*3);
		score+=timeScore;
		
		
		//Debug.Log("Temps: "+deltaT+"\nScore temps:"+timeScore+"\nScore TOTAL:"+score);
		if (trainer is null)
			SendMessage("newCar");
		else
			trainer.sendDeath(this.id, score);
		Destroy(gameObject);
	}
}
