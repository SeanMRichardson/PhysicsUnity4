using UnityEngine;
using System.Collections;

public class MySphereCollider : MyRigidBody
{
	public float radius;
	public Vector3 centre;

	private float tensorValue;
	private Transform collider;

	void OnEnable()
	{
		if (RigidBodyType == global::RigidBodyType.SOLID_SPHERE)
		{
			tensorValue = 0.4f * Mathf.PI * Mathf.Pow(radius, 2);
			inertiaTensor = new Vector3(tensorValue, tensorValue, tensorValue);
		}
		else if(RigidBodyType == global::RigidBodyType.HOLLOW_SPHERE)
		{
			tensorValue = (2f/3f) * Mathf.PI * Mathf.Pow(radius, 2);
			inertiaTensor = new Vector3(tensorValue, tensorValue, tensorValue);
		}
	}

	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			collider = this.gameObject.transform.GetChild(i);
			collider.localScale = this.gameObject.transform.localScale;
		}
		
		distanceToSurface = radius;
		base.Start();
	}

	void Update()
	{
		centre = transform.position;	
	}

	//// Update is called once per frame
	//void FixedUpdate () 
	//{
	//    // call the base version of the fixed update to handle physics for the object
	//    base.FixedUpdate();

	//    // check for collisions against other game objects in the scene
	//    foreach (var obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
	//    {
	//        if (obj.GetComponent<MyPlaneCollider>() != null)
	//        {
	//            var collision = CheckCollision((MyRigidBody)obj.GetComponent<MyPlaneCollider>());
	//            if (collision != null)
	//            {
	//                // we have collided so we need to do something about it
	//                ResolveCollision(collision);
	//            }
	//        }
	//        if (obj.GetComponent<MySphereCollider>() != null)
	//        {
	//            var collision = CheckCollision((MyRigidBody)obj.GetComponent<MySphereCollider>());
	//            if (collision != null)
	//            {
	//                // we have collided so we need to do something about it
	//                ResolveCollision(collision);
	//            }
	//        }
	//    }
	}

	/// <summary>
	/// check to see if a collision has occured with any other rigid bodies
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	//public override MyCollision CheckCollision(MyRigidBody other)
	//{
	//    // declare null MyCollision for return
	//    MyCollision collision = null;

	//    // check for collision with other spheres
	//    if (other is MySphereCollider && other.name != name)
	//    {
	//        // combined radius of both spheres
	//        float combinedRadius = this.radius + ((MySphereCollider)other).radius;

	//        // the vector between the centres of the spheres
	//        Vector3 direction = other.transform.position - transform.position;

	//        // distance between centres
	//        float centreDistance = direction.magnitude;

	//        // normalise the direction
	//        direction /= centreDistance;

	//        // check interection
	//        if (centreDistance < combinedRadius)
	//        {
	//            // create a new collision object, containing the vector of the normal and the distance from the sphere
	//            collision = new MyCollision(this, other, direction, centreDistance);
	//        }
	//    }
	//    else if (other is MyPlaneCollider) //check for collision with a plane
	//    {
	//        // get the mesh filter for the plane
	//        MeshFilter filter = other.gameObject.GetComponent<MeshFilter>();

	//        // make sure it has a mesh filter
	//        if (filter && filter.mesh.normals.Length > 0)
	//        {
	//            // get one of the vertext normals --- first one should do as the are all the same
	//            var normal = filter.transform.TransformDirection(filter.mesh.normals[0]);

	//            // distance of sphere centre from plane
	//            float distanceFromSphereCentre = Mathf.Abs(Vector3.Dot(normal, transform.position)) + (other as MyPlaneCollider).distance;

	//            // distance of sphere from plane
	//            float distanceFromSphere = distanceFromSphereCentre - radius;

	//            // check for intersection
	//            if (distanceFromSphere < 0)
	//            {
	//                // create a new collision object, containing the vector of the normal and the distance from the plane
	//                collision = new MyCollision(this, other, normal, distanceFromSphere);
	//            }
	//        }
	//    }
	//    //else if (other is MyBoxCollider && other.name != name)
	//    //{
	//    //    // combined radius of both spheres
	//    //    float combinedRadius = this.radius + ((MySphereCollider)other).radius;

	//    //    // the vector between the centres of the spheres
	//    //    Vector3 direction = other.transform.position - transform.position;

	//    //    // distance between centres
	//    //    float centreDistance = direction.magnitude;

	//    //    // normalise the direction
	//    //    direction /= centreDistance;

	//    //    // check interection
	//    //    if (centreDistance < combinedRadius)
	//    //    {
	//    //        // create a new collision object, containing the vector of the normal and the distance from the sphere
	//    //        collision = new MyCollision(this, other, direction, centreDistance);
	//    //    }
	//    //}
	//    // return the collision
	//    return collision;
	//}