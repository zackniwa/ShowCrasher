﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private bool jump = false;
	private bool isAttacking = false;
	public float jumpForce = 100000000.0f;
	private bool isInTheAir = false;
	
	private bool isFalling = false;
	private bool grounded = false;
	protected Animator playerAnimation;
	public float timer = 0.2f;

	void Awake()
	{
	}

	// Use this for initialization
	void Start () 
	{
		playerAnimation = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () 
	{
		timer -= Time.deltaTime;
		if (Input.GetButtonDown ("Jump") && grounded) 
		{
			jump = true;
			grounded = false;
		} 

		if (jump && !grounded) 
		{
			isInTheAir = true;
		}

		if (Input.GetButtonDown ("Enter") && timer <= 0.0f && grounded) 
		{
			isAttacking = true;
			playerAnimation.SetBool ("Attack", true);
			jump = false;
			timer = 0.2f;
		}

		if (grounded) 
		{
			playerAnimation.SetBool("Jump", false);
			playerAnimation.SetBool("Grounded", true);
			jump = false;
			isInTheAir = false;
		}

		if(rigidbody2D.velocity.y < 0)
		{
			isFalling = true;
		}
		else
		{
			isFalling = false;
		}

		if (Input.GetButtonDown ("Enter") && timer <= 0.0f && isInTheAir) 
		{
			timer = 0.2f;
			isAttacking = true;
			playerAnimation.SetBool ("Attack", true);
			playerAnimation.SetBool("Grounded", false);
			rigidbody2D.velocity = new Vector2 (0, -jumpForce);
		}
	}
	
	void FixedUpdate()
	{
		if (jump)//Si on peut sauter. 
		{
			rigidbody2D.velocity = new Vector2(0, jumpForce);
			jump = false;
			playerAnimation.SetBool("Jump", true);
			playerAnimation.SetBool("Grounded", false);
		}

		if (isAttacking)
		{
			isAttacking = false;
			playerAnimation.SetBool ("Attack", false);
		} 
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Terrain") 
		{
			grounded = true;
		}

	}

	public void Bounce(float _vel)
	{
		rigidbody2D.velocity = new Vector2(0, jumpForce + _vel);
	}

	public void HitWall()
	{
		Debug.Log ("Cote Collider");
	}
}
