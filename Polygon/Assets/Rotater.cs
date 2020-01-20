using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public GameObject objectToRotate;

    public Vector3 axis;

    public float rotationSpeed = 5f;


    private void Update()
    {
        objectToRotate.transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, objectToRotate.transform.InverseTransformDirection(axis));
    }

    private void OnDrawGizmos()
    {
		Gizmos.color = Color.red;
		Gizmos.DrawRay(objectToRotate.transform.position, axis);
	}
}

