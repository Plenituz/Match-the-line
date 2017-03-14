using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {
	private float speed;
	public bool goUp = true;

	void Update () {
		speed = P.pocP (GameValues.gameSpeed, Side.H);
		transform.position += new Vector3 (0f, goUp ? speed : -speed, 0f);
	}
}
