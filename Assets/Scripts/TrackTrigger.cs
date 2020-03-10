using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Road"))
        {
            transform.root.GetComponent<TrackGenerator>().TryAgain();
            Destroy(gameObject);
        }
    }
}
