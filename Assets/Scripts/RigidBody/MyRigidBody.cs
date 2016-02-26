using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum RigidBodyType
{
    PLANE,
    SOLID_SPHERE,
    HOLLOW_SPHERE,
    SOLID_BOX
}

public class MyRigidBody : MonoBehaviour
{
    public RigidBodyType RigidBodyType;
    public Vector3 linearVelocity; // the current velocity of the rigidbody
    public Vector3 acceleration;
    public Vector3 netForce; // the current overall force acting on the rigidbody
    public List<Vector3> forces; // the list of all forces acting on the rigidbody
    public float mass; // the mass of the rigidbody in kilograms
    public float restitution; // the bounciness of the rigidbody
    public float inverseMass; // 1 / mass;
    public float staticFriction; // the friction of the stationary rigidbody
    public float dynamicFriction; // the friction of the moving rigidbody
    public Vector3 inertiaTensor; // the inertia tensor of the object
    public Vector3 angularVelocity; // the angular velocity of the object
    public Vector3 angularMomentum;
    public Vector3 linearMomentum;
    public bool selected = false;

    public float distanceToSurface; // the distance from the centre of the object to its surface e.g. for a sphere it's radius 

    private SelectObject sel;
    private Matrix4x4 localI; // matrix representation of the inertia tensor
    private Vector3 globalL; // the angular momentum in world space

    // Use this for initialization
    protected void Start()
    {
        sel = GameObject.Find("Engine").GetComponent<SelectObject>();
        // set the inverse mass
        inverseMass = 1 / mass;

        // set the matrix representation of the inertia tensor
        localI = Matrix4x4.Scale(inertiaTensor);
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        // planes don't move
        if (this.RigidBodyType != global::RigidBodyType.PLANE)
        {
            // check for collisions
            CheckForCollisions();

            // add all the forces
            //AddForces();

            // update the velocity
            
            UpdateMomentum();
            UpdatePosition();

            //// update the position
            //transform.position += velocity * Time.fixedDeltaTime;

            // calculate rotation
            //UpdateRotation();           
        }
        // update the ui text for the text which changes
        if(selected)
        {
            sel.stats.transform.GetChild(12).transform.GetChild(1).GetComponent<Text>().text = "x = " + linearVelocity.x + ", " + "y = " + linearVelocity.y + ", " + "z = " + linearVelocity.z;
            sel.stats.transform.GetChild(13).transform.GetChild(1).GetComponent<Text>().text = "x = " + angularVelocity.x + ", " + "y = " + angularVelocity.y + ", " + "z = " + angularVelocity.z;
        }
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
        //linear momentum = mass * linear velocity
        //angularmomentum = radius * mass * angularVelocity * tangent

        // calculate the current aceleration
        acceleration = netForce / mass;

        // update the velocity
        linearVelocity += acceleration * Time.fixedDeltaTime;
    }

    public void UpdatePosition()
    {
        transform.position += linearVelocity * Time.fixedDeltaTime;
        Vector3 spin = AngularVelocityToSpin();
        transform.eulerAngles += spin * Time.fixedDeltaTime;
    }

    public void UpdateMomentum()
    {
        //const float MaxAngularMomentum = 10;

        //// todo: I'd like to clamp at maximum angular VELOCITY not momentum
        //// i only care how fast it is rotating, not how much mass it has

        //float x = Mathf.Clamp(angularMomentum.x, -MaxAngularMomentum, MaxAngularMomentum);
        //float y = Mathf.Clamp(angularMomentum.y, -MaxAngularMomentum, MaxAngularMomentum);
        //float z = Mathf.Clamp(angularMomentum.z, -MaxAngularMomentum, MaxAngularMomentum);

        //angularMomentum = new Vector3(x, y, z);

        linearMomentum += new Vector3(0, -9.81f, 0) * mass * Time.fixedDeltaTime ;

        linearVelocity = linearMomentum * inverseMass;
        angularVelocity = Vector3.Scale(inertiaTensor, angularMomentum);

        
    }

