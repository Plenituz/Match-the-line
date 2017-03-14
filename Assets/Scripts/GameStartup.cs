using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStartup : MonoBehaviour {
	private float playerSize;
	private int malusSize = 0;
	private bool hasClicked = false;
	private GameObject player;
	private GameObject malus;
	private Text scoreText;

	public int score = 0;

	void Start () {
		P.init ();
		playerSize = GameValues.DEFAULT_SIZE;
		GameValues.screenWidthSPoc = P.pocSGame (1f, Side.W);
		GameValues.gameSpeed = 0.01f;
		Time.timeScale = 1f;

		player = new GameObject ("Player");
		player.AddComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("pixel");
		player.tag = "Player";

		malus = new GameObject ("Malus");
		SpriteRenderer srM = malus.AddComponent<SpriteRenderer> ();
		srM.sprite = Resources.Load<Sprite> ("pixel");
		srM.color = Color.black;
		srM.sortingOrder = 10;

		scoreText = GameObject.FindGameObjectWithTag ("ScoreText").GetComponent<Text> ();

		GameObject boundary = new GameObject ("Boundary");
		boundary.tag = "Boundary";
		BoxCollider2D boundaryColl = boundary.AddComponent<BoxCollider2D> ();
		boundaryColl.isTrigger = true;
		boundaryColl.size = new Vector2 (P.pocP (2f, Side.W), P.pocP (2f, Side.H));

		StartCoroutine ("Generate");
	}

	void Update(){
		if (int.Parse (scoreText.text) != score) {
			scoreText.text = score.ToString ();
		}

		malus.transform.localScale = new Vector3 (GameValues.screenWidthSPoc, P.pocSGame (malusSize * 0.1f, Side.H), 1f);
		malus.transform.position = new Vector3 (0f, P.pocP (1f - (malusSize * 0.1f), Side.H), 0f);

		player.transform.localScale = new Vector3 (GameValues.screenWidthSPoc, P.pocSGame (playerSize, Side.H), 1f);
		player.transform.position = new Vector3 (0f, P.pocP (-1f + (playerSize), Side.H), 0f);
	}

	void FixedUpdate(){
		#if !UNITY_EDITOR
		if (Input.touchCount > 0) {
			hasClicked = true;
			playerSize += GameValues.GROW_RATE*Input.touchCount;
		} else if (hasClicked) {
			SpawnDownline(playerSize);
			playerSize = GameValues.DEFAULT_SIZE;
			hasClicked = false;
		}
		#endif

		#if UNITY_EDITOR
		if(Input.GetMouseButton(0)){
			hasClicked = true;
			playerSize += GameValues.GROW_RATE;
		} else if(hasClicked) {
			SpawnDownline(playerSize);
			playerSize = GameValues.DEFAULT_SIZE;
			hasClicked = false;
		}
		#endif
	}

	public static GameObject SpawnDownline(float size){
		GameObject line = new GameObject ("Line");
		line.transform.localScale = new Vector3 (GameValues.screenWidthSPoc, P.pocSGame (size, Side.H), 1f);
		line.transform.position = new Vector3 (0f, P.pocP (-1f + size, Side.H), 0f);

		line.AddComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("pixel");
		EdgeCollider2D collBot = line.AddComponent<EdgeCollider2D> ();
		collBot.offset = new Vector2 (0f, -0.005f);
		collBot.isTrigger = true;
		line.AddComponent<Mover> ();
		Rigidbody2D rg = line.AddComponent<Rigidbody2D> ();
		rg.gravityScale = 0f;
		line.AddComponent<DownLine> ().size = size;
		return line;
	}

	public static GameObject SpawnUpline(float size){
		GameObject upline = new GameObject ("UpLine");
		upline.tag = "Upline";
		upline.transform.localScale = new Vector3 (GameValues.screenWidthSPoc, P.pocSGame (size, Side.H), 1f);
		upline.transform.position = new Vector3 (0f, P.pocP (1f + size, Side.H), 0f);

		SpriteRenderer sr = upline.AddComponent<SpriteRenderer> ();
		sr.sprite = Resources.Load<Sprite> ("pixel");
		sr.color = new Color (0.03529411764705882352941176470588f, 0.3960784313725490196078431372549f, 0.53725490196078431372549019607843f, 1f);
		sr.sortingOrder = -1;

		upline.AddComponent<BoxCollider2D> ().isTrigger = true;
		upline.AddComponent<Mover> ().goUp = false;
		upline.AddComponent<UpLine> ().size = size;
		upline.AddComponent<Rigidbody2D> ().gravityScale = 0f;
		return upline;
	}

	public IEnumerator SpawnExploLine(float pos, float size, Color color){
		color.a = 0.5f;
		GameObject explo = new GameObject ("explo" + color.ToString ());
		SpriteRenderer sr = explo.AddComponent<SpriteRenderer> ();
		sr.sprite = Resources.Load<Sprite> ("pixel");
		sr.color = color;
		sr.sortingOrder = 5;
		explo.transform.position = new Vector3 (0f, pos, 0f);
		explo.transform.localScale = new Vector3 (GameValues.screenWidthSPoc, P.pocSGame (size, Side.H), 1f);
		StartCoroutine (P.AnimateScaleYPoc (explo.transform, size, size + 0.1f, 0.35f, P.DecelerateInterpolator));
		yield return new WaitForSeconds (0.15f);
	//	StartCoroutine (P.AnimateScaleYPoc (explo.transform, size + 0.1f, 0, 0.2f, P.AccelerateDeccelerateInterpolator));
	//	yield return new WaitForSeconds (0.1f);
		StartCoroutine (P.FadeAlpha (sr, 0.2f, 0.5f));

		Destroy(explo, 0.7f);
	}

	public void AddMalus(){
		malusSize++;
		if (malusSize >= 9) {
			Data data = Data.GetInstance ();
			if (score > data.score) 
				data.score = score;
			data.lastScore = score;	
			Data.SetInstance (data);
			
			UnityEngine.SceneManagement.SceneManager.LoadScene ("MainMenu");
		}
	}

	IEnumerator Generate(){

		while (true) {
			UpLine lastLine = SpawnUpline (Random.Range (GameValues.MIN_UPLINE_SIZE, GameValues.MAX_UPLINE_SIZE)).GetComponent<UpLine> ();

			yield return new WaitForSeconds (P.pocP(Random.Range (GameValues.MIN_GAP, GameValues.MAX_GAP) + lastLine.size/100, Side.H) / P.pocP(GameValues.gameSpeed, Side.H));
		}
	}
}