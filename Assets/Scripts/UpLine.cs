using UnityEngine;
using System.Collections;

public class UpLine : MonoBehaviour {
	public float size;
	public bool isDone = false;
	private SpriteRenderer sr;

	public void SetColor(Color color){
		if (sr == null) {
			sr = GetComponent<SpriteRenderer> ();
		}
		sr.color = color;
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.CompareTag ("Boundary")) {
			Destroy (gameObject);
			if(!isDone)
				Camera.main.GetComponent<GameStartup> ().AddMalus ();
		}
	}
}
