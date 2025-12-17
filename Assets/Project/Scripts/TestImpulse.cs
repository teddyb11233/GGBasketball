using UnityEngine;

public class TestImpulse : MonoBehaviour
{
    [SerializeField] private Vector3 impulseDirection = Vector3.forward;
    [SerializeField] private float impulseStrength = 3f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // One-time physics push when Play starts
        rb.AddForce(impulseDirection.normalized * impulseStrength, ForceMode.Impulse);
    }
}
