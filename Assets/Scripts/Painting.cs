using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class Painting : MonoBehaviour {

	public enum Tags{
		Femme,
		Combat,
		Animaux,
		Portrait,
		Science,
		Musique,
		Nature,
		Urbanisme,
		Baroque,
		Cubisme,
		Futurisme,
		Surrealisme,
		Romantisme
	}

	public string artist;
	public string paintingName;
	public int year;
	public string style;
	public float fitness = 1.0f;
	public List<Tags> tagList;

}
