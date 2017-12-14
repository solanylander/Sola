﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles button interactions within the shop
public class UpgradeController : MonoBehaviour {
	
	public GameObject[] Numbers;
	public GameObject[] Positions;
	public Sprite[] ChestSprite, NumberSprite;
	public GameObject Chest, Controller, Store, Single, Online, Back;
	public bool InGameStore;

	GameObject[] _values;
	Upgrade[] _buttons;
	float _wealth, _minus, _oldWealth, _oldMinus;
	bool _hovered, _done;
	string _wealthString, _minusString;
	int _justHovered, _newValue, _individual;

	// Use this for initialization
	void Start ()
	{
		// Get all the buttons in the shop
		_buttons = GetComponentsInChildren<Upgrade>();
		_values = new GameObject[Positions.Length];
		_oldWealth = 0;
		_wealth = 0;
		_justHovered = 0;
		_minus = 0.0f;

		// Initially set players wealth and item price to 0. In case the actual values weren't pulled correctly
		for (int i = 0; i < Positions.Length; i++)
		{
			_values[i] = GameObject.Instantiate(Numbers[0]);
			_values[i].transform.position = Positions[i].transform.position;
			_values[i].transform.parent = transform;
			if (InGameStore)
			{
				_values[i].transform.localScale = new Vector3(0.2f, 0.2f, _values[i].transform.localScale.z);
			}
		}
	}

	void FixedUpdate ()
	{
		CalculateDifferences();
		Dictionary();
		if(!InGameStore)
		{
			foreach (Upgrade up in _buttons)
			{
				Clicked(up);
			}
		}
	}

	void Dictionary()
	{
		if (Controller.GetComponent<Controller>().Requested.ContainsKey("coins"))
		{
			_wealth = Controller.GetComponent<Controller>().Requested["coins"];
		}
		foreach (Upgrade up in _buttons)
		{
			if (Controller.GetComponent<Controller>().Requested.ContainsKey(up.Key))
			{
				float value = Controller.GetComponent<Controller>().Requested[up.Key];
				if (value != 0)
				{
					int position = up.Previous.Length;
					if (position < value)
					{
						up.Bought = true;
					}
				}
			}
		}

	}

	void Clicked(Upgrade up)
	{
		if (up.Pressed == true && _wealth >= _minus)
		{
			Controller.GetComponent<Controller>().Requested.Remove("coins");
			Controller.GetComponent<Controller>().AddResource("coins", -_minus);
			Controller.GetComponent<Controller>().GetResource("coins");

			int amount = 1;
			if (up.Previous.Length != 0)
			{
				foreach(GameObject u in up.Previous)
				{
					if(!u.GetComponent<Upgrade>().Bought)
					{
						amount++;
						u.GetComponent<Upgrade>().Bought = true;
					}
				}
			}
			if (_minus > 0)
			{
				Controller.GetComponent<Controller>().AddResource(up.Key, amount);
				Controller.GetComponent<Controller>().GetResource(up.Key);
				up.Bought = true;
			}
		}
		if (up.Pressed == true)
		{
			up.Pressed = false;
		}
	}

	void CalculateDifferences()
	{
		if(!InGameStore)
		{
			if (_hovered)
			{
				Chest.GetComponent<SpriteRenderer>().sprite = ChestSprite[1];
			}
			else
			{
				Chest.GetComponent<SpriteRenderer>().sprite = ChestSprite[0];
			}

		}
		_wealthString = (_wealth - _minus).ToString();
		while (_wealthString.Length < 7)
		{
			_wealthString = "0" + _wealthString;
		}
		int index = _wealthString.IndexOf('-');
		if (index != -1)
		{
			_wealthString = "-" + _wealthString.Substring(0, index) + _wealthString.Substring(index + 1, _wealthString.Length - (index + 1));
		}

		_hovered = false;
		foreach (Upgrade b in _buttons)
		{
			if (b.Hover && !b.Bought)
			{
				_hovered = true;
				_minus = b.Value;
				if (_justHovered == 0)
				{
					_justHovered = 1;
				}
			}
		}
		if (!_hovered)
		{
			_minus = 0.0f;
			if (_justHovered != 0)
			{
				_justHovered = 3;
			}
		}

		int send = 0;
		if ((_wealth - _minus) != _oldWealth || _minus != _oldMinus)
		{
			send = 1;
		}

		_minusString = _minus.ToString();
		while (_minusString.Length < 7)
		{
			_minusString = "0" + _minusString;
		}
			// 1 means just covered, 2 means covering, 3 means just stopped covering & 0 means not covering
		if (_justHovered == 1 && Mathf.Abs(_values[7].transform.localEulerAngles.x) < 0.04)
		{
			_justHovered = 2;
		}
		if (_justHovered == 3)
		{
			_justHovered = 0;
		}
		// 7 Digits for your wealth and 7 for the price
		FlipNumbers(0, 7, _wealthString, send);
		FlipNumbers(7, Positions.Length, _minusString, send);

	}

	void FlipNumbers(int start, int end, string comparison, int pass)
	{
		
		//It will cut off at 7 digits. Make it so that it either has a coin cap or makes it clear
		for (int i = start; i < end; i++)
		{
			try
			{
				_individual = Int32.Parse(comparison.Substring(i - start, 1));
			}
			catch (FormatException e)
			{
				_individual = 10;
			}

			if (pass == 1 || Mathf.Abs(_values[i].transform.localEulerAngles.x) > 0.4f)
			{
				_values[i].transform.localEulerAngles = new Vector3(_values[i].transform.localEulerAngles.x + 10.0f, 0.0f, 0.0f);
			}
			if (_values[i].transform.localEulerAngles.x >= 90.0f && _values[i].transform.localEulerAngles.x <= 120.0f)
			{
				if (start == 0)
				{
					_oldWealth = Int32.Parse(comparison);
				}
				if (start == 7)
				{
					_oldMinus = Int32.Parse(comparison);
				}
				Destroy(_values[i]);
				_values[i] = GameObject.Instantiate(Numbers[_individual]);
				_values[i].transform.position = Positions[i].transform.position;
				_values[i].transform.localEulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
				_values[i].transform.parent = transform;
				if (InGameStore)
				{
					_values[i].transform.localScale = new Vector3(0.2f, 0.2f, _values[i].transform.localScale.z);
				}
				else
				{
					_values[i].transform.localScale = new Vector3(0.5f, 0.5f, _values[i].transform.localScale.z);
				}

				if (_minus != 0)
				{
					for (int j = 0; j < 8; j++)
					{
						if (!InGameStore)
						{
							SpriteRenderer[] sprites = _values[i].transform.GetComponentsInChildren<SpriteRenderer>();
							foreach (SpriteRenderer s in sprites)
							{
								if (Int32.Parse(_wealthString) > 0)
								{
									s.sprite = NumberSprite[1];
								}
								else
								{
									s.sprite = NumberSprite[2];
								}
							}
						}
					}
				}
			}
		}
	}
	
}
