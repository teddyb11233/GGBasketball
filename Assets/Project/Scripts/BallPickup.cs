using UnityEngine;
using StarterAssets;

public class BallPickup : MonoBehaviour
{
    [Header("Pickup")]
    public Transform ballHoldPoint;
    public float pickupRadius = 2.0f;
    public LayerMask basketballLayer;

    [Header("Hoop")]
    public Transform rimTarget; // center of rim + slight upward offset

    [Header("Timing")]
    public float idealHoldTime = 0.35f;   // seconds
    public float greenWindow = 0.06f;     // +/- seconds
    public float maxHoldTime = 1.0f;
    public float missMakeChance = 0.25f;

    [Header("Ballistic Arc")]
    public float baseArcHeight = 1.1f;
    public float arcPerMeter = 0.08f;
    public float minArcHeight = 0.9f;
    public float maxArcHeight = 2.2f;

    [Header("Miss Error")]
    public float maxHorizontalError = 0.55f;
    public float errorStartsAfter = 0.10f;

    private Basketball heldBall;
    private StarterAssetsInputs inputs;

    private bool isCharging;
    private float chargeStartTime;

    void Awake()
    {
        inputs = GetComponent<StarterAssetsInputs>();
    }

    private bool prevShootState;

    void Update()
    {
        // ---------- INTERACT ----------
        if (inputs.interact)
        {
            inputs.interact = false;

            if (heldBall == null)
                TryPickupBall();
            else
                DropBall();
        }

        bool shootPressed = inputs.shoot;

        // ---------- START CHARGING ----------
        if (heldBall != null && shootPressed && !prevShootState)
        {
            isCharging = true;
            chargeStartTime = Time.time;
        }

        // ---------- RELEASE TO SHOOT ----------
        if (isCharging && !shootPressed && prevShootState)
        {
            isCharging = false;

            float heldTime = Mathf.Clamp(
                Time.time - chargeStartTime,
                0f,
                maxHoldTime
            );

            ShootWithTiming(heldTime);
        }

        prevShootState = shootPressed;
    }



    // =====================================================
    // PICKUP / DROP
    // =====================================================

    void TryPickupBall()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            pickupRadius,
            basketballLayer
        );

        if (hits.Length == 0)
            return;

        Collider closest = hits[0];
        float bestDist = Vector3.Distance(transform.position, closest.transform.position);

        for (int i = 1; i < hits.Length; i++)
        {
            float d = Vector3.Distance(transform.position, hits[i].transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                closest = hits[i];
            }
        }

        Basketball ball = closest.GetComponent<Basketball>();
        if (ball == null)
            return;

        heldBall = ball;
        heldBall.SetHeld(true, ballHoldPoint);
    }

    void DropBall()
    {
        heldBall.SetHeld(false, null);
        heldBall = null;
    }

    // =====================================================
    // SHOOTING
    // =====================================================

    void ShootWithTiming(float heldTime)
    {
        if (rimTarget == null)
        {
            Debug.LogWarning("RimTarget not assigned.");
            return;
        }

        Basketball ball = heldBall;
        heldBall = null;
        ball.SetHeld(false, null);

        Vector3 start = ball.transform.position;
        Vector3 target = rimTarget.position;

        // Distance-based arc
        float planarDist = Vector3.Distance(
            new Vector3(start.x, 0f, start.z),
            new Vector3(target.x, 0f, target.z)
        );

        float arcHeight = Mathf.Clamp(
            baseArcHeight + planarDist * arcPerMeter,
            minArcHeight,
            maxArcHeight
        );

        bool isGreen = Mathf.Abs(heldTime - idealHoldTime) <= greenWindow;

        // ---------- PERFECT SHOT ----------
        if (isGreen)
        {
            if (SolveBallisticArc(start, target, arcHeight, out Vector3 velocity))
            {
                ball.Rb.linearVelocity = velocity;
            }
            return;
        }

        // ---------- MISSED TIMING ----------
        bool stillMake = Random.value < missMakeChance;
        Vector3 finalTarget = target;

        if (!stillMake)
        {
            float delta = Mathf.Abs(heldTime - idealHoldTime) - greenWindow;
            float severity = Mathf.InverseLerp(errorStartsAfter, 0.6f, delta);
            severity = Mathf.Clamp01(severity);

            float distFactor = Mathf.Clamp01(planarDist / 10f);
            float error = maxHorizontalError * severity * (0.5f + 0.5f * distFactor);

            Vector3 right = Vector3.Cross(Vector3.up, (finalTarget - start)).normalized;
            finalTarget += right * Random.Range(-error, error);
        }

        if (SolveBallisticArc(start, finalTarget, arcHeight, out Vector3 v))
        {
            ball.Rb.linearVelocity = v;
        }
    }

    // =====================================================
    // BALLISTIC SOLVER
    // =====================================================

    bool SolveBallisticArc(
        Vector3 start,
        Vector3 target,
        float arcHeight,
        out Vector3 velocity)
    {
        velocity = Vector3.zero;

        float g = Mathf.Abs(Physics.gravity.y);

        float apexY = Mathf.Max(start.y, target.y) + arcHeight;

        float timeUp = Mathf.Sqrt(
            2f * Mathf.Max(0.01f, apexY - start.y) / g
        );

        float timeDown = Mathf.Sqrt(
            2f * Mathf.Max(0.01f, apexY - target.y) / g
        );

        float totalTime = timeUp + timeDown;
        if (totalTime < 0.05f)
            return false;

        Vector3 planarStart = new Vector3(start.x, 0f, start.z);
        Vector3 planarTarget = new Vector3(target.x, 0f, target.z);
        Vector3 planarDelta = planarTarget - planarStart;

        Vector3 planarVelocity = planarDelta / totalTime;
        float verticalVelocity = g * timeUp;

        velocity = new Vector3(
            planarVelocity.x,
            verticalVelocity,
            planarVelocity.z
        );

        return true;
    }

    // =====================================================
    // DEBUG
    // =====================================================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
