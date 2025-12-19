using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Basketball"))
            return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && rb.linearVelocity.y < 0f) // only count downward passes
        {
            Debug.Log("SCORE ? Ball through net");
        }
    }
}
