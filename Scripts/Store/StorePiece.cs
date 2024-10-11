using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorePiece : MonoBehaviour
{
	List<Part> parts;
	Part part;

	[SerializeField] Text textNameType ;
	[SerializeField] Text textPriceType;

	[SerializeField] Text textNameName;
	[SerializeField] Text textPriceName;

	[SerializeField] InputField input;
	CanvasManager canvas;

	GameObject pieces;
	TypePart type;
	int money = 10;
	bool sortChoose;

	// Start is called before the first frame update
	void Start()
	{
		pieces = new GameObject();
		parts = new List<Part>();
		canvas = GameObject.FindObjectOfType<CanvasManager>();
		textPriceName.enabled = false;
		textNameName.enabled = false;
	}

	

	public void PiecesSortByType()
	{
		parts = SavedDatasManager.GetPartsByTypePart(type);

		foreach (Part p in parts)
		{
			pieces = Instantiate(p.gameObject);

			textNameType.text = p.Name;
			textPriceType.text = p.Price.ToString();
		}
		sortChoose = true;
	}
	public void PiecesSortByName()
	{
	if (SavedDatasManager.GetPartByName(input.text)!= null)
		{
			textPriceName.enabled = true;
			textNameName.enabled = true;

			part = SavedDatasManager.GetPartByName(input.text);
			Debug.Log("part : " + part);
			pieces = Instantiate(part.gameObject);
			textNameName.text = part.Name;
			textPriceName.text = part.Price.ToString();
		}
	else
		{
			Debug.Log("There are no pieces with this name");
		}
		sortChoose = false;

	}

	public void ButtonBaseFrame()
	{
		type = TypePart.BaseFrame;
		canvas.ActivateCanvas();
	}

	public void ButtonReactor()
	{
		type = TypePart.Reactor;
		canvas.ActivateCanvas();
	}

	public void ButtonSteering()
	{
		type = TypePart.FrontWings;
		canvas.ActivateCanvas();
	}

	public void ButtonCooling()
	{
		type = TypePart.BackWings;
		canvas.ActivateCanvas();
	}

	public void ClearText()
	{
		textNameName.text = "";
		textPriceName.text = "";
		input.text = "";

	}


	public void DestroyObject()
	{
		Destroy(pieces.gameObject);
	}

	public void ButtonBuy()
	{
		if(sortChoose == true)
		{
			parts = SavedDatasManager.GetPartsByTypePart(type);
			foreach (Part p in parts)
			{
				if (p.Price <= money)
				{
					//p.IsUnlocked = true;
					money -= p.Price;
					Debug.Log("Buy");
				}
				else
				{
					Debug.Log("You doesn't have enough money");
				}

			}
		}
		else if (sortChoose == false && SavedDatasManager.GetPartByName(input.text)!= null)
		{
			part = SavedDatasManager.GetPartByName(input.text);

			if(part.Price <= money)
			{
				//part.IsUnlocked = true;
				money -= part.Price;
				Debug.Log("Buy");
			}
			else
			{
				Debug.Log("You doesn't have enough money");

			}
		}
	}
}
