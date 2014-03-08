﻿using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawn : MonoBehaviour {

	public GameObject wave1;
	public GameObject wave2;
	public GameObject wave3;
	public GameObject triangle;
	public GameObject cymbal;
	public GameObject trumpet;
	//private ArrayList waveList;
	//private ArrayList itmList;
	private float timer = 0.0f;
	private float timeSpawn = 0.5f;
	private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
	private int size = 1;
	private GameObject waveSpawn;
	private GameObject triangleSpawn;
	private GameObject cymbalSpawn;
	private GameObject trumpetSpawn;
	private GameObject CollectableSpawn;
	private Vector2 wavePosition;
	private float[] itmHeight;
	private float waveHeight;
	private float CurrentVelocitySpawn;
	private float bouncePower;
	private Vector2 initWavePosition;
	private Vector2 initItemPosition;
	private int ctr;
	private ArrayList collectables;
	private int songSize = 60;
	private int collectableGap = 8;
	private int icollect = 0;
	// Use this for initialization
	void Awake()
	{
		collectables = new ArrayList ();
		ctr = 0;
		itmHeight = new float[3];
		itmHeight [0] = 8.0f;
		itmHeight [1] = 12.0f;
		itmHeight [2] = 16.0f;
		waveHeight = -0.9f;
		initWavePosition = transform.position; 
		initItemPosition.x = transform.position.x+2; 
		//waveList = new ArrayList();
		//itmList = new ArrayList();
	}
	void Start () {
		//ClearGame ();
		if (!determineCollectables ()) {
			Debug.Log ("ERREUR DANS DETERMINECOLLECTABLES!!!!");
		}
		Shuffle(collectables);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void FixedUpdate()
	{
		initWavePosition.y = waveHeight;
		CurrentVelocitySpawn = -15f;
		timer += Time.deltaTime;
		//Debug.Log ("FixedUpdate");
		if(timer >= timeSpawn)
		{

			switch(size)
			{
			case 1:
				initItemPosition.y = itmHeight [0];
				waveSpawn = (GameObject)Instantiate(wave1, initWavePosition , Quaternion.identity);
				bouncePower = 8;
				break;
			case 2:
				initItemPosition.y = itmHeight [1];
				initWavePosition.y+=1.0f;
				waveSpawn = (GameObject)Instantiate(wave2, initWavePosition, Quaternion.identity);
				bouncePower = 13;
				break;
			case 3:
				initItemPosition.y = itmHeight [2];
				initWavePosition.y+=2.1f;
				waveSpawn = (GameObject)Instantiate(wave3, initWavePosition, Quaternion.identity);
				bouncePower = 18;
				break;
			default:
				break;
			}
			size = RollDice(3);

			if (ctr==collectableGap && icollect<collectables.Count)
			{
				switch((int)collectables[icollect++])
				{
				case 1:
					CollectableSpawn = (GameObject)Instantiate(triangle, initItemPosition , Quaternion.identity);
					break;
				case 2 :
					CollectableSpawn = (GameObject)Instantiate(cymbal, initItemPosition , Quaternion.identity);
					break;
				case 3 :
					CollectableSpawn = (GameObject)Instantiate(trumpet, initItemPosition , Quaternion.identity);
					break;
				default:
					break;
				}

				CollectableSpawn.GetComponent<Collectable>().SetVelocity(CurrentVelocitySpawn);
				ctr = 0;
			}
			else
			{
				ctr++;
			}
			waveSpawn.GetComponent<WaveScript>().SetVelocity(CurrentVelocitySpawn);
			waveSpawn.GetComponent<WaveScript>().BouncePower = bouncePower;
			//waveList.Add (waveSpawn);
			//itmList.Add (itemSpawn);
			timer = 0.0f;
		
			//Debug.Log ("TimeSpawn : " + timeSpawn);
			//Debug.Log ("Size : " + size);
		}
	}

	/*void ClearGame()
	{
  		for(int i = waveList.Count-1;i>=0;i--)
		{
			GameObject waveDestroy = (GameObject)waveList[i];
			Destroy(waveDestroy.gameObject);
			//waveList.RemoveAt(i);        
		}
		for(int i = itmList.Count-1;i>=0;i--)
		{
			GameObject itmDestroy = (GameObject)itmList[i];
			Destroy(itmDestroy.gameObject);
			//itmList.RemoveAt(i);        
		}
	}*/
	private bool determineCollectables()
	{
		float nbCollectables = songSize / collectableGap;
		double nbTriangles = Math.Round(0.5 * nbCollectables);
		double nbCymbal = Math.Round(0.33 * nbCollectables);
		double nbTrumpet = Math.Round(0.17 * nbCollectables);
		if (nbTriangles + nbCymbal + nbTrumpet == nbCollectables) {
			for (int i=0; i<nbTriangles; i++)
			{
				collectables.Add (1);
			}
			for (int i=0; i<nbCymbal; i++)
			{
				collectables.Add (2);
			}
			for (int i=0; i<nbTrumpet; i++)
			{
				collectables.Add (3);
			}
			return true;
		}
		else
			return false;
	}
	public static void Shuffle( ArrayList list)
	{
		byte size = (byte)list.Count;
		int n = --size;
		int newpos;
		int temp;

		while (n+1 > 1)
		{
			newpos = RollDice(size);
			temp = (int)list[newpos];
			list[newpos] = list[n];
			list[n] = temp;
			n--;
		}
	}
	public static byte RollDice(byte numberSides)
	{
		if (numberSides <= 0)
			throw new ArgumentOutOfRangeException("numberSides");
		
		// Create a byte array to hold the random value. 
		byte[] randomNumber = new byte[1];

		do
		{
			// Fill the array with a random value.
			rngCsp.GetBytes(randomNumber);
		}

		while (!IsFairRoll(randomNumber[0], numberSides));
		// Return the random number mod the number 
		// of sides.  The possible values are zero- 
		// based, so we add one. 
		return (byte)((randomNumber[0] % numberSides) + 1);
	}
	
	private static bool IsFairRoll(byte roll, byte numSides)
	{
		// There are MaxValue / numSides full sets of numbers that can come up 
		// in a single byte.  For instance, if we have a 6 sided die, there are 
		// 42 full sets of 1-6 that come up.  The 43rd set is incomplete. 
		int fullSetsOfValues = Byte.MaxValue / numSides;
		
		// If the roll is within this range of fair values, then we let it continue. 
		// In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use 
		// < rather than <= since the = portion allows through an extra 0 value). 
		// 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair 
		// to use. 
		return roll < numSides * fullSetsOfValues;
	}
}
