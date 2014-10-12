using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Painting : MonoBehaviour {

	[XmlAttribute("name")]
	public string name;
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
	public string artist;
	public string paintingName;
	public int date;

	// Use this for initialization
	void Start () {
//		Painting[] paintingsCollection = AutoInstanciation.Load("C:/Users/Cindy/Documents/test/MuseumVisit/Assets/Resources/paintingList.xml");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
