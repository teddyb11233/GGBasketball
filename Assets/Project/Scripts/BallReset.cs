using UnityEngine;

public class BallReset : MonoBehaviour
{
    [SerializeField] private Vector3 resetPosition;
    [SerializeField] private Quaternion resetRotation = Quaternion.identity;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ResetBall()
    {
        // Stop physics influence
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Temporarily disable physics so teleporting is clean
        rb.isKinematic = true;

        // Move the ball
        transform.position = resetPosition;
        transform.rotation = resetRotation;

        // Re-enable physics
        rb.isKinematic = false;
    }
}
