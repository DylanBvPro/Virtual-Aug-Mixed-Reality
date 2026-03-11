using UnityEngine;

public class floatingRockAPI : MonoBehaviour
{
    [SerializeField] private GameObject rockObject;
    [SerializeField] private string targetObjectName = "ironrock2";
    [SerializeField] private float startStillSeconds = 4f;
    [SerializeField] private float spinSpeed = 10f;
    [SerializeField] private float moveSpeed = 0.0f;
    [SerializeField] private float bounceMultiplier = 0.0f;
    [SerializeField] private Vector3 initialDirection = new Vector3(0f, 0f, 0f);

    private GameObject targetObject;
    private Rigidbody targetRigidbody;
    private Vector3 moveDirection;
    private float moveStartTime;
    private bool hasDetached;

    private class CollisionRelay : MonoBehaviour
    {
        public floatingRockAPI owner;

        private void OnCollisionEnter(Collision collision)
        {
            if (owner != null)
            {
                owner.HandleCollision(collision);
            }
        }
    }

    void Start()
    {
        targetObject = rockObject;

        if (targetObject == null && !string.IsNullOrWhiteSpace(targetObjectName))
        {
            targetObject = GameObject.Find(targetObjectName);
        }

        if (targetObject == null)
        {
            Debug.LogError($"floatingRockAPI could not find target object '{targetObjectName}'. Assign rockObject in the Inspector or ensure the object name matches.");
            return;
        }

        moveStartTime = Time.time + Mathf.Max(0f, startStillSeconds);
    hasDetached = false;

        targetRigidbody = targetObject.GetComponent<Rigidbody>();
        if (targetRigidbody == null)
        {
            targetRigidbody = targetObject.AddComponent<Rigidbody>();
        }

        targetRigidbody.useGravity = false;
        targetRigidbody.isKinematic = false;
        targetRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        targetRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        targetRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        moveDirection = initialDirection.sqrMagnitude > 0f ? initialDirection.normalized : Vector3.right;

        CollisionRelay relay = targetObject.GetComponent<CollisionRelay>();
        if (relay == null)
        {
            relay = targetObject.AddComponent<CollisionRelay>();
        }

        relay.owner = this;
    }

    void Update()
    {
        if (targetObject == null)
        {
            return;
        }

        if (!hasDetached && Time.time >= moveStartTime)
        {
            DetachFromParentHierarchy();
        }

        if (Time.time < moveStartTime)
        {
            return;
        }

        targetObject.transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.Self);
    }

    void FixedUpdate()
    {
        if (targetRigidbody == null)
        {
            return;
        }

        if (Time.time < moveStartTime)
        {
            targetRigidbody.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 nextPosition = targetRigidbody.position + (moveDirection * moveSpeed * Time.fixedDeltaTime);
        targetRigidbody.MovePosition(nextPosition);
    }

    private void HandleCollision(Collision collision)
    {
        if (targetRigidbody == null || collision.contactCount == 0 || Time.time < moveStartTime)
        {
            return;
        }

        Vector3 normal = collision.GetContact(0).normal;
        moveDirection = Vector3.Reflect(moveDirection, normal).normalized;
        targetRigidbody.linearVelocity = moveDirection * moveSpeed * Mathf.Max(1f, bounceMultiplier);
    }

    private void DetachFromParentHierarchy()
    {
        if (targetObject == null)
        {
            return;
        }
        Debug.Log("Detaching rock from parent hierarchy to allow independent movement.");
        targetObject.transform.SetParent(null, true);
        hasDetached = true;
    }
}