    /// <summary>
    /// check for collisions with all other objects
    /// </summary>
    public void CheckForCollisions()
    {
        foreach (var obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            if (obj.GetComponent<MyPlaneCollider>() != null && obj.name != this.name)
            {
                var collision = CheckCollision((MyRigidBody)obj.GetComponent<MyPlaneCollider>());
                if (collision != null)
                {
                    // we have collided so we need to do something about it
                    ResolveCollision(collision);
                }
            }
            if (obj.GetComponent<MySphereCollider>() != null && obj.name != this.name)
            {
                var collision = CheckCollision((MyRigidBody)obj.GetComponent<MySphereCollider>());
                if (collision != null)
                {
                    // we have collided so we need to do something about it
                    ResolveCollision(collision);
                }
            }
        }
    }

    /// <summary>
    /// Checks for collisions with other bodies
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public MyCollision CheckCollision(MyRigidBody other)
    {
        // no collision by default
        MyCollision collision = null;

        // only check for collisions on bodies that we are interested in
        switch (this.RigidBodyType)
        {
            case global::RigidBodyType.SOLID_SPHERE:
            case global::RigidBodyType.HOLLOW_SPHERE:
            case global::RigidBodyType.SOLID_BOX:
                collision = Collided(this, other);
                break;
            default:
                break;
        }
        return collision;
    }

    private MyCollision Collided(MyRigidBody object1, MyRigidBody object2)
    {
        MyCollision collision = null;
        switch(object2.RigidBodyType)
        {
            case global::RigidBodyType.PLANE:
                collision =  CollidedWithPlane(object1, object2 as MyPlaneCollider);
                break;
            case global::RigidBodyType.SOLID_SPHERE:
            case global::RigidBodyType.HOLLOW_SPHERE:
                collision = CollidedWithSphere(object1, object2 as MySphereCollider);
                break;
            case global::RigidBodyType.SOLID_BOX:
                collision = CollidedWithBox(object1, object2 as MyBoxCollider);
                break;
            default:
                break;

        }
        return collision;
    }

    /// <summary>
    /// check if input body has collided with plane
    /// </summary>
    /// <param name="body"></param>
    /// <param name="plane"></param>
    /// <returns></returns>
    private MyCollision CollidedWithPlane(MyRigidBody body, MyPlaneCollider plane)
    {
        // default to no collision
        MyCollision collision = null;

        // get the mesh filter for the plane
        MeshFilter filter = plane.gameObject.GetComponent<MeshFilter>();

        // make sure it has a mesh filter
        if (filter && filter.mesh.normals.Length > 0)
        {
            // get one of the vertext normals --- first one should do as the are all the same
            var normal = filter.transform.TransformDirection(filter.mesh.normals[0]);

            // distance of sphere centre from plane
            float distanceFromSphereCentre = Mathf.Abs(Vector3.Dot(normal, transform.position)) + (plane as MyPlaneCollider).distance;

            // distance of sphere from plane
            float distanceFromSphere = distanceFromSphereCentre - distanceToSurface;

            // check for intersection
            if (distanceFromSphere < 0)
            {
                // create a new collision object, containing the vector of the normal and the distance from the plane
                collision = new MyCollision(this, plane, new Vector3(transform.position.x, transform.position.y - ((MySphereCollider)body).radius, transform.position.z), normal, distanceFromSphere);
            }
        }
        return collision;
    }

    /// <summary>
    /// Check if the input body has corrected with a sphere
    /// </summary>
    /// <param name="body"></param>
    /// <param name="sphere"></param>
    /// <returns></returns>
    private MyCollision CollidedWithSphere(MyRigidBody body, MySphereCollider sphere)
    {
        // default to no collision
        MyCollision collision = null;

        // combined radius of both spheres
        float combinedRadius = this.distanceToSurface + ((MySphereCollider)sphere).radius;

        // the vector between the centres of the spheres
        Vector3 direction = sphere.transform.position - transform.position;

        // distance between centres
        float centreDistance = direction.magnitude;

        // normalise the direction
        direction /= centreDistance;

        // check interection
        if (centreDistance < combinedRadius)
        {
            // create a new collision object, containing the vector of the normal and the distance from the sphere
            collision = new MyCollision(this, sphere, new Vector3(transform.position.x, transform.position.y - ((MySphereCollider)body).radius, transform.position.z), direction, centreDistance);
        }

        return collision;
    }

