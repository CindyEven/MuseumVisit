using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
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
		List<string> tags = new List<string>();
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
				foreach(XmlElement tag in infoNode.SelectNodes("Tags/Tag")){
					if(tag != null) tags.Add(tag.InnerText);
				}
			}
			XmlNode materialNode = node.SelectSingleNode("Material");
			if(materialNode != null){
				material = materialNode.InnerText;
			}
			instanciateNewPainting(name,position,rotation,scale,artist,paintingName,year,style,material,tags);
			name=""; position=""; rotation=""; scale=""; artist=""; paintingName=""; year=""; style=""; material=""; tags.Clear();
		}
	}
	
	private void modifyXml(){
		foreach(XmlElement node in xmlDoc.SelectNodes("SceneObjects/Paintings/Painting")){
			Transform newTransform = GameObject.Find(node.GetAttribute("name")).transform;
			XmlNode transformNode = node.SelectSingleNode("Transform");
			if(transformNode != null){
				char[] charsToTrim = { ' ', '(', ')'};
				transformNode.SelectSingleNode("Position").InnerText = newTransform.position.ToString("f6").Trim(charsToTrim);
				transformNode.SelectSingleNode("Scale").InnerText = newTransform.localScale.ToString("f6").Trim(charsToTrim);
				transformNode.SelectSingleNode("Rotation").InnerText = newTransform.rotation.eulerAngles.ToString("f6").Trim(charsToTrim);
			}

			XmlNode tagsNode = node.SelectSingleNode("Informations/Tags");
			if(tagsNode == null){
				XmlNode newTagsNode = xmlDoc.CreateNode(XmlNodeType.Element,"Tags",null);
				node.SelectSingleNode("Informations").AppendChild(newTagsNode);
				foreach(Painting.Tags tag in newTransform.GetComponent<Painting>().tagList){
					XmlNode newTag = xmlDoc.CreateNode(XmlNodeType.Element,"Tag",null);
					newTag.InnerText = tag.ToString();
					newTagsNode.AppendChild(newTag);
				}
			}
			if(tagsNode != null){
				tagsNode.RemoveAll();
				foreach(Painting.Tags tag in newTransform.GetComponent<Painting>().tagList){
					XmlNode newTag = xmlDoc.CreateNode(XmlNodeType.Element,"Tag",null);
					newTag.InnerText = tag.ToString();
					tagsNode.AppendChild(newTag);
				}
			}
		}
		xmlDoc.Save(Application.dataPath +"/Resources/"+fileName+".xml");
	}
	
	private void instanciateNewPainting(string name,string positionString, string rotationString, string scaleString,
	                                    string artist, string paintingName, string year, string style, string material,
	                                    List<string> tags){
		GameObject go = (GameObject)Instantiate(Resources.Load("Painting", typeof(GameObject)));
		go.transform.parent = GameObject.Find ("Paintings").transform;
		go.name = name;
		go.transform.position = StringToVect3 (positionString);
		go.transform.rotation = Quaternion.Euler (StringToVect3 (rotationString));
		go.transform.localScale = StringToVect3 (scaleString);
		Painting paint = go.GetComponent<Painting>();
		paint.artist = artist;
		paint.style = style;
		if (style == "Baroque") paint.tagList.Add (Painting.Tags.Baroque);
		if (style == "Cubisme") paint.tagList.Add (Painting.Tags.Cubisme);
		if (style == "Romantisme") paint.tagList.Add (Painting.Tags.Romantisme);
		if (style == "Futurisme") paint.tagList.Add (Painting.Tags.Futurisme);
		if (style == "Surréalisme") paint.tagList.Add (Painting.Tags.Surrealisme);
		paint.paintingName = paintingName;
		if (year == "") {
			paint.year = 0;
		} else {
			paint.year = int.Parse (year);
		}
		foreach (string tag in tags) {
			Debug.Log(tag);
			if(tag == "Femme") paint.tagList.Add(Painting.Tags.Femme);
			if(tag == "Animaux") paint.tagList.Add(Painting.Tags.Animaux);
			if(tag == "Combat") paint.tagList.Add(Painting.Tags.Combat);
			if(tag == "Portrait") paint.tagList.Add(Painting.Tags.Portrait);
			if(tag == "Science") paint.tagList.Add(Painting.Tags.Science);
			if(tag == "Musique") paint.tagList.Add(Painting.Tags.Musique);
			if(tag == "Nature") paint.tagList.Add(Painting.Tags.Nature);
			if(tag == "Urbanisme") paint.tagList.Add(Painting.Tags.Urbanisme);
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