using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

public class PaintingsAutoInit : MonoBehaviour {

	private string path;
	private string fileName;
	private XmlDocument xmlDoc;
	private TextAsset textXml;

	void Awake(){
		fileName = "paintingList";
	}
	
	void Start (){
		loadXMLFromAssest();
		readXml();
	}
	// Following method load xml file from resouces folder under Assets
	private void loadXMLFromAssest(){
		xmlDoc = new XmlDocument();
		textXml = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
		xmlDoc.LoadXml(textXml.text);
	}

	// Following method reads the xml file and display its content 
	private void readXml(){
		string name="", position="", rotation="", scale="", artist="", paintingName="", year="";
		foreach(XmlElement node in xmlDoc.SelectNodes("SceneObjects/Paintings/Painting")){
			name = node.GetAttribute("name");
			XmlNode transformNode = node.SelectSingleNode("Transform");
			if(transformNode != null){
				position = transformNode.SelectSingleNode("Position").InnerText;
				scale = transformNode.SelectSingleNode("Scale").InnerText;
				rotation = transformNode.SelectSingleNode("Rotation").InnerText;
			}
			XmlNode infoNode = node.SelectSingleNode("Informations");
			if(infoNode != null){
				artist = infoNode.SelectSingleNode("Artist").InnerText;
				paintingName = infoNode.SelectSingleNode("PaintingName").InnerText;
				year = infoNode.SelectSingleNode("Year").InnerText;
			}
			instanciateNewPainting(name,position,rotation,scale,artist,paintingName,year);
			name=""; position=""; rotation=""; scale=""; artist=""; paintingName=""; year="";
		}
	}

	private void instanciateNewPainting(string name,string positionString, string rotationString, string scaleString,
	                                    string artist, string paintingName,string year){
		GameObject go = (GameObject)Instantiate(Resources.Load("Painting", typeof(GameObject)));
		go.name = name;
		go.transform.position = StringToVect3 (positionString);
		go.transform.rotation = Quaternion.Euler (StringToVect3 (rotationString));
		go.transform.localScale = StringToVect3 (scaleString);
		Painting paint = go.GetComponent<Painting>();
		paint.artist = artist;
		paint.paintingName = paintingName;
		if (year == "") {
			paint.year = 0;
		} else {
			paint.year = int.Parse (year);
		}
	}

	private Vector3 StringToVect3 (string stringValue){
		string[] valuesString = stringValue.Split (',');
		Vector3 vect3Value = new Vector3 (float.Parse (valuesString [0]), float.Parse (valuesString [1]), float.Parse (valuesString [2]));
		return vect3Value;
	}

}