using UnityEngine;

public class OutOfBoundsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Only care about basketballs
        if (!other.CompareTag("Ball"))
            return;

        // If the ball is currently held, ignore it
        Basketball ball = other.GetComponent<Basketball>();
        if (ball != null && ball.IsHeld)
            return;

        // Ball reset logic
        BallReset reset = other.GetComponent<BallReset>();
        if (reset == null)
            return;

        Debug.Log("Ball out of bounds - resetting");
        reset.ResetBall();
    }
}
