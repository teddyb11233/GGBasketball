using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Basketball : MonoBehaviour
{
    public Rigidbody Rb { get; private set; }

    private Transform followTarget;
    private bool isHeld;
    private Collider col;

    public bool IsHeld => isHeld;


    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }


    void LateUpdate()
    {
        if (!isHeld || followTarget == null)
            return;

        transform.position = followTarget.position;
        transform.rotation = followTarget.rotation;
    }

    public void SetHeld(bool held, Transform holdPoint)
    {
        isHeld = held;

        if (held)
        {
            followTarget = holdPoint;

            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
            Rb.isKinematic = true;

            // ?? KEY FIX
            col.enabled = false;
        }
        else
        {
            followTarget = null;
            Rb.isKinematic = false;

            // ?? RE-ENABLE COLLISION ON RELEASE
            col.enabled = true;
        }
    }

}
