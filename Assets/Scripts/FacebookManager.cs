using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
/**
 * Pour avoir le hash de l'application :
 * Fais tourner l'appli et fais une demande de login sur le telephone directement  
 * Sur le telephone il va te mettre une erreur et t'afficher le bon hash
 * Pour avoir le has (tu peux pas le recopier a la main)
 * avant de faire la demande de login sur le telephone lance une console
 * adb logcat
 * va y avoir une tonne de truc mais des que tu fais la demande de login fais Ctrl + c pour arreter la commande
 * et cherche le message d'erreur de facebook il devrait avoir le hash dedans
 * 
 * Sinon copie direct celui de la page facebook dev que j'ai deja mit
 * 
 * RniWiubeGAE77pp4ZIavm5t4k/g=
 */
public class FacebookManager{

	public static void Init(){
		FB.Init (OnInitCompleted, OnHideUnity);
	}

	public static void Init(Runnable onInit){
		FB.Init (() => OnInitCompletedWithListener(onInit), OnHideUnity);
	}

	private static void OnInitCompletedWithListener(Runnable r){
		OnInitCompleted ();
		r ();
	}

	private static void OnInitCompleted(){
		Debug.Log ("init complete");
		if (FB.IsLoggedIn) {
			Debug.Log ("logged in");
		} else {
			Debug.Log ("not logged in");
		}
	}

	private static void OnHideUnity(bool isGameShow){
		if (!isGameShow) {
			//pause game ?
		} else {
			//un pause game ?
		}
	}

	public static void Loggin(StringDelegate resultt){
		List<string> permissions = new List<string> ();
		permissions.Add ("public_profile");
		permissions.Add ("publish_actions");
		FB.LogInWithReadPermissions (permissions, (ILoginResult result) => OnAuthFinished (result, resultt));
	}

	private static void OnAuthFinished(ILoginResult result, StringDelegate tmpStringDelegate){
		if (result.Error != null) {
			//there is an error
			Debug.Log(result.Error);
			if (tmpStringDelegate != null)
				tmpStringDelegate ("Error on authentification");
		} else {
			//no errir
			if (FB.IsLoggedIn) {
				if (tmpStringDelegate != null)
					tmpStringDelegate ("Sucess");
				Debug.Log ("logged in auth finished");
			} else {
				if (tmpStringDelegate != null)
					tmpStringDelegate ("You canceled e.e");
				Debug.Log ("not logged in auth finished");
			}
		}
	}

	public static void RequestUserFullName(StringDelegate ev){
		FB.API ("/me?fields=first_name,last_name", HttpMethod.GET, ((IGraphResult result) => ReceiveUserName(result, ev)) );
	}
	
	private static void ReceiveUserName(IResult result, StringDelegate tmpStringDelegate){
		if (result.Error == null) {
			if (tmpStringDelegate != null)
				tmpStringDelegate (result.ResultDictionary ["first_name"].ToString () + " " + result.ResultDictionary ["last_name"].ToString ());
		} else {
			Debug.Log (result.Error);
			if (tmpStringDelegate != null)
				tmpStringDelegate (result.Error);
		}
	}

	public static void RequestFriendFullName(string userId, StringDelegate ev){
		FB.API ("/" + userId + "?fields=first_name,last_name", HttpMethod.GET, ((IGraphResult result) => ReceiveUserName (result, ev)));
	}

	public static void RequestUserProfilPic(int height, SpriteDelegate sd){
		FB.API ("/me/picture?type=square&height=" + height + "&width=" + height, HttpMethod.GET, (IGraphResult result) => ReceiveProfilPic(result, sd, height));
	}

	private static void ReceiveProfilPic(IGraphResult result, SpriteDelegate tmpSpriteDelegate, int tmpInt){
		if (result.Error == null && result.Texture != null) {
			Sprite r = Sprite.Create (result.Texture, new Rect (0, 0, tmpInt, tmpInt), Vector2.zero);
			if(tmpSpriteDelegate != null)
				tmpSpriteDelegate (r);
		} else {
			Debug.Log (result.Error);
			if (tmpSpriteDelegate != null)
				tmpSpriteDelegate (null);
		}
	}

	public static void RequestFriendProfilPic(int height, SpriteDelegate sd, string userId){
		FB.API ("/" + userId + "/picture?type=square&height=" + height + "&width=" + height, HttpMethod.GET, (IGraphResult result) => ReceiveProfilPic (result, sd, height));
	}

	public static void RequestFriendsScores(ScoreArrayDelegate s){
		FB.API ("app/scores?fields=score,user.limit(30)", HttpMethod.GET, ((IGraphResult result) => ReceiveFriendsScores (result, s)));
	}

	private static void ReceiveFriendsScores(IResult result, ScoreArrayDelegate tmpScoreArrayDelegate){
		if (result.Error == null) {
			ArrayList list = new ArrayList ();
			Dictionary<string,object> resultDict = (Dictionary<string,object>)Facebook.MiniJSON.Json.Deserialize (result.RawResult);

			foreach (object entryObj in (List<object>) resultDict["data"]) {
				Dictionary<string, object> entry = (Dictionary<string, object>) entryObj;
				Dictionary<string, object> user = (Dictionary<string, object>)entry ["user"];

				ScoreData data = new ScoreData ();
				data.score = int.Parse(entry ["score"].ToString());
				data.user_id = user ["id"].ToString();
				data.user_name = user ["name"].ToString();
				list.Add (data);
			}
			if (tmpScoreArrayDelegate != null) {
				tmpScoreArrayDelegate (list.ToArray (typeof(ScoreData)) as ScoreData[]);
			}
		} else {
			Debug.Log (result.Error);
			if (tmpScoreArrayDelegate != null)
				tmpScoreArrayDelegate (null);
		}
	}

	public static void PostScore(int score, FacebookDelegate<IGraphResult> onResultReceived){
		Dictionary<string, string> scoreDict = new Dictionary<string, string> ();
		scoreDict ["score"] = score.ToString ();
		FB.API ("me/scores", HttpMethod.POST, onResultReceived, scoreDict);
	}
}

public delegate void StringDelegate(string str);
public delegate void SpriteDelegate(Sprite s);
public delegate void ScoreArrayDelegate(ScoreData[] scores);
public delegate void Runnable();

public class ScoreData{
	public int score;
	public string user_name;
	public string user_id;
}