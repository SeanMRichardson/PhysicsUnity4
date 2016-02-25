using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectObject : MonoBehaviour 
{
	public GameObject[] objects;// the gameobjects in the scene
	public GameObject rigidbodyStats;// the UI prefab for showing the rigidbodyStats stats
	public Canvas canvas;

	public GameObject[] statsObjsOnCanvas;// the Gameobjects on the canvas used for correctly creating rigidbodyStats prefab in game 
	public GameObject stats;// the clone of the rigidbodyStats prefab for changing values
	private Vector3 mousePosition;// the local mouse position

	// Use this for initialization
	void Start () 
	{
		// populate the objects array with the selectable items in the scene
		objects = GameObject.FindGameObjectsWithTag("Selectable");
	}
	
	// Update is called once per frame
	void Update()
	{
		// constantly check to see how many objects are on the canvas
		statsObjsOnCanvas = GameObject.FindGameObjectsWithTag("Stats");

		// convert the mouse position to screen space
		mousePosition = Input.mousePosition;
		mousePosition.z = 10.0f;
		mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

		foreach (GameObject g in objects)
		{
			// check to see if the mouse position is selecting the object and if the mouse has been clicked
			if (mousePosition.x < g.transform.position.x + 0.5
			&& mousePosition.y < g.transform.position.y + 0.5
			&& mousePosition.x > g.transform.position.x - 0.5
			&& mousePosition.y > g.transform.position.y - 0.5
			&& Input.GetMouseButtonUp(0)
			&& !g.GetComponent<MyRigidBody>().selected)
			{
				// show the collider sphere
				g.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
				g.GetComponent<MyRigidBody>().selected = true;

				// check if a stats panel can be created
				if (statsObjsOnCanvas.Length < 1)
				{
					CreateStatsPanel();
					SetStatsPanelValues(g);
				}
				else if( statsObjsOnCanvas.Length >= 1)
				{
					// destroy the object list before creating the new stats panel
					Destroy(statsObjsOnCanvas[0]);
					CreateStatsPanel();
					SetStatsPanelValues(g);
				}
			}
			// check for a mouse click away from a selectable object
			else if (g.GetComponent<MyRigidBody>().selected && Input.GetMouseButtonUp(0))
			{
				//destroy the stats panel if the object is unselected
				Destroy(statsObjsOnCanvas[0]);
				g.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
				g.GetComponent<MyRigidBody>().selected = false;
			}
		}
	}

	/// <summary>
	/// a function which creates a UI panel containing all the data for a MyRigidBody
	/// </summary>
	void CreateStatsPanel()
	{
		stats = Instantiate(rigidbodyStats) as GameObject;

		// set the parent of the stats to be the canvas and position the panel correctly
		stats.transform.SetParent(canvas.transform);
		stats.transform.localPosition = rigidbodyStats.transform.position;
		//UpdateStatsPanel();
	}

	/// <summary>
	/// a function which sets the values of the stats which do not change 
	/// </summary>
	/// <param name="o"></param>
	void SetStatsPanelValues(GameObject o)
	{
		stats.transform.GetChild(0).GetComponent<Text>().text = o.name;
		stats.transform.GetChild(8).transform.GetChild(0).GetComponent<Text>().text = o.GetComponent<MyRigidBody>().mass.ToString();
		stats.transform.GetChild(9).transform.GetChild(1).GetComponent<Text>().text = o.GetComponent<MyRigidBody>().restitution.ToString();
		stats.transform.GetChild(10).transform.GetChild(1).GetComponent<Text>().text = o.GetComponent<MyRigidBody>().staticFriction.ToString();
		stats.transform.GetChild(11).transform.GetChild(1).GetComponent<Text>().text = o.GetComponent<MyRigidBody>().dynamicFriction.ToString();
		stats.transform.GetChild(14).transform.GetChild(0).GetComponent<Text>().text = o.GetComponent<MyRigidBody>().RigidBodyType.ToString(); 
	}
}
