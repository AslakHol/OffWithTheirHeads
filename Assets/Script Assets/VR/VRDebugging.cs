using UnityEngine;
using System.Collections;

public class VRDebugging : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F5))
		{
			CameraRigManager.Instance.State = CameraRigManager.RigState.NONE;
		}
		else if(Input.GetKeyDown(KeyCode.F6))
		{
			CameraRigManager.Instance.State = CameraRigManager.RigState.OCULUS;
		}
	}
}