    private MyCollision CollidedWithBox(MyRigidBody body, MyBoxCollider plane)
    {
        return null;
    }

    /// <summary>
    /// Resolve a collision with another rigid body
    /// </summary>
    /// <param name="collision"></param>
    public void ResolveCollision(MyCollision collision)
    {
        // if any collider collides with a plane
        if ((collision.bodyA is MySphereCollider || collision.bodyA is MyBoxCollider) && collision.bodyB is MyPlaneCollider)
        {
            // calculate relative velocity
            //Vector3 relativeVelocity = collision.bodyB.velocity - collision.bodyA.velocity;
            Vector3 velAtPoint = new Vector3();
            velAtPoint = GetVelocityAtPoint(collision.contactPoint, velAtPoint);

            // calculate relative velocity along the normal of the plane
            float velocityAlongNormal = Mathf.Min(0, Vector3.Dot(velAtPoint, collision.collisionNormal));

            // do not resolve if velocities are separating
            if (velocityAlongNormal > 0)
                return;

            // calculate restitution
            float e = Mathf.Min(collision.bodyA.restitution, collision.bodyB.restitution);

            // calculate impulse scalar
            float j = -(1 + e) * velocityAlongNormal;

            //Vector4 bodyA = collision.bodyA.localI.inverse * (Mathf.Pow(((MySphereCollider)(collision.bodyA)).radius, 2) * collision.collisionNormal);
            //Vector4 bodyB = collision.bodyB.localI.inverse * (Mathf.Pow(((MySphereCollider)(collision.bodyB)).radius, 2) * collision.collisionNormal);

            Vector3 r = collision.contactPoint - collision.bodyA.transform.position;

            //j /= collision.bodyA.inverseMass + collision.bodyB.inverseMass;
            j /= collision.bodyA.inverseMass + Vector3.Dot(Vector3.Cross(collision.bodyA.localI.inverse * Vector3.Cross(r, collision.collisionNormal), r), collision.collisionNormal);

            collision.bodyA.linearMomentum += j * collision.collisionNormal;
            collision.bodyA.angularMomentum += j * Vector3.Cross(r, collision.collisionNormal);

            // apply impulse
            //Vector3 impulse = j * collision.collisionNormal;
            //collision.bodyA.linearVelocity += collision.bodyA.inverseMass * linearMomentum;
            //collision.bodyB.velocity -= collision.bodyB.inverseMass * impulse;
            //UpdateMomentum();

            // re-calculate the relative velocity
            //relativeVelocity = collision.bodyB.velocity - collision.bodyA.velocity;
            velAtPoint = GetVelocityAtPoint(collision.contactPoint, velAtPoint);

            // get the tangent vector
            Vector3 tangentVelocity = velAtPoint - collision.collisionNormal * Vector3.Dot(velAtPoint, collision.collisionNormal);

            Vector3 tangent = Vector3.Normalize(tangentVelocity);

            // get the magnitude of force along the friction vector
            float jt = Vector3.Dot(velAtPoint, tangent);
            //jt /= collision.bodyA.inverseMass + collision.bodyB.inverseMass;
            jt /= collision.bodyA.inverseMass + Vector3.Dot(Vector3.Cross(collision.bodyA.localI.inverse * Vector3.Cross(r, collision.collisionNormal), r), tangent);

            collision.bodyA.linearMomentum += j * tangent * Time.fixedDeltaTime;
            collision.bodyA.angularMomentum += j * Vector3.Cross(r, tangent);

            // get the average co-efficient of friction
            float mu = (collision.bodyA.staticFriction + collision.bodyB.staticFriction) / 2;

            jt = Mathf.Clamp(-jt, -mu * j, mu * j);

            // clamp the magnitude of friction and create impulse vector
            //Vector3 frictionImpulse;
            //if (Mathf.Abs(jt) < j * mu)
            //{
            //    frictionImpulse = jt * tangentVelocity;
            //}
            //else
            //{
                //var averageDynamicFriction = (collision.bodyA.dynamicFriction + collision.bodyB.dynamicFriction) / 2;
                //frictionImpulse = -jt * averageDynamicFriction;
            //}

            
                // apply the impulse
                //collision.bodyA.linearMomentum += collision.bodyA.inverseMass * frictionImpulse * Time.fixedDeltaTime;
            collision.bodyA.linearMomentum += jt * tangent;
            collision.bodyA.angularMomentum += jt * Vector3.Cross(r, tangent);
                //collision.bodyB.velocity -= collision.bodyB.inverseMass * frictionImpulse;
            

            if ((collision.bodyA is MySphereCollider) && collision.bodyB is MyPlaneCollider)
            {
                //collision.bodyA.angularVelocity -= (collision.bodyA.localI.inverse * frictionImpulse * ((MySphereCollider)(collision.bodyA)).radius) * Time.fixedDeltaTime;
                //angularMomentum = collision.bodyA.mass * collision.bodyA.velocity * ((MySphereCollider)collision.bodyA).radius * Time.fixedDeltaTime;
                //collision.bodyA.angularVelocity = Matrix4x4.Scale(collision.bodyA.inertiaTensor).inverse * new Vector3(angularMomentum.x, angularMomentum.y, angularMomentum.z);
            }
            // correct the position
            PositionalCorrection(collision);
        }
        if (collision.bodyA is MySphereCollider && collision.bodyB is MySphereCollider)
        {
            // calculate relative velocity
            //Vector3 relativeVelocity = collision.bodyB.linearVelocity - collision.bodyA.linearVelocity;
            Vector3 velAtPoint = new Vector3();
            velAtPoint = GetVelocityAtPoint(collision.contactPoint, velAtPoint);

            // calculate relative velocity in terms of the normal direction
            float velocityAlongNormal = Mathf.Min(0, Vector3.Dot(velAtPoint, collision.collisionNormal));

            // do not resolve if velocities are separating
            if (velocityAlongNormal > 0)
                return;

            // calculate restitution
            float e = Mathf.Min(collision.bodyA.restitution, collision.bodyB.restitution);

            // calculate impulse scalar
            float j = -(1 + e) * velocityAlongNormal;

            Vector3 rA = collision.contactPoint - collision.bodyA.transform.position;
            Vector3 rB = collision.contactPoint - collision.bodyB.transform.position;

            //j /= collision.bodyA.inverseMass + collision.bodyB.inverseMass;
            j /= collision.bodyA.inverseMass + collision.bodyB.inverseMass + Vector3.Dot(Vector3.Cross(collision.bodyA.localI.inverse * Vector3.Cross(rA, collision.collisionNormal), rA) + Vector3.Cross(collision.bodyB.localI.inverse * Vector3.Cross(rB, collision.collisionNormal), rB), collision.collisionNormal);

            // apply impulse
            //Vector3 impulse = j * collision.collisionNormal;
            //collision.bodyA.linearVelocity -= collision.bodyA.inverseMass * impulse;
            
            //collision.bodyB.linearVelocity += collision.bodyB.inverseMass * impulse;

            collision.bodyA.linearMomentum -= j * collision.collisionNormal;
            collision.bodyA.angularMomentum -= j * Vector3.Cross(rA, collision.collisionNormal);

            collision.bodyB.linearMomentum += j * collision.collisionNormal;
            collision.bodyB.angularMomentum += j * Vector3.Cross(rB, collision.collisionNormal);

            // recalcuate the relative velocity
            //relativeVelocity = collision.bodyA.linearVelocity - collision.bodyB.linearVelocity;

            velAtPoint = GetVelocityAtPoint(collision.contactPoint, velAtPoint);

            // get the tangent vector
            //Vector3 tangent = Vector3.Dot(relativeVelocity, collision.collisionNormal) * collision.collisionNormal;
            //tangent.Normalize();

            Vector3 tangentVelocity = velAtPoint - collision.collisionNormal * Vector3.Dot(velAtPoint, collision.collisionNormal);
            Vector3 tangent = Vector3.Normalize(tangentVelocity);

            // get the magnitude of the force along the friction vactor
            float jt = -Vector3.Dot(velAtPoint, tangent);
            jt /= collision.bodyA.inverseMass + collision.bodyB.inverseMass + Vector3.Dot(Vector3.Cross(collision.bodyA.localI.inverse * Vector3.Cross(rA, tangent), rA) + Vector3.Cross(collision.bodyB.localI.inverse * Vector3.Cross(rB, tangent), rB), tangent);

            collision.bodyA.linearMomentum -= j * tangent;
            collision.bodyA.angularMomentum -= j * Vector3.Cross(rA, tangent);

            collision.bodyB.linearMomentum += j * tangent;
            collision.bodyB.angularMomentum += j * Vector3.Cross(rB, tangent);

            // get average coefficioent of friction in collision
            //float mu = (collision.bodyA.staticFriction + collision.bodyB.staticFriction) / 2;

            // clamp magnitude of friction and create impulse vector
            //Vector3 frictionImpulse;
            //if (Mathf.Abs(jt) < j * mu)
            //    frictionImpulse = jt * tangent;
            //else
            //{
            //    var averageDynamicFriction = (collision.bodyA.dynamicFriction + collision.bodyB.dynamicFriction) / 2;
            //    frictionImpulse = -j * tangent * averageDynamicFriction;
            //}

            // apply the impulse
            //collision.bodyA.linearMomentum = collision.bodyA.inverseMass * frictionImpulse * Time.fixedDeltaTime;
            //collision.bodyB.linearMomentum = collision.bodyB.inverseMass * frictionImpulse * Time.fixedDeltaTime;

            // conserve angular momentum
            //collision.bodyA.angularMomentum -= new Vector3((frictionImpulse.x * ((MySphereCollider)(collision.bodyA)).radius) / collision.bodyA.inertiaTensor.x, (frictionImpulse.y * ((MySphereCollider)(collision.bodyA)).radius) / collision.bodyA.inertiaTensor.y, (frictionImpulse.z * ((MySphereCollider)(collision.bodyA)).radius) / collision.bodyA.inertiaTensor.z);
            //collision.bodyB.angularMomentum += new Vector3((frictionImpulse.x * ((MySphereCollider)(collision.bodyB)).radius) / collision.bodyB.inertiaTensor.x, (frictionImpulse.y * ((MySphereCollider)(collision.bodyB)).radius) / collision.bodyB.inertiaTensor.y, (frictionImpulse.z * ((MySphereCollider)(collision.bodyB)).radius) / collision.bodyB.inertiaTensor.z);
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
        //collision.bodyB.transform.position -= collision.bodyB.inverseMass * correction;
    }

    /// <summary>
    /// update the rotational position of the object
    /// </summary>
    public void UpdateRotation()
    {
        // update the angular momentum
        globalL = localI * angularVelocity;

        // gets the rotation matrix from the rotation vector
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);

        // transform the local space inertia tensor to world space
        Matrix4x4 globalI = rotationMatrix * localI * rotationMatrix.inverse;

        // calculate the angular velocity in world space
        Vector3 globalW = globalI.inverse * globalL;

        // work out the nuber of degrees to rotate based on the magnitude 
        // and direction of the angular velocity
        Vector3 globalRotationAxis = angularVelocity.normalized;
        float speed = globalW.magnitude;
        float degreesThisFrame = speed * Time.deltaTime * Mathf.Rad2Deg;
        transform.RotateAround(transform.position, globalRotationAxis, degreesThisFrame);
    }

    Vector3 GetVelocityAtPoint(Vector3 point, Vector3 velocity)
    {
        Vector3 angVelocity = (localI.inverse * angularMomentum);
        velocity = linearVelocity + Vector3.Cross(angVelocity, point - transform.position);
        return velocity;
    }

    Vector3 AngularVelocityToSpin()
    {
        return Vector3.Scale(new Vector3(angularVelocity.x / 2, angularVelocity.y / 2, angularVelocity.z / 2), transform.eulerAngles);
    }

}