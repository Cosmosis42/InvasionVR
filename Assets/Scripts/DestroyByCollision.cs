using UnityEngine;
using System.Collections;

public class DestroyByCollision : MonoBehaviour {

    public GameController gameController;

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
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Laser"))
        {
            Destroy(gameObject);
            Destroy(col.gameObject);
        }
        else if (col.gameObject.CompareTag("MainCamera"))
        {
            gameController.LoseShield();
            Destroy(gameObject);
        }
    }
}
