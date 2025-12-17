using UnityEngine;

public class OutOfBoundsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
            return;

        BallReset reset = other.GetComponent<BallReset>();
        if (reset == null)
            return;

        Debug.Log("Ball out of bounds — resetting");
        reset.ResetBall();
    }
}
