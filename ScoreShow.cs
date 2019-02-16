using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScoreShow : MonoBehaviour {

	
	void Start () {
		


	}
	
	
	void Update () {
		


	}

    //save score to text file...

    public void savetoText(string name, int score)
    {

        string fileName = Application.persistentDataPath + "/playerScore.txt";
        if (!File.Exists(fileName))
        {
            Debug.Log(fileName + " doesn't exists.");
            File.CreateText(fileName);
        }
        using (System.IO.StreamWriter file =
        new System.IO.StreamWriter(@fileName, true))
        {
            //file.WriteLine("\n Name : " + name + "-------Right score:" + score + "\n");

        }

    }


}
