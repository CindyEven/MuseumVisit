using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class RouletteWheelSelection {

	static Painting[] listPainting = GameObject.FindObjectsOfType<Painting> ();

	public static Painting getAPainting(){
		float sumFitness = 0.0f;

		foreach(Painting p in listPainting){
			sumFitness += p.fitness;
		}

		float nb = Random.Range (0.0f, sumFitness);
		float currentFitness = 0.0f;

		foreach(Painting p in listPainting){
			currentFitness += p.fitness;
			if(nb < currentFitness){
				return p;
			}
		}

		return null;
	}

}
