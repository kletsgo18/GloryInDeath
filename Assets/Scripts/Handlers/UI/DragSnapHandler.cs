﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSnapHandler : MonoBehaviour {

	public Vector3 heightOffset;

	public Transform characterModels;

	private Vector3 currentPosition;

	private Vector3 lastPosition;

	private CameraController camera;

	private int selectedIndex, lastSelected;

	private float rotationMovement;

	private Vector3 defaultPosition;

	private CharacterSelectUI characterSelectUI;

	void OnEnable () {

		EventManager.OnStateChange += OnStateChange;
	}

	void OnDisable ()
	{

		EventManager.OnStateChange -= OnStateChange;
	}
	void Start()
	{
		defaultPosition = characterModels.parent.transform.position + heightOffset;

		transform.position = defaultPosition;

		camera = CameraController.Instance;

		characterSelectUI = FindObjectOfType<CharacterSelectUI>();

		transform.rotation = Quaternion.Euler(new Vector3(25, -90, 0));

		characterModels.GetChild(0).GetComponent<CharacterModel>().Hover();

		characterSelectUI.ToggleArrows(selectedIndex, characterModels.childCount);

	}

	void Update()
	{
		if (GameController.state != State.CHARACTER_SELECT) { return; }


		if (Input.GetMouseButton(0))
		{
			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			Control();

			Camera.main.transform.position += new Vector3(0, 0, -rotationMovement);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			float threshold = .05f;
			float adjustedMovement = rotationMovement * 10f;

			if (adjustedMovement > threshold)
			{
				selectedIndex--;

			} else if (adjustedMovement < -threshold)
			{
				selectedIndex++;
			}

			selectedIndex = Mathf.Clamp(selectedIndex, 0, characterModels.childCount - 1);

			characterSelectUI.ToggleArrows(selectedIndex, characterModels.childCount);

			Vector3 pos = characterModels.GetChild(selectedIndex).transform.position;

			pos.x = characterModels.GetChild(selectedIndex).transform.position.x + 8;

			if (lastSelected != selectedIndex)
			{
				characterModels.GetChild(selectedIndex).GetComponent<CharacterModel>().Hover();
				Haptic.Vibrate(HapticIntensity.Light);
			}

			rotationMovement = 0;

			StopCoroutine("ISnapPosition");

			StartCoroutine("ISnapPosition", pos);

			lastSelected = selectedIndex;
		}
	}

	void OnStateChange(State s)
	{
		if (s != State.CHARACTER_SELECT)
		{
			StopCoroutine("ISnapPosition");
		}
	}

	IEnumerator ISnapPosition(Vector3 target)
	{
		while (camera.transform.position != target)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, target + heightOffset, Time.deltaTime * 10f);
			yield return null;
		}
	}

	void Control()
	{

		currentPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);

		if (Input.GetMouseButtonDown(0))
		{
			lastPosition = currentPosition;
		}

		float diff = (currentPosition.x - lastPosition.x);

		rotationMovement = diff;



		//Debug.Log(selectedIndex);

		lastPosition = currentPosition;
	}

	public int SelectedIndex
	{
		get { return selectedIndex; }
	}

	public int MaxChildCount
	{
		get { return characterModels.childCount; }
	}
}