using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TestTargetShot : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform hoopTarget;

    [Header("Shot Power")]
    [SerializeField] private float minForce = 6f;
    [SerializeField] private float maxForce = 14f;
    [SerializeField] private float maxEffectiveDistance = 12f;

    [Header("Arc Control")]
    [SerializeField] private float verticalBoost = 0.35f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ShootAtTarget();
        }
    }

    private void ShootAtTarget()
    {
        if (hoopTarget == null)
            return;

        // Reset motion
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 toTarget = hoopTarget.position - transform.position;

        // Horizontal-only direction
        Vector3 horizontal = new Vector3(toTarget.x, 0f, toTarget.z);
        float horizontalDistance = horizontal.magnitude;
        Vector3 forwardDir = horizontal.normalized;

        float t = Mathf.Clamp01(horizontalDistance / maxEffectiveDistance);
        float forwardForce = Mathf.Lerp(minForce, maxForce, t);
        float verticalForce = forwardForce * verticalBoost;

        Vector3 impulse =
            forwardDir * forwardForce +
            Vector3.up * verticalForce;

        rb.AddForce(impulse, ForceMode.VelocityChange);
    }

}
