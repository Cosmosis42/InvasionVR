using UnityEngine;
using System.Collections;

public class DestroyByBorder : MonoBehaviour {

	void OnTriggerExit(Collider col)
    {
        Destroy(col.gameObject);
    }
}
