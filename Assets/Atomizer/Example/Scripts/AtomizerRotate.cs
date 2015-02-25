using UnityEngine;
using System.Collections;

public class AtomizerRotate : MonoBehaviour 
{
	public Vector3 axis;
	
	void LateUpdate() 
	{
		transform.Rotate(axis.x * Time.deltaTime, axis.y * Time.deltaTime, axis.z * Time.deltaTime);		
	}
}
