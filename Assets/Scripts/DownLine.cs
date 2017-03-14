using UnityEngine;
using System.Collections;

public class DownLine : MonoBehaviour {
	public float size;
	public bool isSecondary = false;
	
	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag ("Upline")) {
			UpLine obs = other.GetComponent<UpLine> ();
			if (!obs.isDone) {
				obs.isDone = true;
				if (size <= obs.size) {
					CountPoints (obs);
				} else {
					if (isSecondary) {
						Destroy (Split (obs));
						Camera.main.GetComponent<GameStartup> ().AddMalus ();
						StartCoroutine(Camera.main.GetComponent<GameStartup>().SpawnExploLine(obs.transform.position.y, obs.size, Color.red)); 
					} else {
						Split (obs);
					}
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.CompareTag ("Boundary")) {
			Destroy (gameObject, 1f);
		}
	}

	void CountPoints(UpLine other){
		//count point and add malus if necessary
		other.SetColor(Color.gray);
		GameStartup startup = Camera.main.GetComponent<GameStartup>();
		int addScore = (int)((size / other.size) * 100);
		startup.score += addScore;
		if (addScore < 60) {
			startup.AddMalus ();
			StartCoroutine(startup.SpawnExploLine(other.transform.position.y, other.size, Color.red)); 
		}
		else if (isSecondary && addScore > 60) {
			startup.score += (int)((size / other.size) * 100);
			//TODO spawn explo jaune sur le trait
			StartCoroutine(startup.SpawnExploLine(other.transform.position.y, other.size, Color.yellow)); 
		}
			
		GetComponent<Mover> ().goUp = false;
		transform.position = other.transform.position - new Vector3 (0f, P.pocP (other.size - size, Side.H), 0f);
	}

	GameObject Split(UpLine other){
		//add 100 points and split the line (destriy the current one, create a smaller one a the right place and change color of the upline)
		GameStartup startup = Camera.main.GetComponent<GameStartup>();
		startup.score += 100;
		GetComponent<Mover> ().goUp = false;
		GameObject miniLine = GameStartup.SpawnDownline (size - other.size);
		miniLine.GetComponent<DownLine> ().isSecondary = true;
		miniLine.transform.position = other.transform.position + new Vector3 (0f, P.pocP (other.size + (size - other.size), Side.H), 0f);
		//miniLine.GetComponent<SpriteRenderer> ().color = Color.red;

		transform.localScale = other.transform.localScale;
		transform.position = other.transform.position;
		return miniLine;
	}
}
