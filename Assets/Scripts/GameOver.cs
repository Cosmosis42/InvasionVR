using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOver : MonoBehaviour {

    public Text ScoreText;

    // Use this for initialization
    void Start() {
        MagnetSensor.OnCardboardTrigger += ChangeScene;
        ScoreText.text = "Your Score: " + GameController.finalScore + "\nHigh Score: " + GameController.highscore
            + "\nTap to Play Again";
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0))
            ChangeScene();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameController.QuitGame();
        }

    }

    public static void ChangeScene()
    {
        MagnetSensor.OnCardboardTrigger -= ChangeScene;
        SceneManager.LoadScene("Invasion");
    }
}
