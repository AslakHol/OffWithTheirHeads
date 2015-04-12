using Hyper.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Component = UnityEngine.Component;

public class CameraRigManager : Singleton<CameraRigManager>
{

	//The main mouselook camera rig
	public GameObject DefaultRig;

	public GameObject MainCameras;
	public GameObject GUICameras;

	//The oculus camera rig
	public GameObject OculusRig;

	public GameObject OculusMainCameras;
	public GameObject OculusGUICameras;

	public Camera MainCamera
	{
		get;
		private set;
	}

	public Camera SecondaryCamera
	{
		get;
		private set;
	}

	public enum RigState
	{
		NONE,
		OCULUS
	}
	private RigState _state = RigState.NONE;
	public RigState State
	{
		get { return _state; }
		set
		{
			SetNewState(_state, value);
			_state = value;
			OnRigChanged(_state);
		}
	}

	public delegate void DelegateRigChanged(RigState state);
	public DelegateRigChanged OnRigChanged;
	private static readonly HashSet<Type> ignoredComponentTypes = new HashSet<Type>()
	{
		typeof(Camera),
		typeof(Transform),
		typeof(GUILayer),
		typeof(GameObject),
	//	typeof(MorpheusDistortionPass),
	//	typeof(MorpheusStereoCamera),
	};

	private List<MonoBehaviour> components = new List<MonoBehaviour>();
	private List<bool> allEnabled = new List<bool>();
	//---

	private void Awake()
	{
		OnRigChanged += delegate(RigState state) { };
	}

	// Use this for initialization
	private void Start () 
	{
		components.AddRange(MainCameras.transform.FindChild("Camera").GetComponents<MonoBehaviour>());
		RemoveIgnoredComponents(components);

		for (int i = 0; i < components.Count; ++i)
		{
			var component = components[i];

			if (component.GetType() == typeof(Behaviour))
				continue;

			allEnabled.Add(component.enabled);
			HyperTypeDescriptionProvider.Add(component.GetType());
		}
	}
	
	// Update is called once per frame
	void Update () {

		switch (_state)
		{
			case RigState.OCULUS:
				{
					UpdateSecondaryCameraValues(MainCamera, SecondaryCamera);
					break;
				}
			default:
				break;
		}
	}

	void SetNewState(RigState from, RigState to)
	{
		switch (to)
		{
			case RigState.NONE:
				{
					SetMainState(from);
					Debug.Log("Switching to Main Rig");
					break;
				}
			case RigState.OCULUS:
				{
					Debug.Log("Switching to Oculus Rig");
					SetOculusState(from);
					break;
				}
			default:
				break;
		}
	}

	void SetMainState(RigState from)
	{
		if(from == RigState.NONE)
		{
			Debug.LogWarning("Tried to switch to main camera rig but we're already using it");
			return;
		}

		DefaultRig.SetActive(true);
		OculusRig.SetActive(false);
		
		//Move child objects to the new rig, crosshair, inventory stuff etc

		GameObject mainFrom = null;
		GameObject guiFrom = null;

		if (from == RigState.OCULUS)
		{
			mainFrom = OculusMainCameras.FindChild("RightEyeAnchor");
			guiFrom = OculusGUICameras.FindChild("RightEyeAnchor");
		}

		var childObjectsMainCamera = mainFrom.transform.GetAllChildren();
		var childObjectsGUICamera = guiFrom.transform.GetAllChildren();

		foreach(var obj in childObjectsMainCamera)
		{
			obj.transform.parent = MainCameras.FindChild("Camera").transform;
		}

		foreach (var obj in childObjectsGUICamera)
		{
			obj.transform.parent = GUICameras.FindChild("Camera").transform;
		}

		//Now update the new cameras in assetmanager
		MainCamera = MainCameras.FindChild("Camera").GetComponent<Camera>();
		SecondaryCamera = MainCameras.FindChild("Camera").GetComponent<Camera>();
	}

	void SetOculusState(RigState from)
	{
		if (from == RigState.OCULUS)
		{
			Debug.LogWarning("Tried to switch to oculus rig but we're already using it");
			return;
		}

		DefaultRig.SetActive(false);
		OculusRig.SetActive(true);

		var rightCamera = OculusMainCameras.FindChild("RightEyeAnchor");
		var leftCamera = OculusMainCameras.FindChild("LeftEyeAnchor");
		
		//Copy components to OVRCameraRig Main CameraRight
		CopyComponentsFromCamera(rightCamera);
		CopyComponentsFromCamera(leftCamera);

		//Move child objects to the new rig, crosshair, inventory stuff etc
		GameObject mainFrom = null;
		GameObject guiFrom = null;

		if(from == RigState.NONE)
		{
			mainFrom = MainCameras.FindChild("Camera");
			guiFrom = GUICameras.FindChild("Camera");
		}

		var childObjectsMainCamera = mainFrom.transform.GetAllChildren();
		var childObjectsGUICamera = guiFrom.transform.GetAllChildren();

		foreach(var obj in childObjectsMainCamera)
		{
			obj.transform.parent = OculusMainCameras.FindChild("RightEyeAnchor").transform;
		}

		foreach (var obj in childObjectsGUICamera)
		{
			obj.transform.parent = OculusGUICameras.FindChild("RightEyeAnchor").transform;
		}


		MainCamera = OculusMainCameras.FindChild("RightEyeAnchor").GetComponent<Camera>();
		SecondaryCamera = OculusMainCameras.FindChild("LeftEyeAnchor").GetComponent<Camera>();
	}

	private void CopyComponentsFromCamera(GameObject toObject)
	{
		foreach (Component oldComponent in components)
		{
			if (toObject.GetComponent(oldComponent.GetType()) != null)
				continue;

			Component newComponent = toObject.AddComponent(oldComponent.GetType());

			foreach (FieldInfo info in oldComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				info.SetValue(newComponent, info.GetValue(oldComponent));
			}

			((Behaviour)newComponent).enabled = ((Behaviour)oldComponent).enabled;
		}
	}

	private void RemoveIgnoredComponents(List<MonoBehaviour> components)
	{
		components.RemoveAll(monoBehaviour => monoBehaviour == null || ignoredComponentTypes.Contains(monoBehaviour.GetType()));
	}

	private void UpdateSecondaryCameraValues(Camera mainCamera, Camera secondCamera)
	{
		for (int i = 0; i < components.Count; ++i)
		{
			Type type = components[i].GetType();
			MonoBehaviour mainComponent = (MonoBehaviour)mainCamera.GetComponent(type);

			if (!allEnabled[i] && !mainComponent.enabled)
				continue;

			MonoBehaviour secondaryComponent = (MonoBehaviour)secondCamera.GetComponent(type);

			if (secondaryComponent == null)
			{
				Debug.LogWarning("secondCamera is missing component '" + type.Name + '\'');
				continue;
			}
			FieldInfo[] content = mainComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (FieldInfo info in content)
			{
				object value = info.GetValue(mainComponent);
				if (value != null && value.GetType() == typeof(RenderTexture))
				{
					continue;
				}

				info.SetValue(secondaryComponent, value);
			}

			secondaryComponent.enabled = mainComponent.enabled;
			allEnabled[i] = mainComponent.enabled;
		}
	}
}
