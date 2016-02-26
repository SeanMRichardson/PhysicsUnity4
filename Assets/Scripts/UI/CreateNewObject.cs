using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateNewObject : MonoBehaviour {

	public GameObject[] availableObjects;// list of all the types of creatable objects
	public GameObject[] prefabs = new GameObject[3];// the prefab objects
	public Sprite[] sprites = new Sprite[3];// the images of the objects
	public GameObject prefabPanel;// the panels spawned for the list
	public GameObject editPanel;// the panel for the edit window on an object
	public Canvas canvas;// the canvas

	private GameObject edit;// the cloned copy of the edit panel
	private GameObject panel;//the cloned copy of the list panel
	private RectTransform rect;// the rectTransform of panel
	private Vector3 pos = new Vector3(-470, 280);// spawn position of the first panel in the list 
	private Vector3 offset = new Vector3(0, -140, 0);// the offset for the rest of the tiles
	private Button[] buttons;// the list of the buttons on the panels
	private Button[] editButtons;// edit panel buttons
	private string massText;// the text from the UI inputfield for mass
	private string restitutionText;// the text from the UI inputfield for restitution
	private string sFrictionText;// the text from the UI inputfield for staticFriction
	private string dFrictionText;// the text from the UI inputfield for dynamicFriction
	private string posXText;// the text from the UI inputfield for the x position
	private string posYText;// the text from the UI inputfield for the y position
	private string posZText;// the text from the UI inputfield for the z position
	private int nextNum = 0;// the number used for changing the nam of the new object so collisions work

	// Use this for initialization
	void Start () 
	{
		// initialize the avilable objects
		availableObjects = new GameObject[3];

		// make the cancel button uninteractable
		HideCancelButton();
	}
	
	// Update is called once per frame
	void Update()
	{
		GetEditPanelValues();
	}

	/// <summary>
	/// creates the create menu panels
	/// </summary>
	public void CreateMenu()
	{
		// initialize the list of buttons
		buttons = new Button[3];

		// create the panel list on screen, add a button listener and set the panels up
		for(int i = 0; i < availableObjects.Length; i++)
		{
			GameObject o = prefabs[i];
			panel = Instantiate(prefabPanel) as GameObject;
			panel.transform.SetParent(canvas.transform);
			availableObjects[i] = panel;
			rect = panel.GetComponent<RectTransform>();
			rect.transform.localPosition = new Vector3(pos.x + (offset.x * i), pos.y + (offset.y * i));
			buttons[i] = rect.GetComponent<Button>();
			buttons[i].onClick.AddListener(delegate() { CreateEditPanel(o); });
			panel.transform.GetChild(1).GetComponent<Text>().text = prefabs[i].name;
			panel.transform.GetChild(0).GetComponent<Image>().sprite = sprites[i];
		}
		ShowCancelButton();
	}

	/// <summary>
	/// makes the pressed button uninteractable 
	/// </summary>
	public void HideCreateButton()
	{
		canvas.transform.GetChild(0).GetComponent<Button>().interactable = false;
	}

	/// <summary>
	/// makes create button interactable
	/// </summary>
	public void ShowCreateButton()
	{
		canvas.transform.GetChild(0).GetComponent<Button>().interactable = true;
	}

	/// <summary>
	/// makes cancel button interactable
	/// </summary>
	public void ShowCancelButton()
	{
		canvas.transform.GetChild(1).GetComponent<Button>().interactable = true;
	}

	/// <summary>
	/// makes the cancel button uninteractable
	/// </summary>
	public void HideCancelButton()
	{
		canvas.transform.GetChild(1).GetComponent<Button>().interactable = false;
	}

	/// <summary>
	/// creates an editing panel with values taken from the object wanting to be edited
	/// and can change them at run time
	/// </summary>
	/// <param name="o"></param>
	public void CreateEditPanel(GameObject o)
	{
		// if edit exists destroy before making a new panel
		if(edit != null)
		{
			Destroy(edit);
		}

		// initialize the array
		editButtons = new Button[2];

		// create the panel
		edit = Instantiate(editPanel) as GameObject;

		// set the parent of edit to be the canvas and position the panel correctly
		edit.transform.SetParent(canvas.transform);
		edit.transform.localPosition = editPanel.transform.position;

		// find and create a listener for the panel
		for (int i = 0; i < editButtons.Length; i++)
		{
			editButtons[i] = edit.transform.GetChild(11 + i).GetComponent<Button>();
		}
		editButtons[0].onClick.AddListener(delegate { CreateObject(o); });
		editButtons[1].onClick.AddListener(delegate { CancelEditPanel(); });

		// set up the value of the gameobject to be spawned
		edit.transform.GetChild(0).GetComponent<Text>().text = o.name;
		//SetEditPanelValues();
	}

	/// <summary>
	/// cancel the overall creation panel and re-enable the create menu button
	/// </summary>
	public void CancelCreatePanel()
	{
		for (int i = 0; i < availableObjects.Length; i++)
		{
			GameObject.Destroy(availableObjects[i]);
		}
		GameObject.Destroy(edit);
		ShowCreateButton();
	}

	/// <summary>
	/// closes the edit panel
	/// </summary>
	public void CancelEditPanel()
	{
		GameObject.Destroy(edit);
	}

	/// <summary>
	/// creates the new object
	/// </summary>
	/// <param name="o"></param>
	public void CreateObject(GameObject o)
	{

		// checks if any of the fields are empty if so spawns an object with default values
		// spawns an object with user input values
		if (posXText != string.Empty && posYText != string.Empty && posZText != string.Empty)
		{
			GameObject i = Instantiate(o, new Vector3(float.Parse(posXText), float.Parse(posYText), float.Parse(posZText)), Quaternion.identity) as GameObject;
			i.name += nextNum;
			if (massText != string.Empty && float.Parse(massText) <= 1)
				i.GetComponent<MyRigidBody>().mass = float.Parse(massText);
			else
			{
				//edit.transform.GetChild(6).transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = "Field Required";
				i.GetComponent<MyRigidBody>().mass = 1;
			}
			if (restitutionText != string.Empty)
				i.GetComponent<MyRigidBody>().restitution = float.Parse(restitutionText);
			else
			{
				i.GetComponent<MyRigidBody>().restitution = 0;
			}
			if (sFrictionText != string.Empty)
				i.GetComponent<MyRigidBody>().staticFriction = float.Parse(sFrictionText);
			else
			{
				i.GetComponent<MyRigidBody>().staticFriction = 0;
			}
			if (dFrictionText != string.Empty)
				i.GetComponent<MyRigidBody>().dynamicFriction = float.Parse(dFrictionText);
			else
			{
				i.GetComponent<MyRigidBody>().dynamicFriction = 0;
			}
		}
		else
		{
			GameObject i = Instantiate(o) as GameObject;
			i.name += nextNum;
			i.GetComponent<MyRigidBody>().mass = 1;
			i.transform.position = new Vector3(0, 0.5f, 0);
		}
		// advance the number to change the name
		nextNum++;

		// delete the create panel after an object has been created
		CancelCreatePanel();
		HideCancelButton();
	}

	/// <summary>
	/// get the user input values and store them for use
	/// </summary>
	public void GetEditPanelValues()
	{
		if (edit != null)
		{
			massText = edit.transform.GetChild(6).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text.ToString();
			restitutionText = edit.transform.GetChild(7).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text.ToString();
			sFrictionText = edit.transform.GetChild(8).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text.ToString();
			dFrictionText = edit.transform.GetChild(9).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text.ToString();
			posXText = edit.transform.GetChild(10).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text.ToString();
			posYText = edit.transform.GetChild(10).transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text.ToString();
			posZText = edit.transform.GetChild(10).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().text.ToString();
		}
	}
}
