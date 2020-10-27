using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingValuesManager : MonoBehaviour
{
	//void Start() {Debug.Log(System.Globalization.CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);}
	
    // Transmet les valeurs de la fenêtre de démarrage dans l'AI Trainer
	public AITrainer ait;
	public InputField CarsPerBatchText;
	public InputField BatchesText;
	public InputField CpCooldownText;
	public InputField CpScalingText;
	public InputField CpMinCooldownText;
	public InputField MutationRateText;
	
	public GameObject StartingPanel;
	
	public void startTraining() {
		ait.cpCooldown = float.Parse(CpCooldownText.text.Replace(',','.'),System.Globalization.CultureInfo.InvariantCulture.NumberFormat); // Permet de gérer les , ou les . dans les entrées
		ait.finalCpCooldown = float.Parse(CpMinCooldownText.text.Replace(',','.'),System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		ait.generationCooldownScale = float.Parse(CpScalingText.text.Replace(',','.'),System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		ait.carsPerBatch = int.Parse(CarsPerBatchText.text);
		ait.batches = int.Parse(BatchesText.text);
		neurone.mutationRate = int.Parse(MutationRateText.text);
		
		Destroy(StartingPanel);
		ait.trainingStart();
	}
}
