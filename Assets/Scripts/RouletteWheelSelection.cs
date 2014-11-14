using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class RouletteWheelSelection {
	

	public static Painting getAPainting(Painting[] listPainting){
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

	public static Painting getAPainting2(Painting[] listPainting,Dictionary<Painting,float> fitness){
		float sumFitness = 0.0f;
		
		foreach(Painting p in listPainting){
			float f;
			fitness.TryGetValue(p,out f);
			sumFitness += f;
		}
		
		float nb = Random.Range (0.0f, sumFitness);
		float currentFitness = 0.0f;
		
		foreach(Painting p in listPainting){
			float f;
			fitness.TryGetValue(p,out f);
			currentFitness += f;
			if(nb < currentFitness){
				return p;
			}
		}
		
		return null;
	}

}
