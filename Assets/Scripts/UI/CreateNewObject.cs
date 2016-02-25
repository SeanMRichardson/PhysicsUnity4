using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreateNewObject : MonoBehaviour {

	public GameObject[] availableObjects;// list of all the types of creatable objects

	public GameObject prefabPanel;// the panels spawned for the list
	public GameObject editPanel;// the panel for the edit window on an object
	GameObject edit;// the cloned copy of the edit panel

	public Canvas canvas;// the canvas
	GameObject panel;//the cloned copy of the list panel

	RectTransform rect;// the rectTransform of panel

	Vector3 pos = new Vector3(-470, 280);// spawn position of the first panel in the list 
	Vector3 offset = new Vector3(0,-140,0);// the offset for the rest of the tiles

	Button[] buttons;// the list of the buttons on the panels

	// Use this for initialization
	void Start () 
	{
		// initialize the avilable objects
		availableObjects = new GameObject[3];
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}

	public void CreateMenu()
	{
		// initialize the list of buttons
		buttons = new Button[3];

		// create the panel list on screen and add a button listener
		for(int i = 0; i < availableObjects.Length; i++)
		{
			panel = Instantiate(prefabPanel) as GameObject;
			panel.transform.SetParent(canvas.transform);
			rect = panel.GetComponent<RectTransform>();
			rect.transform.localPosition = new Vector3(pos.x + (offset.x * i), pos.y + (offset.y * i));
			buttons[i] = rect.GetComponent<Button>();
			buttons[i].onClick.AddListener(delegate() { OpenEditPanel(); });
		}
	}

	/// <summary>
	/// makes the pressed button uninteractable 
	/// </summary>
	public void HideButton()
	{
		canvas.transform.GetChild(0).GetComponent<Button>().interactable = false;
	}

	//show the editing panel
	public void OpenEditPanel()
	{

		edit = Instantiate(editPanel) as GameObject;

		// set the parent of edit to be the canvas and position the panel correctly
		edit.transform.SetParent(canvas.transform);
		edit.transform.localPosition = editPanel.transform.position;
	}
}
