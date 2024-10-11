using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class PlayerControllerGhost : MonoBehaviour
{
	[Range(1.0f, 100.0f)]
	[SerializeField]
	private float speed = 50.0f;

	public float Speed
	{
		get => speed;
	}
	private float timer;
	private Vector3 spawnPosition;

	//public List<Vector3> positonSave;
	//public List<Quaternion> quaternionSave;

	//private PlayerGhost playerGhost;
	private JsonWriter jsonWriter;
	private RaceManagerGhost raceManager;
	private bool isSpawning;

	[Tooltip("0.017 save the position and rotation each frame")]
	[Range(0.017f, 1.0f)]
	[SerializeField]
	private float timerOffsetSave = 0.2f;

	private void Start()
	{
		timer = 0.0f;
		spawnPosition = this.transform.position;
		raceManager = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManagerGhost>();
		jsonWriter = GameObject.FindGameObjectWithTag("JsonWriter").GetComponent<JsonWriter>();


		isSpawning = false;
	}

	private void Update()
	{
		Movement();
		SaveGhostPosition();
		CheckRaceFinished();
	}

	private void Movement()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		// Rotations
		Vector3 rotation = new Vector3(0.0f, h, 0.0f);
		this.transform.Rotate(rotation * speed * 2.0f * Time.deltaTime);

		// Movements
		if (Input.GetKey(KeyCode.Z))
		{
			this.transform.position += this.transform.forward;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			this.transform.position -= this.transform.forward;
		}
		//Vector3 velocity = Vector3.zero;
		//velocity += this.transform.forward * v * speed;
		//velocity = Vector3.ClampMagnitude(velocity, speed);

		// Apply velocity
		//this.gameObject.GetComponent<Rigidbody>().velocity = velocity;
	}

	private void SaveGhostPosition()
	{
		if (!raceManager.raceFinished)
		{
			timer += Time.deltaTime;

			if (timer >= timerOffsetSave)
			{
				jsonWriter.writeToJson(gameObject.transform.position, gameObject.transform.rotation);

				timer -= timerOffsetSave;
			}
		}
	}

	private void CheckRaceFinished()
	{
		if (raceManager.raceFinished && !isSpawning)
		{
			this.transform.position = spawnPosition;

			isSpawning = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{

		if (other.CompareTag("TriggerFinish"))
		{
			raceManager.raceFinished = true;
		}
	}

}
