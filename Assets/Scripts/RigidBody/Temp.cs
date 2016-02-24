using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Temp : MonoBehaviour 
{
	public Vector3 velocity; // the current velocity of the rigidbody
	public Vector3 netForce; // the current overall force acting on the rigidbody
	public List<Vector3> forces; // the list of all forces acting on the rigidbody
	public float mass; // the mass of the rigidbody in kilograms
	public float restitution; // the bounciness of the rigidbody
	public float inverseMass; // 1 / mass;
	public float staticFriction; // the friction of the stationary rigidbody
	public float dynamicFriction; // the friction of the moving rigidbody
	public Vector3 inertiaTensor; // the inertia tensor of the object
	public Vector3 angularVelocity; // the angular velocity of the object

	private Matrix4x4 localI; // matrix representation of the inertia tensor
	public Vector3 globalL; // the angular momentum in world space

	// Use this for initialization
	protected void Start () 
	{
		//get the inverse mass
		inverseMass = 1 / mass;

		localI = Matrix4x4.Scale(inertiaTensor);
	}
	
	// Update is called once per frame
	protected void FixedUpdate () 
	{
		globalL = localI * angularVelocity;
		
		// add all the forces
		AddForces();

		// update the velocity
		UpdateVelocity();

		// update the position
		transform.position += velocity * Time.deltaTime;

		// calculate rotation
		UpdateRotation();
	}

	/// <summary>
	/// checks for a collision between this and another MyRigidBody
	/// implemented on each sub class
	/// 
	/// TODO: make it only happen here
	/// 
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	//public abstract MyCollision CheckCollision(MyRigidBody other);

	public void ResolveCollision(MyCollision collision)
	{
		Vector4 angularMomentum;
		// if any collider collides with a plane
		if((collision.bodyA is MyPlaneCollider && (collision.bodyB is MySphereCollider || collision.bodyB is MyBoxCollider)) 
			|| ((collision.bodyA is MySphereCollider || collision.bodyA is MyBoxCollider) && collision.bodyB is MyPlaneCollider))
		{
			// calculate relative velocity
			Vector3 relativeVelocity = collision.bodyA.linearVelocity - collision.bodyB.linearVelocity;

			// calculate relative velocity along the normal of the plane
			float velocityAlongNormal = Vector3.Dot(relativeVelocity, collision.collisionNormal);

			// do not resolve if velocities are separating
			if (velocityAlongNormal > 0)
				return;

			// calculate restitution
			float e = Mathf.Min(collision.bodyA.restitution, collision.bodyB.restitution);

			// calculate impulse scalar
			float j = -(1 + e) * velocityAlongNormal;

			//Vector4 bodyA = collision.bodyA.localI.inverse * (Mathf.Pow(((MySphereCollider)(collision.bodyA)).radius, 2) * collision.collisionNormal);
			//Vector4 bodyB = collision.bodyB.localI.inverse * (Mathf.Pow(((MySphereCollider)(collision.bodyB)).radius, 2) * collision.collisionNormal);

			j /= collision.bodyA.inverseMass + collision.bodyB.inverseMass;
			//j /= collision.bodyA.inverseMass + collision.bodyB.inverseMass + Vector3.Dot((new Vector3(bodyA.x, bodyA.y, bodyA.z) + new Vector3(bodyB.x, bodyB.y, bodyB.z)), collision.collisionNormal);

			// apply impulse
			Vector3 impulse = j * collision.collisionNormal;
			collision.bodyA.linearVelocity += collision.bodyA.inverseMass * impulse;
			collision.bodyB.linearVelocity -= collision.bodyB.inverseMass * impulse;

			// re-calculate the relative velocity
			relativeVelocity = collision.bodyA.linearVelocity - collision.bodyB.linearVelocity;

			// get the tangent vector
			Vector3 tangent = Vector3.Dot(relativeVelocity, collision.collisionNormal) * collision.collisionNormal;
			tangent.Normalize();

			// get the magnitude of force along the friction vector
			float jt = -Vector3.Dot(relativeVelocity, tangent);
			jt /= collision.bodyA.inverseMass + collision.bodyB.inverseMass;
			//jt /= collision.bodyA.inverseMass + Vector3.Dot(new Vector3(bodyA.x / collision.collisionNormal.x, bodyA.y / collision.collisionNormal.y, bodyA.z / collision.collisionNormal.z), tangent);


			// get the average co-efficient of friction
			float mu = (collision.bodyA.staticFriction + collision.bodyB.staticFriction) / 2;

			// clamp the magnitude of friction and create impulse vector
			Vector3 frictionImpulse;
			if (Mathf.Abs(jt) < j * mu)
			{
				frictionImpulse = jt * tangent;
			}
			else
			{
				dynamicFriction = (collision.bodyA.dynamicFriction + collision.bodyB.dynamicFriction) / 2;
				frictionImpulse = -j * tangent * dynamicFriction;
			}

			// apply the impulse
			collision.bodyA.linearVelocity += collision.bodyA.inverseMass * frictionImpulse;
			collision.bodyB.linearVelocity -= collision.bodyB.inverseMass * frictionImpulse;

			if ((collision.bodyA is MySphereCollider) && collision.bodyB is MyPlaneCollider)
			{
				angularMomentum = (Matrix4x4.Scale(collision.bodyA.inertiaTensor).inverse * new Vector4(impulse.x, impulse.y, impulse.z) * ((MySphereCollider)(collision.bodyA)).radius);
				collision.bodyA.angularVelocity -= new Vector3(angularMomentum.x, angularMomentum.y, angularMomentum.z);
				
			}
			else if (collision.bodyA is MyPlaneCollider && (collision.bodyB is MySphereCollider))
			{
				angularMomentum = (Matrix4x4.Scale(collision.bodyB.inertiaTensor).inverse * new Vector4(impulse.x, impulse.y, impulse.z) * ((MySphereCollider)(collision.bodyB)).radius);
				collision.bodyB.angularVelocity += new Vector3(angularMomentum.x, angularMomentum.y, angularMomentum.z);
			}

			// correct the position
			PositionalCorrection(collision);
		}

		if ((collision.bodyA is MySphereCollider && collision.bodyB is MySphereCollider) 
		|| (collision.bodyA is MyBoxCollider && collision.bodyB is MySphereCollider))
		{
			// calculate relative velocity
			Vector3 relativeVelocity = collision.bodyB.linearVelocity - collision.bodyA.linearVelocity;

			// calculate relative velocity in terms of the normal direction
			float velocityAlongNormal = Vector3.Dot(relativeVelocity, collision.collisionNormal);

			// do not resolve if velocities are separating
			if (velocityAlongNormal > 0)
				return;

			// calculate restitution
			float e = Mathf.Min(collision.bodyA.restitution, collision.bodyB.restitution);

			// calculate impulse scalar
			float j = -(1 + e) * velocityAlongNormal;
			j /= collision.bodyA.inverseMass + collision.bodyB.inverseMass;

			// apply impulse
			Vector3 impulse = j * collision.collisionNormal;
			collision.bodyA.linearVelocity -= collision.bodyA.inverseMass * impulse;
			collision.bodyB.linearVelocity += collision.bodyB.inverseMass * impulse;

			// recalcuate the relative velocity
			relativeVelocity = collision.bodyA.linearVelocity - collision.bodyB.linearVelocity;

			// get the tangent vector
			Vector3 tangent = Vector3.Dot(relativeVelocity, collision.collisionNormal) * collision.collisionNormal;
			tangent.Normalize();

			// get the magnitude of the force along the friction vactor
			float jt = -Vector3.Dot(relativeVelocity, tangent);
			jt /= collision.bodyA.inverseMass + collision.bodyB.inverseMass;

			// get average coefficioent of friction in collision
			float mu = (collision.bodyA.staticFriction + collision.bodyB.staticFriction) / 2;

			// clamp magnitude of friction and create impulse vector
			Vector3 frictionImpulse;
			if (Mathf.Abs(jt) < j * mu)
				frictionImpulse = jt * tangent;
			else
			{
				dynamicFriction = (collision.bodyA.dynamicFriction + collision.bodyB.dynamicFriction) / 2;
				frictionImpulse = -j * tangent * dynamicFriction; ;
			}

			// apply the impulse
			collision.bodyA.linearVelocity -= collision.bodyA.inverseMass * frictionImpulse;
			collision.bodyB.linearVelocity += collision.bodyB.inverseMass * frictionImpulse;

			// conserve angular momentum
			collision.bodyA.angularVelocity -= new Vector3((impulse.x * ((MySphereCollider)(collision.bodyA)).radius) / collision.bodyA.inertiaTensor.x, (impulse.y * ((MySphereCollider)(collision.bodyA)).radius) / collision.bodyA.inertiaTensor.y, (impulse.z * ((MySphereCollider)(collision.bodyA)).radius) / collision.bodyA.inertiaTensor.z);
			collision.bodyB.angularVelocity += new Vector3((impulse.x * ((MySphereCollider)(collision.bodyB)).radius) / collision.bodyB.inertiaTensor.x, (impulse.y * ((MySphereCollider)(collision.bodyB)).radius) / collision.bodyB.inertiaTensor.y, (impulse.z * ((MySphereCollider)(collision.bodyB)).radius) / collision.bodyB.inertiaTensor.z);
		}
	}

	/// <summary>
	/// deal with floating point errors in the positional calculations
	/// </summary>
	/// <param name="collision"></param>
	public void PositionalCorrection(MyCollision collision)
	{
		float percent = 0.2f; // usually 20% to 80%
		float slop = 0.01f; // usually 0.01 to 0.1

		if (collision.bodyA is MyBoxCollider || collision.bodyB is MyBoxCollider)
			percent = 0.01f;
		Vector3 correction = (Mathf.Max(Mathf.Abs(collision.penetration - slop), 0.0f) / (collision.bodyA.inverseMass + collision.bodyB.inverseMass)) * percent * collision.collisionNormal;
		collision.bodyA.transform.position += collision.bodyA.inverseMass * correction;
		collision.bodyB.transform.position -= collision.bodyB.inverseMass * correction;
	}

	/// <summary>
	/// add up all of the component forces on the object
	/// </summary>
	public void AddForces()
	{
		// reset the net force
		netForce = Vector3.zero;

		// combine all forces
		foreach (var force in forces)
			netForce += force;
	}

	/// <summary>
	/// update the current velocity based on acceleration
	/// </summary>
	public void UpdateVelocity()
	{
		// calculate the current aceleration
		Vector3 acceleration = netForce / mass;

		// update the velocity
		velocity += acceleration * Time.deltaTime;
	}

	/// <summary>
	/// update the rotational position of the object
	/// </summary>
	public void UpdateRotation()
	{
		// gets the rotation matrix from the rotation vector
		Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);

		// transform the local space inertia tensor to world space
		Matrix4x4 globalI = rotationMatrix * localI * rotationMatrix.inverse;

		// calculate the angular velocity in world space
		Vector3 globalW = globalI.inverse * globalL;

		// work out the nuber of degrees to rotate based on the magnitude 
		// and direction of the angular velocity
		Vector3 globalRotationAxis = globalW.normalized;
		float speed = globalW.magnitude;
		float degreesThisFrame = speed * Time.deltaTime * Mathf.Rad2Deg;
		transform.RotateAround(transform.position, globalRotationAxis, degreesThisFrame);
	}
}
