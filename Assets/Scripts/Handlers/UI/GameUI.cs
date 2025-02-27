﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameUI : MonoBehaviour {


	public CanvasGroup waveHUD, waveEndHUD, virtualJoyStickHUD, pausePanel, bloodOverlay;

	public Animation hurtOverlay, fade;

	public DisplayText levelUpDisplayText;

	void OnEnable () {
		EventManager.OnGameEvent += OnGameEvent;
		EventManager.OnButtonClick += OnButtonClick;
		EventManager.OnStateChange += OnStateChange;
	}

	void OnDisable ()
	{
		EventManager.OnGameEvent -= OnGameEvent;
		EventManager.OnButtonClick -= OnButtonClick;
		EventManager.OnStateChange -= OnStateChange;
	}

	void Start()
	{
		Toggle(false);

	}

	void OnGameEvent(EventID id)
	{
		switch (id)
		{


			case EventID.WAVE_END:
			{
				StopCoroutine("IShowWaveEndHUD");
				StartCoroutine("IShowWaveEndHUD");

				break;
			}

			case EventID.PLAYER_HURT:
			{
				hurtOverlay.Play();
				break;
			}

			case EventID.ENEMY_KILLED:
			{
				WaveController.Instance.GoldCollected += 20;
				break;
			}

			case EventID.LEVEL_UP:
			{
				levelUpDisplayText.Show("Level Up!", Color.yellow);
				break;
			}
		}
	}

	private bool b;

	void Update()
	{
		if (GameController.state != State.GAME) { return; }
		bloodOverlay.alpha = 1f - PlayerController.Instance.HealthRatio;
	}

	void OnStateChange(State s)
	{
		switch (s)
		{
			case State.GAME:
			{
				Toggle(true);
				fade.Play("fade_in");
				break;
			}
		}
	}

	void OnButtonClick(ButtonID id, SimpleButtonHandler handler)
	{
	}


	IEnumerator IShowWaveEndHUD()
	{
		CameraController.Instance.ToggleBlur(true);

		virtualJoyStickHUD.alpha = 0f;

		virtualJoyStickHUD.blocksRaycasts = false;

		waveEndHUD.alpha = 1f;

		waveEndHUD.blocksRaycasts = true;

		waveEndHUD.transform.GetComponent<Animation>().Play();

		yield return new WaitForSeconds(5f);

		while (waveEndHUD.alpha > .01f)
		{
			waveEndHUD.alpha -= Time.deltaTime;

			yield return null;
		}

		fade.Play("fade_out");

		waveEndHUD.alpha = 0f;

		waveEndHUD.blocksRaycasts = false;

		yield return new WaitForSeconds(1);

		GameController.Instance.Reload();
	}

	private void Toggle(bool b)
	{

		CanvasGroup cg = GetComponent<CanvasGroup>();

		cg.alpha = b ? 1 : 0;

		cg.blocksRaycasts = b;
	}


	public void PauseButtonClick()
	{
		Time.timeScale = 0f;
		GameController.PAUSE = true;
		pausePanel.alpha = 1f;
		pausePanel.blocksRaycasts = true;
	}

	public void ResumeButtonClick()
	{
		Time.timeScale = 1f;
		GameController.PAUSE = false;
		pausePanel.alpha = 0f;
		pausePanel.blocksRaycasts = false;
	}

	public void FleeButtonClick()
	{
		Time.timeScale = 1f;
		GameController.Instance.Reload();
	}
}
