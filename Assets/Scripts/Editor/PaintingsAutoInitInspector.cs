using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

[CustomEditor (typeof(PaintingsAutoInit))]
public class PaintingsAutoInitInspector : Editor {

	private string path;
	private string fileName = "paintingList";
	private XmlDocument xmlDoc;
	private TextAsset textXml;
	
	public override void OnInspectorGUI () {
		GameObject go = GameObject.FindObjectOfType<PaintingsAutoInit>().gameObject;
		go.name = "Paintings";
		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;

        if(GUILayout.Button("Instanciate Paintings")){
			loadXMLFromAssest();
			readXml();
		}

		if(GUILayout.Button("Save modifs")){
			loadXMLFromAssest();
			modifyXml();
		}
	}
	
	// Following method load xml file from resouces folder under Assets
	private void loadXMLFromAssest(){
		
		xmlDoc = new XmlDocument();
		textXml = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
		xmlDoc.LoadXml(textXml.text);
	}
	
	// Following method reads the xml file and display its content 
	private void readXml(){
		string name="", position="", rotation="", scale="", artist="", paintingName="", year="", style="", material="";
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
				style = infoNode.SelectSingleNode("ArtisticMovement").InnerText;
			}
			XmlNode materialNode = node.SelectSingleNode("Material");
			if(materialNode != null){
				material = materialNode.InnerText;
			}
			instanciateNewPainting(name,position,rotation,scale,artist,paintingName,year,style,material);
			name=""; position=""; rotation=""; scale=""; artist=""; paintingName=""; year=""; style=""; material="";
		}
	}
	
	private void modifyXml(){
		string position="", rotation="", scale="";
		foreach(XmlElement node in xmlDoc.SelectNodes("SceneObjects/Paintings/Painting")){
			Transform newTransform = GameObject.Find(node.GetAttribute("name")).transform;
			XmlNode transformNode = node.SelectSingleNode("Transform");
			if(transformNode != null){
				char[] charsToTrim = { ' ', '(', ')'};
				transformNode.SelectSingleNode("Position").InnerText = newTransform.position.ToString("f6").Trim(charsToTrim);
				transformNode.SelectSingleNode("Scale").InnerText = newTransform.localScale.ToString("f6").Trim(charsToTrim);
				transformNode.SelectSingleNode("Rotation").InnerText = newTransform.rotation.eulerAngles.ToString("f6").Trim(charsToTrim);
			}
		}
		xmlDoc.Save(Application.dataPath +"/Resources/"+fileName+".xml");
	}
	
	private void instanciateNewPainting(string name,string positionString, string rotationString, string scaleString,
	                                    string artist, string paintingName, string year, string style, string material){
		GameObject go = (GameObject)Instantiate(Resources.Load("Painting", typeof(GameObject)));
		go.transform.parent = GameObject.Find ("Paintings").transform;
		go.name = name;
		go.transform.position = StringToVect3 (positionString);
		go.transform.rotation = Quaternion.Euler (StringToVect3 (rotationString));
		go.transform.localScale = StringToVect3 (scaleString);
		Painting paint = go.GetComponent<Painting>();
		paint.artist = artist;
		paint.style = style;
		paint.paintingName = paintingName;
		if (year == "") {
			paint.year = 0;
		} else {
			paint.year = int.Parse (year);
		}
		Material newMat = Resources.Load("Materials/Paintings/"+material) as Material;
		//Material[] mats = go.renderer.sharedMaterials;
		//mats [1] = newMat;
		//go.renderer.materials = mats;
		go.transform.FindChild ("Canvas").renderer.material = newMat;
	}
	
	private Vector3 StringToVect3 (string stringValue){
		string[] valuesString = stringValue.Split (',');
		Vector3 vect3Value = new Vector3 (float.Parse (valuesString [0]), float.Parse (valuesString [1]), float.Parse (valuesString [2]));
		return vect3Value;
	}
}