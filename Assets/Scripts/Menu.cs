using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public Text bestScore;
	public Text lastScore;

	void Start(){
		Data data = Data.GetInstance ();
		bestScore.text = "Best Score - " + data.score;
		lastScore.text = "Last Score - " + data.lastScore;
	}

	public void PlayGame(){
		UnityEngine.SceneManagement.SceneManager.LoadScene ("GameScene");
	}

	public void Leaderboard(){
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Leaderboard");
	}
}
