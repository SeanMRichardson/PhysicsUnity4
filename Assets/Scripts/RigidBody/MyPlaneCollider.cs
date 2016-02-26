using UnityEngine;
using System.Collections;

public class MyPlaneCollider : MyRigidBody 
{
    public Vector3 normal;
    public float distance;

    // Use this for initialization
    new void Start()
    {
        base.Start();
    }
}
