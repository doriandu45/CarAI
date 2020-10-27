using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionCastDetector : MonoBehaviour
{
	public float distMax;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
	
	public float dist(){
		int layerMask = 1 << 8; // Filtre que le layer 8 (opération bitshift)
		RaycastHit2D hitInfo=Physics2D.Linecast (transform.position, transform.position + transform.up*distMax,layerMask);
		//Debug.DrawLine(transform.position, transform.position + transform.up*distMax, Color.white, 0.0f);
		
		float d = hitInfo.distance;
		Debug.DrawLine(transform.position, transform.position + transform.up*d, Color.green, 0.0f);
		if (d<0.01f) // Si aucun objet n'est trouvé
			return distMax;
		return d;
	}
	
    // Update is called once per frame
    void Update()
    {
        
    }
}
