using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject enemyPrefab;

    [Header("Front Enemy Spawn Area")]
    public int xMaxF = 25;
    public int xMinF = -25, yMaxF = 10, yMinF = 0, zMaxF = 30, zMinF = 0;
    [Space(20)]

    [Header("Enemy and Wave Info")]
    public int enemyCount;
    public float timeBetweenSpawns, timeBetweenWaves, prepareWait;
    public int waveNum = 0;
    int enemiesLeft = 0;
    public Text countdownText;
    bool waveInProgress = false;

    [Header("Score Info")]
    public Text scoreText;
    public int waveScore = 100;
    public int enemyScore = 10;
    private int score;
    public static int highscore = 0;
    public static int finalScore;

    [Header("Laser Info")]
    public GameObject laserPrefab;
    public float speed = 1.0f;
    public float delay = 1;
    public Text controlText; 

    public int controls = 1;
    private bool isLaserFiring = false;

    [Header("Shields Info")]
    public Text shieldText;
    public int maxShields = 3;
    int shieldsRemaining;

    private bool paused = false;

    // Use this for initialization
    void Start() {
        shieldsRemaining = maxShields;
        controlText.text = "Ctrls: Tap";
        highscore = SaveGame.LoadScore();
        score = 0;
        UpdateScore();
        UpdateShields();
        StartCoroutine(SpawnWaves());
    }

    // Update is called once per frame
    void Update() {

        if (controls == 3)
        {
            StartCoroutine(ShootLaserOnTimer(delay));
        }

        if (controls == 1 && Input.GetMouseButtonDown(0))
        {
            ShootLaser();
        }

        if (shieldsRemaining <= 0)
        {
            finalScore = score;
            if (finalScore >= highscore)
                highscore = finalScore;

            SaveGame.SaveScore(highscore);
            MagnetSensor.OnCardboardTrigger -= ShootLaser;
            SceneManager.LoadScene("Game Over");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public static void QuitGame()
    {
        SaveGame.SaveScore(highscore);
        Application.Quit();
    }

    IEnumerator SpawnWaves()
    {
        Vector3 spawnPosition;

        StartCoroutine(Countdown((int)prepareWait, true));
        yield return new WaitForSeconds(prepareWait);

        while (true)
        {
            enemiesLeft = enemyCount;
            waveInProgress = true;

            for (int i = 0; i < enemyCount; i++)
            {
                spawnPosition = new Vector3(Random.Range(xMinF, xMaxF), Random.Range(yMinF, yMaxF), Random.Range(zMinF, zMaxF));
                   
                Quaternion spawnRotation = new Quaternion();
                GameObject obj = Instantiate(enemyPrefab, spawnPosition, spawnRotation) as GameObject;
                obj.transform.LookAt(GameObject.Find("Main Camera").transform);
                obj.transform.Rotate(90, 0, 0);

                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // Wait for all enemies to be destroyed
            while (enemiesLeft != 0)
            {
                yield return null;
            }

            waveNum += 1;
            Debug.Log(waveNum);
            timeBetweenSpawns = 1f + (2f / (1 + (waveNum / 3f)));
            enemyCount += 2;

            AddScore(waveScore);

            StartCoroutine(Countdown((int)timeBetweenWaves, false));
            waveInProgress = false;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    public IEnumerator Countdown(int time, bool begin)
    {
        if (begin)
        {
            scoreText.text = "";
            shieldText.text = "";

            for (int i = 0; i < time; i++)
            {
                countdownText.text = ("GAME BEGINS IN: " + (time - i));
                yield return new WaitForSeconds(1);
            }

            UpdateScore();
            UpdateShields();
        }
        else
        {
            for (int i = 0; i < time; i++)
            {
                countdownText.text = ("NEXT WAVE IN: " + (time - i));
                yield return new WaitForSeconds(1);
            }
        }

        countdownText.text = "";
    }

    public void EnemyDestroyed()
    {
        AddScore(enemyScore);
        enemiesLeft--;
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void LoseShield()
    {
        shieldsRemaining--;
        UpdateShields();
    }

    public void RestoreShield()
    {
        if (shieldsRemaining < maxShields)
        {
            shieldsRemaining++;
            UpdateShields();
        }
    }

    public void RestoreAllShields()
    {
        if (shieldsRemaining < maxShields)
        {
            shieldsRemaining = maxShields;
            UpdateShields();
        }
    }

    void UpdateShields()
    {
        shieldText.text = "Shields: " + shieldsRemaining;
    }

    // Shoot a laser after the specified amount of seconds
    IEnumerator ShootLaserOnTimer(float seconds)
    {
        //Debug.Log("Laser has been shot");
        // If this is already running, wait
        if (isLaserFiring)
            yield break;

        if (waveInProgress)
        {
            // Make sure we know where the camera is 
            GameObject camera = GameObject.Find("Main Camera");

            // Make sure we know it's executing
            isLaserFiring = true;

            // Wait the correct amount of time
            yield return new WaitForSeconds(seconds);

            // Actually shoot the laser
            GameObject newProjectile = (GameObject)Instantiate(laserPrefab, camera.transform.position, camera.transform.rotation);
            newProjectile.transform.Translate(Vector3.forward * 2, Space.Self);
            newProjectile.GetComponent<Transform>().localScale = new Vector3(camera.transform.localScale.x / 3, camera.transform.localScale.y / 3,
                camera.transform.localScale.z / 12);
            newProjectile.GetComponent<Rigidbody>().AddForce(newProjectile.transform.forward * speed);

            // Aaaand we're done
            isLaserFiring = false;
        }
    }

    public void ShootLaser()
    {
        if (waveInProgress)
        {
            // Make sure we know where the camera is 
            GameObject camera = GameObject.Find("Main Camera");

            // Actually shoot the laser
            GameObject newProjectile = (GameObject)Instantiate(laserPrefab, camera.transform.position, camera.transform.rotation);
            newProjectile.transform.Translate(Vector3.forward * 2, Space.Self);
            newProjectile.GetComponent<Transform>().localScale = new Vector3(camera.transform.localScale.x / 3, camera.transform.localScale.y / 3,
                camera.transform.localScale.z / 12);
            newProjectile.GetComponent<Rigidbody>().AddForce(newProjectile.transform.forward * speed);
        }
    }

    public void PauseGame()
    {
        if (!paused)
        {
            paused = true;
            Time.timeScale = 0;
        }
        else
        {
            paused = false;
            Time.timeScale = 1;
        }
    }

    public void ChangeControls()
    {
        switch (controls)
        {
            case 1: // Screen tap
                controls++;
                controlText.text = "Ctrls: Mag";
                MagnetSensor.OnCardboardTrigger += ShootLaser;
                break;
            case 2: // Magnet
                controls++;
                MagnetSensor.OnCardboardTrigger -= ShootLaser;
                controlText.text = "Ctrls: Time";
                break;
            case 3: // Timer
                controls = 1;
                controlText.text = "Ctrls: Tap";
                break;
        }
    }
}
