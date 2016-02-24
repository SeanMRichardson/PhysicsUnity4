﻿using UnityEngine;
using System.Collections;

public class MyCollision
{
    public MyRigidBody bodyA; // the first body 
    public MyRigidBody bodyB; // the second body
    public Vector3 contactPoint; // the point bodyA collides
    public Vector3 collisionNormal; // the normal along which the collision has taken place
    public float penetration; // the penetration of body A into body B

    /// <summary>
    /// Constructor to set member properties
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="normal"></param>
    /// <param name="p"></param>
    public MyCollision(MyRigidBody a, MyRigidBody b, Vector3 point, Vector3 normal, float p)
    {
        bodyA = a;
        bodyB = b;
        contactPoint = point;
        collisionNormal = normal;
        penetration = p;
    }
}
