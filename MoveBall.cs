
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour {
	//variables for the ball
	Vector3 ballStartPosition;
	private Rigidbody2D rb;
	private float speed = 400;
	public AudioSource blip;
	public AudioSource blop;

	private void Start()
	{
		//setting up the rigidbody
		rb = this.GetComponent<Rigidbody2D>();
		ballStartPosition = this.transform.position;
		//put the ball at the start and push it
		ResetBall();
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		//playing sound effects 
		if (col.gameObject.tag == "backwall")
		{
			blop.Play();
		}
		else blip.Play();
	}

	void ResetBall()
	{
		//sets ball at start
		this.transform.position = ballStartPosition;
		//velocity equal zero, to provent from moving
		rb.velocity = Vector3.zero;
		//direction in which we want to push the ball randomly selected
		Vector3 dir = new Vector3(Random.Range(100, 300), Random.Range(-100, 100), 0).normalized;
		//adds force to the ball
		rb.AddForce(dir * speed);
	}

	void Update()
	{
		//updates in case the ball doesn't behave the way we want it to behave
		if (Input.GetKeyDown("space"))
		{
			ResetBall();
		}
	}
}
