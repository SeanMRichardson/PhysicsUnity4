using UnityEngine;
using System.Collections;

public class MyBoxCollider : MyRigidBody
{
	public Vector3 centrePos; // centre of the collider
	public Vector3 halfWidths; // the halfWidths of the sides of the box
	public float approxRadius;
	public Vector3[] axes = {   // normalized axes of the box
								new Vector3(1.0f, 0.0f, 0.0f), 
								new Vector3(0.0f, 1.0f, 0.0f), 
								new Vector3(0.0f, 0.0f, 1.0f)
							};

	// Use this for initialization
	void Start()
	{
		base.Start();
	}

	// Update is called once per frame
    //void FixedUpdate()
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
    //        if (obj.GetComponent<MyBoxCollider>() != null)
    //        {
    //            var collision = CheckCollision((MyRigidBody)obj.GetComponent<MyBoxCollider>());
    //            if (collision != null)
    //            {
    //                // we have collided so we need to do something about it
    //                ResolveCollision(collision);
    //            }
    //        }
    //    }
    //}

	/// <summary>
	/// check to see if a collision has occured with any other rigid bodies
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
    //public override MyCollision CheckCollision(MyRigidBody other)
    //{
    //    // declare null MyCollision for return
    //    MyCollision collision = null;

    //    // get the mesh filter for the plane
    //    MeshFilter filter = other.gameObject.GetComponent<MeshFilter>();

    //    // make sure it has a mesh filter
    //    if (filter && filter.mesh.normals.Length > 0)
    //    {
    //        // get one of the vertext normals --- first one should do as the are all the same
    //        var normal = filter.transform.TransformDirection(filter.mesh.normals[0]);

    //        approxRadius = halfWidths[0] * Mathf.Abs(Vector3.Dot(normal, axes[0])) +
    //                        halfWidths[1] * Mathf.Abs(Vector3.Dot(normal, axes[1])) +
    //                        halfWidths[2] * Mathf.Abs(Vector3.Dot(normal, axes[2]));

    //        // check for collision with other spheres
    //        if (other is MySphereCollider && other.name != name)
    //        {
    //            // combined radius of both spheres
    //            float combinedRadius = this.approxRadius + ((MySphereCollider)other).radius;

    //            // the vector between the centres of the spheres
    //            Vector3 direction = other.transform.position - transform.position;

    //            // distance between centres
    //            float centreDistance = direction.magnitude;

    //            // normalise the direction
    //            direction /= centreDistance;

    //            // check interection
    //            if (centreDistance < combinedRadius)
    //            {
    //                // create a new collision object, containing the vector of the normal and the distance from the sphere
    //                collision = new MyCollision(this, other, direction, centreDistance);
    //            }
    //        }

    //        else if (other is MyPlaneCollider) //check for collision with a plane
    //        {
    //            // the distance of the box's centre from the plane
    //            float distance = Mathf.Abs(Vector3.Dot(normal, transform.position)) - (other as MyPlaneCollider).distance;

    //            // check for intersection
    //            if (Mathf.Abs(distance) <= approxRadius)
    //            {
    //                // create a new collision object, containing the vector of the normal and the distance from the plane
    //                collision = new MyCollision(this, other, normal, distance);
    //            }
    //        }
    //    }
    //    // return the collision
    //    return collision;
    //}
}
