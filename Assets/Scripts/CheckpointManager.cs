using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
	private Collider2D[] checkpoints;
	private int ComponentNb;
	
    // Start is called before the first frame update
    void Start()
    {
        checkpoints= GetComponentsInChildren<EdgeCollider2D>();
		ComponentNb = checkpoints.Length;
    }

    // Renvoie le numéro du checkpoint
	public int getCpID(Component col) {
		for (int i = 0; i < ComponentNb; i++)
			if (checkpoints[i] == col)
				return i;
		// Pas trouvé
		return -1;
	}
	
	// Renvoie le nombre de checkpoint
	public int getCpNb() {
		return ComponentNb;
	}
	
	// Renvoie le collider du checkpoint demandé
	public Collider2D getCpCol(int id) {
		return checkpoints[id];
	}
}
