using UnityEngine;
using Facebook.Unity;
using System.Collections;
using UnityEngine.UI;

public class LeaderboardHandler : MonoBehaviour {
	public Text loginText;
	public GameObject loginButton;
	public GameObject loadingCanvas;
	public GameObject listCanvas;
	public GameObject scrollViewContent;

	void Start(){
		FacebookManager.Init (Begin);
	}

	void Begin(){
		if(FB.IsLoggedIn)
			ClickLogin ();
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("MainMenu");
		}
	}

	public void ClickLogin(){
		if (!FB.IsInitialized) {
			loginText.text = "Error, try restarting :)";
		} else {
			if (!FB.IsLoggedIn) {
				FacebookManager.Loggin (
					(string result) => {
						if (result.Contains ("Sucess")) {
							ShowWaitingScreenAndLoadScores ();
						} else {
							loginText.text = result;
						}
					}
				);
			} else {
				ShowWaitingScreenAndLoadScores ();
			}
		}
	}

	void ShowWaitingScreenAndLoadScores(){
		loginButton.SetActive (false);
		loadingCanvas.SetActive (true);
		FacebookManager.RequestFriendsScores (DisplayScores);
		FacebookManager.PostScore (Data.GetInstance ().score, 
			(IGraphResult result) => {
				if(result.Error != null){
					//TODO signal to user
				}
			});
	}

	void DisplayScores(ScoreData[] scores){
		loadingCanvas.SetActive (false);
		listCanvas.SetActive (true);
		RectTransform scrollRectT = scrollViewContent.GetComponent<RectTransform> ();
		scrollRectT.sizeDelta = new Vector2 (0f, 155f * scores.Length);
		scrollRectT.anchoredPosition = new Vector3 (0f, 0f, 0f);

		GameObject prefabUi = Resources.Load<GameObject> ("Prefab UI/Profil Pic");
		for (int i = 0; i < scores.Length; i++) {
			GameObject playerUI = Instantiate (prefabUi) as GameObject;
			Image pic = playerUI.GetComponent<Image> ();
			Text name = playerUI.transform.FindChild ("Name").GetComponent<Text> ();
			Text scoreText = playerUI.transform.FindChild ("Score").GetComponent<Text> ();
			scoreText.text = scores [i].score.ToString ();
			name.text = scores [i].user_name;

			FacebookManager.RequestFriendProfilPic (200, 
				(Sprite sprite) => {
					pic.sprite = sprite;
				}, scores [i].user_id);
			
			RectTransform rect = pic.GetComponent<RectTransform> ();
			rect.SetParent (scrollViewContent.transform);
			rect.anchoredPosition = new Vector3 (-300f, (((155f * scores.Length) / 2) - pic.rectTransform.sizeDelta.y) + (i * -150f), 0f);
		}
	}

}
