using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class PlayerGhost : MonoBehaviour
{

	private JsonWriter jsonWriter;

	private bool isGhostObjectCreated;
	private RaceManagerGhost raceManager;
	private GameObject cube;
	private int currentPath;
	private float speedPlayer;
	JsonData data;

	private void Start()
	{

		isGhostObjectCreated = false;
		raceManager = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManagerGhost>();
		cube = null;
		currentPath = 0;
		speedPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerGhost>().Speed;
		jsonWriter = GameObject.FindGameObjectWithTag("JsonWriter").GetComponent<JsonWriter>();
	}

	private void Update()
	{
		CreateGhostObject();
		MoveToNextPosition();

	}

	private bool CheckNotEmptyPosition()
	{
		if (jsonWriter.saveObject.positionX.Count > 0)
			return true;
		else
			return false;
	}

	private void CreateGhostObject()
	{

		if (raceManager.raceFinished && CheckNotEmptyPosition() && !isGhostObjectCreated)
		{
			cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.rotation = new Quaternion();
			cube.transform.position = new Vector3();
			cube.GetComponent<MeshRenderer>().material = (Material)Resources.Load("GhostMAT");
			Physics.IgnoreCollision(cube.GetComponent<Collider>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>());
			isGhostObjectCreated = true;

		}
	}

	private void MoveToNextPosition()
	{
		if (isGhostObjectCreated)
		{
			jsonWriter.readFromJson(currentPath);

			if (cube != null)
			{
				if (currentPath < jsonWriter.saveObject.positionX.Count)
				{

					if (Vector3.Distance(cube.transform.position, new Vector3(jsonWriter.saveObject.positionX[currentPath], jsonWriter.saveObject.positionY[currentPath], jsonWriter.saveObject.positionZ[currentPath])) >= 1.0f)
					{
						cube.transform.rotation = Quaternion.Lerp(cube.transform.rotation,jsonWriter.saveObject.rotation[currentPath], speedPlayer * 2.0f * Time.deltaTime);
						cube.transform.position = Vector3.Lerp(cube.transform.position, new Vector3(jsonWriter.saveObject.positionX[currentPath], jsonWriter.saveObject.positionY[currentPath], jsonWriter.saveObject.positionZ[currentPath]), speedPlayer * 2.0f * Time.deltaTime);
						transform.Translate(cube.transform.position * speedPlayer);
					}
					else
					{
						currentPath++;

					}
				}
			}
		}
	}

}
