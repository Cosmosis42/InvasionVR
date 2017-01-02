using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public GameController gameController;

    [Header("Laser Info")]
    public GameObject laserPrefab;
    public float speed = 1.0f;
    public float delay = 1;

    private bool isLaserFiring = false;

    [Header("Behaviour")]
    public float moveSpeed;
    public int distance;
    float moveDirection = 1;
    public float frequency = 20.0f;
    public float magnitude = 0.5f;
    private Vector3 pos;
    public bool startSine = false;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        else if (gameControllerObject == null)
        {
            Debug.Log("Cannot find Game Controller object");
        }

        moveDirection = Random.Range(0f, 1f);

        if (moveDirection < 0.5)
            moveDirection = -1;
        else
            moveDirection = 1;

        if (gameController.waveNum > 0)
            delay = (float)(1.5 + (3.5 / (1 + (gameController.waveNum / 5))));
    }

    void Update()
    {
        StartCoroutine(ShootLaserOnTimer(delay));
        Move();
    }

    private void Move()
    {
        float distAway = Vector3.Distance(GameObject.Find("Main Camera").transform.position, transform.position);
        Vector3 direction = GameObject.Find("Main Camera").transform.position - transform.position;
        transform.forward = direction;
        transform.Rotate(90, 0, 0);

        if (distAway < distance && distAway > distance - 2)
        {
            transform.position += transform.right * moveSpeed * moveDirection;
            Debug.Log(Mathf.Sin(Time.deltaTime * frequency));
            transform.position += transform.forward * Mathf.Sin (Time.time * frequency) * moveSpeed;

        }
        else if (distAway < distance - 2)
        {
            transform.position -= transform.up * moveSpeed;
        }
        else
        {
            transform.position += transform.up * moveSpeed;
        }

        if (transform.position.z <= 0)
        {
            moveDirection *= -1;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Laser"))
        {
            gameController.EnemyDestroyed();
            print("I've been hit!");
            Destroy(col.gameObject);
            Destroy(gameObject);
        }
    }

    // Shoot a laser after the specified amount of seconds
    IEnumerator ShootLaserOnTimer(float seconds)
    {
        //Debug.Log("Laser has been shot");
        // If this is already running, wait
        if (isLaserFiring)
            yield break;

        // Make sure we know it's executing
        isLaserFiring = true;

        // Wait the correct amount of time
        yield return new WaitForSeconds(seconds);

        // Actually shoot the laser
        GameObject newProjectile = (GameObject)Instantiate(laserPrefab, transform.position, transform.rotation);
        newProjectile.transform.Rotate(-90, 0, 0);

        newProjectile.GetComponent<Rigidbody>().AddForce(newProjectile.transform.forward * speed);

        // Aaaand we're done
        isLaserFiring = false;
    }
}
