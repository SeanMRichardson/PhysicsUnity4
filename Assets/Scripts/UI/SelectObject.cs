using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectObject : MonoBehaviour 
{
	public GameObject[] objects;

	private Vector3 position;
	// Use this for initialization
	void Start () 
	{
		objects = GameObject.FindGameObjectsWithTag("Selectable");
	}
	
	// Update is called once per frame
	void Update () 
	{
		position = Input.mousePosition;
		position.z = 10.0f;
		position = Camera.main.ScreenToWorldPoint(position);

		foreach(GameObject g in objects)
		{

			if (position.x < g.transform.position.x + 0.5
			&& position.y < g.transform.position.y + 0.5
			&& position.x > g.transform.position.x - 0.5
			&& position.y > g.transform.position.y - 0.5
			&& Input.GetMouseButtonUp(0)
			&& !g.GetComponent<MyRigidBody>().selected)
			{
				g.transform.GetChild(0).renderer.enabled = true;
				g.GetComponent<MyRigidBody>().selected = true;
			}
			else if (g.GetComponent<MyRigidBody>().selected && Input.GetMouseButtonUp(0))
			{
				g.transform.GetChild(0).renderer.enabled = false;
				g.GetComponent<MyRigidBody>().selected = false;
			}
			
		}
	}
}
