using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Data{
	private static Data instance;

	public static Data GetInstance(){
		if (instance == null)
			LoadData ();
		return instance;
	}

	public static void SetInstance(Data data){
		instance = data;
		SaveData ();
	}

	public const string DATA_PATH = "/data.pltz";

	public int score = 0;
	public int lastScore = 0;

	public static void LoadData(){
		if (instance != null)
			return;
		if (File.Exists (Application.persistentDataPath + DATA_PATH)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + DATA_PATH, FileMode.Open);
			instance = (Data)bf.Deserialize (file);
			file.Close ();
		} else {
			instance = new Data ();
			SaveData ();
		}
	}

	public static void SaveData(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + DATA_PATH);
		bf.Serialize (file, instance);
		file.Close ();
	}
}
