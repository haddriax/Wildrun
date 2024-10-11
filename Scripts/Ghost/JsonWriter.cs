using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class JsonWriter : MonoBehaviour
{
	private List<float> savedPositionX;
	private List<float> savedPositionY;
	private List<float> savedPositionZ;
	private List<Quaternion> savedRotation;

	public SaveObject saveObject;

	JsonData data;
	string jsonString;


	public void Start()
	{
		savedPositionX = new List<float>();
		savedPositionY = new List<float>();
		savedPositionZ = new List<float>();
		savedRotation = new List<Quaternion>();

		saveObject = new SaveObject { positionX = savedPositionX, positionY = savedPositionY, positionZ = savedPositionZ, rotation = savedRotation };

	}
	public void Update()
	{
	}

	public void writeToJson(Vector3 position, Quaternion rotation)
	{
		savedPositionX.Add(position.x);
		savedPositionY.Add(position.y);
		savedPositionZ.Add(position.z);

		savedRotation.Add(rotation);

		data = JsonUtility.ToJson(saveObject);
		File.WriteAllText(Application.dataPath + "/Resources/GhostPosition.json", data.ToString());
	}

	public void readFromJson(int currentPath)
	{
		jsonString = File.ReadAllText(Application.dataPath + "/Resources/GhostPosition.json");

		saveObject = JsonUtility.FromJson<SaveObject>(jsonString);
	}

	public class SaveObject
	{
		public List<float> positionX;
		public List<float> positionY;
		public List<float> positionZ;

		public List<Quaternion> rotation;

	}
}
