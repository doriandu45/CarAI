using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
	// Config physique
	float carSpeed = 10f; // Accélération de la voiture 
	float carTorque = -25f; // Moment appliqué quand la voiture tourne
	float driftFactorSticky = 0.33f; // Quantité de mouvement sur le côté conservé quand les roues adhèrent
	float driftFactorSlippy = 0.999f; // Quantité de mouvement sur le côté conservé quand les roues glissent
	float slippyThreshold = 100f; // Vitesse sur le côté nécessaire pour perdre l'adhérance des roues (passage de sticky à slippy)
	// NOTE: Valeur énorme pour désactiver, au final ça encourage l'IA à rester en sticky et à rouler lentement
	float stickyThreshold = 1f; // Vitesse sur le côté nécessaire pour reprendre l'adhérance
	
	
	// Debug
	bool debugSlippy=false;
	//bool debugCollision=true;
	
	public float thrust;
	public float direction;
	public GameObject carPrefab;
	
	bool isSlippy = false; // Si la voiture a perdu l'adhérance
	Rigidbody2D rb;
	SpriteRenderer sr;
	
    // Start is called before the first frame update
    void Start()
    {
		// Quelques variables pour éviter la rapatition
        rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		// On vérifie les valeurs:
		thrust = constraintFloat(thrust,-1f,1f);
		direction = constraintFloat(direction,-1f,1f);
		
		
		// Accélération de la voiture
        //rb.AddForce (transform.up * carSpeed * Input.GetAxis("Acceleration") );
		rb.AddForce (transform.up * carSpeed * thrust );
		
		// Direction
		//rb.angularVelocity = Input.GetAxis("Direction") * carTorque * ForwardVel().magnitude ;
		rb.angularVelocity = direction * carTorque * ForwardVel().magnitude ;
		
		
		// On supprime les movements sur le côté
		float currentMagn = RightVel().magnitude;
		
		if (debugSlippy) {
			if (isSlippy)
				sr.color = Color.blue;
			else
				sr.color = Color.red;
		}
		
		if (!isSlippy && currentMagn > slippyThreshold)
			isSlippy = true;
		else if (isSlippy && currentMagn < stickyThreshold)
			isSlippy = false;
				
		float driftFactor= driftFactorSticky;
		if (isSlippy )
			driftFactor = driftFactorSlippy;
		
		rb.velocity = ForwardVel() + RightVel() * driftFactor;
		
		/*if (debugCollision) {
			detectLeft.dist();
			detectRight.dist();
			if (detectFront.dist() > 2f)
				sr.color= Color.green;
			else
				sr.color=Color.red;
		}*/
		
    }
	
	float constraintFloat (float fl, float min, float max) {
		if (fl > max) return max;
		if (fl < min) return min;
		return fl;
	}
	
	Vector2 ForwardVel() {
		// Nous rend le vecteur vitesse suivant la direction vers l'avant (à l'aide d'un produit scalaire)
		return transform.up * Vector2.Dot (rb.velocity, transform.up);
	}
	
	Vector2 RightVel() {
		// Idem, mais pour le côté (droite = + / gauche = -)
		return transform.right * Vector2.Dot (rb.velocity, transform.right);
	}
	
	public float getSpeed() {
		return rb.velocity.magnitude;
	}
	
	void OnCollisionEnter2D(Collision2D col) {
		SendMessage("killCar");
	}
	
	void newCar() {
		Instantiate(carPrefab, new Vector2(-42.27f, -6.87f), Quaternion.identity);
	}
}
