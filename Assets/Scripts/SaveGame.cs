using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveGame : MonoBehaviour {

    private static string fileName = "/IVRHighscore.txt";
    private static string path = Application.persistentDataPath;

    public static void SaveScore(int score)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path + fileName);
        bf.Serialize(file, score);
        file.Close();
    }

    public static int LoadScore()
    {
        int score;

        if (File.Exists(path + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path + fileName, FileMode.Open);
            score = (int)bf.Deserialize(file);

            return score;
        }

        return 0;
    }
}
