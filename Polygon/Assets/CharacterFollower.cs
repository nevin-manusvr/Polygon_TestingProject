using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFollower : MonoBehaviour
{
	public Transform target;

	[Header("Position")]
	public Vector3 dir;
	public float distance;
	public float height;

	[Header("LookAt")]
	public float lookAtHeight;

	[Header("Smoothing")]
	public float positionSmoothing;
	public float rotationSmoothing;

	private Vector3 position;
	private Vector3 positionVel;

	private Vector3 lookAtPosition;
	private Vector3 lookAtPositionVel;

	private void LateUpdate()
	{
		Vector3 pos = target.position + (target.rotation * dir).normalized * distance;
		pos.y = height;
		position = Vector3.SmoothDamp(position, pos, ref positionVel, positionSmoothing);

		transform.position = position;

		Vector3 lookAtPos = target.position;
		lookAtPos.y = lookAtHeight;
		lookAtPosition = Vector3.SmoothDamp(lookAtPosition, lookAtPos, ref lookAtPositionVel, rotationSmoothing);

		transform.LookAt(lookAtPosition);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(target.position, (target.rotation * dir).normalized);
	}
}
