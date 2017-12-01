﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller for the sprite animations in the shop screen
public class ShopControllers : MonoBehaviour {

	//Everyone
	public int ItemNumber, WaitTime;
	
	int _counter;

	//Explosion
	public float _explosionW, _explosionH;

	//Blink
	public float Distance;

	//Blades
	public float Direction;

	//Gun
	public GameObject Bullet;
	GameObject _bullet;

	// Use this for initialization
	void Start () {
		_explosionW = Random.Range(0.07f, 0.50f);
		_explosionH = Random.Range(0.07f, 0.50f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(ItemNumber == 0)
		{
			Speed();
		}
		else if(ItemNumber == 1)
		{
			Explosion();
		}
		else if (ItemNumber == 2)
		{
			Blink();
		}
		else if (ItemNumber == 3)
		{
			Blades();
		}
		else if (ItemNumber == 4)
		{
			Gun();
		}
		else if (ItemNumber == 5)
		{
			Shield();
		}
	}

	// Makes the shoes run back and forth across the screen. The speed scales with the localPosition.x value
	void Speed ()
	{
		if (transform.localPosition.x <= -3.65f && transform.localScale.x < 0.3f)
		{
			transform.localScale = new Vector3(transform.localScale.x + 0.01f, transform.localScale.y, transform.localScale.z);
		}
		else if (transform.localPosition.x >= 3.6f && transform.localScale.x > -0.3f)
		{
			transform.localScale = new Vector3(transform.localScale.x - 0.01f, transform.localScale.y, transform.localScale.z);
		}
		else if (_counter > 0 && transform.localPosition.x <= -3.65f && transform.localScale.x == 0.3f)
		{
			_counter--;
		}
		else
		{
			float scalar = 1 - ((3.6f - transform.localPosition.x) / (3.6f + 3.65f));
			if (scalar < 0.2)
			{
				scalar = 0.2f;
			}
			if (transform.localScale.x == 0.3f)
			{
				transform.localPosition = new Vector3(transform.localPosition.x + scalar * 0.2f, transform.localPosition.y, 0.01f);
			}
			else
			{
				transform.localPosition = new Vector3(transform.localPosition.x - scalar * 0.2f, transform.localPosition.y, -0.01f);
			}
			_counter = WaitTime;
		}
	}

	// The explosion sprite animation. Just rotates it and stretches it to alter the shape whenever the counter hits 0
	void Explosion ()
	{
		if (_counter == 0)
		{
			_explosionW = Random.Range(-0.50f, 0.50f);
			_explosionH = Random.Range(-0.50f, 0.50f);
			transform.localScale = new Vector3(_explosionW, _explosionH, transform.localScale.z);
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
			_counter = WaitTime;
		}
		if(_counter > 0)
		{
			_counter--;
		}
	}

	// The blink sprite animation. Spawns in a circle around its purchase button whenever the counter hits 0
	void Blink()
	{
		if (_counter == 0)
		{
			float random = Random.Range(0.0f, 360.0f);
			transform.localPosition = new Vector3(0.38f + Distance * Mathf.Cos(Mathf.PI * random / 180.0f), 2.62f + Distance * Mathf.Sin(Mathf.PI * random / 180.0f), 0.0f);
			_counter = WaitTime;
		}
		if (_counter > 0)
		{
			_counter--;
		}
	}

	// The blades sprite animation. Just rotates
	void Blades()
	{
		transform.localEulerAngles = new Vector3(0.0f, 0.0f, transform.localEulerAngles.z + Direction);
	}

	// The gun sprite animation. Fires a bullet whenever the counter hits 0 and wobbles a little
	void Gun()
	{
		if (_bullet == null && _counter == 0)
		{
			_bullet = GameObject.Instantiate(Bullet);
			_bullet.transform.parent = transform;
			_bullet.transform.position = new Vector3(Bullet.transform.position.x, Bullet.transform.position.y, Bullet.transform.position.z);
			_bullet.SetActive(true);
			_counter = WaitTime;
		}
		else if (_bullet != null)
		{
			_bullet.transform.localPosition = new Vector3(_bullet.transform.localPosition.x + 0.5f, _bullet.transform.localPosition.y, _bullet.transform.localPosition.z);
		}
		if(_counter >= 50)
		{
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 60.0f - _counter);
		}
		else if (_counter >= 40)
		{
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, _counter - 40);
		}
		else if (_counter >= 35)
		{
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, _counter - 40);
		}
		else if (_counter >= 30)
		{
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 30 - _counter);
		}
		if (_counter > 0)
		{
			_counter--;
		}
	}

	// The shield sprite animatioon. blinks for a few seconds whenver a bullet hits it
	void Shield()
	{
		if (_counter % 20 < 10 && _counter != 0)
		{
			GetComponent<SpriteRenderer>().enabled = false;
		}
		else
		{
			GetComponent<SpriteRenderer>().enabled = true;
		}
		if (_counter > 0)
		{
			_counter--;
		}
	}

	// When a bullet hits the shield sprite destroy it and start the shields animation
	void OnTriggerEnter(Collider other)
	{
		if(ItemNumber == 5)
		{
			Destroy(other.gameObject);
			_counter = WaitTime;
		}
	}
}
