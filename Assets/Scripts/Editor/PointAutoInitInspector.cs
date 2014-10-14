using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

[CustomEditor (typeof(PointAutoInit))]
public class PointAutoInitInspector : Editor {

	private string path;
	private string fileName = "pointList";
	private XmlDocument xmlDoc;
	private TextAsset textXml;

	public override void OnInspectorGUI () {
		GameObject go = GameObject.FindObjectOfType<PointAutoInit>().gameObject;
		go.name = "Point Graph";
		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		if(GUILayout.Button("Instanciate Points")){
			loadXMLFromAssest();
			readXml();
			createLink();
		}
		
		if(GUILayout.Button("Save modifs")){
			loadXMLFromAssest();
			modifyXml();
			saveLink();
		}
	}
	
	void Awake(){

	}

	private void loadXMLFromAssest(){
		
		xmlDoc = new XmlDocument();
		textXml = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
		xmlDoc.LoadXml(textXml.text);
	}
	
	// Following method reads the xml file and display its content 
	private void readXml(){
		string name="", position="";
		foreach(XmlElement node in xmlDoc.SelectNodes("SceneObjects/Points/Point")){
			name = node.GetAttribute("name");
			XmlNode transformNode = node.SelectSingleNode("Transform");
			if(transformNode != null){
				position = transformNode.SelectSingleNode("Position").InnerText;
			}
			instanciateNewPoint(name,position);
			name=""; position="";
		}
	}

	private void createLink(){
		string name="", connected="";
		foreach(XmlElement node in xmlDoc.SelectNodes("SceneObjects/Points/Point")){
			name = node.GetAttribute("name");
			XmlNode connectedNode = node.SelectSingleNode("Connected");
			if(connectedNode != null){
				foreach(XmlElement nodeTo in connectedNode){
					connected = nodeTo.InnerText;
					addLink(name,connected);
					connected="";
				}
			}
			name="";
		}
	}

	private void modifyXml(){
		string position="";
		foreach(XmlElement node in xmlDoc.SelectNodes("SceneObjects/Points/Point")){
			Debug.Log(node.GetAttribute("name"));
			Transform newTransform = GameObject.Find(node.GetAttribute("name")).transform;
			XmlNode transformNode = node.SelectSingleNode("Transform");
			if(transformNode != null){
				Debug.Log(newTransform.position.ToString());
				char[] charsToTrim = { ' ', '(', ')'};
				transformNode.SelectSingleNode("Position").InnerText = newTransform.position.ToString("f6").Trim(charsToTrim);
			}
		}
		xmlDoc.Save(Application.dataPath +"/Resources/"+fileName+".xml");
	}

	private void saveLink(){
		foreach (XmlElement node in xmlDoc.SelectNodes("SceneObjects/Points/Point")) {
			Debug.Log (node.GetAttribute ("name"));
			bool alreadyConnected = false;
			List<Point> connectedTo = GameObject.Find (node.GetAttribute ("name")).GetComponent<Point> ().ConnectedTo;
			XmlNode connectedNode = node.SelectSingleNode ("Connected");
			if (connectedNode == null) {
				connectedNode = xmlDoc.CreateNode("element","Connected","");
				node.AppendChild(connectedNode);
			}
			foreach (Point point in connectedTo) {
				foreach (XmlElement nodeTo in connectedNode) {
					if (point.gameObject.name.Equals (nodeTo.InnerText)) {
						alreadyConnected = true;
						break;
					}
				}
				if (!alreadyConnected) {
					XmlNode newConnection = xmlDoc.CreateNode("element","To",""); 
					newConnection.InnerText = point.gameObject.name;
					connectedNode.AppendChild (newConnection);
				}
			}
		}
		xmlDoc.Save (Application.dataPath + "/Resources/" + fileName + ".xml");
	}

	private void instanciateNewPoint(string name,string positionString){
		GameObject go = (GameObject)Instantiate(Resources.Load("Point", typeof(GameObject)));
		go.transform.parent = GameObject.Find ("Point Graph").transform;
		go.name = name;
		go.transform.position = StringToVect3 (positionString);
	}

	private void addLink(string name,string connectedTo){
		GameObject go = GameObject.Find (name);
		GameObject goTo = GameObject.Find (connectedTo);
		Point point = go.GetComponent<Point>();
		point.ConnectedTo.Add (goTo.GetComponent<Point> ());
		Debug.DrawLine(go.transform.position,goTo.transform.position);
		Debug.Log("link between"+name+"and"+connectedTo);
	}
	
	private Vector3 StringToVect3 (string stringValue){
		string[] valuesString = stringValue.Split (',');
		Vector3 vect3Value = new Vector3 (float.Parse (valuesString [0]), float.Parse (valuesString [1]), float.Parse (valuesString [2]));
		return vect3Value;
	}

}
