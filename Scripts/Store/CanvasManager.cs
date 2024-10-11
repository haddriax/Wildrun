using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{

	[SerializeField] Canvas canvas;
	[SerializeField] Canvas canvas2;
	[SerializeField] Canvas canvas3;
	[SerializeField] Canvas canvas4;


	private Canvas CurrentCanvas;
	// Start is called before the first frame update
	void Start()
	{
		CurrentCanvas = canvas;
	}

	public void ActivateCanvas()
	{
		if (CurrentCanvas == canvas)
		{
			canvas.gameObject.SetActive(false);
			canvas2.gameObject.SetActive(true);
			CurrentCanvas = canvas2;
		}
		else if (CurrentCanvas == canvas2)
		{
			canvas2.gameObject.SetActive(false);
			canvas3.gameObject.SetActive(true);
			CurrentCanvas = canvas3;
		}
	}
	public void ActivateCanvas4()
	{
		if(CurrentCanvas==canvas)
		{
			canvas.gameObject.SetActive(false);
			canvas4.gameObject.SetActive(true);
			CurrentCanvas = canvas4;
		}
	}

	public void DesactivateCanvas()
	{
		if (CurrentCanvas == canvas2)
		{
			canvas.gameObject.SetActive(true);
			canvas2.gameObject.SetActive(false);
			CurrentCanvas = canvas;
		}
		else if (CurrentCanvas == canvas3)
		{
			canvas2.gameObject.SetActive(true);
			canvas3.gameObject.SetActive(false);
			CurrentCanvas = canvas2;
		}
		else if (CurrentCanvas==canvas4)
		{
			canvas.gameObject.SetActive(true);
			canvas4.gameObject.SetActive(false);
			CurrentCanvas = canvas;
		}
	}



	//    // Update is called once per frame
	//    void Update()
	//    {

	//    }
}
