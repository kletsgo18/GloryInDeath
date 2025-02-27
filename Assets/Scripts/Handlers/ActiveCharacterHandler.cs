﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCharacterHandler : MonoBehaviour
{
	public CharacterStats stats;
	public HealthHandler healthHandler;

	private PlayerController _player;

	void Start()
	{
		_player = PlayerController.Instance;
	}
	void Update()
	{
		if (_player.IsDead)
		{
			healthHandler.Toggle(false);
		}
		healthHandler.health = PlayerController.Instance.HealthRatio;
		healthHandler.healthString = PlayerController.Instance.HealthText;
		healthHandler.stamina = PlayerController.Instance.StaminaRatio;
		healthHandler.levelProgress = PlayerController.Instance.LevelProgress;
		healthHandler.levelTextString = PlayerController.Instance.LevelString;
	}
}
