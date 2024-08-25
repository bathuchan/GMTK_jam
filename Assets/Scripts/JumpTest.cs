using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTest : MonoBehaviour
{
    public float groundCheckDistance = 0.1f; // Distance from the collider to check for the ground
    public LayerMask groundLayer; // Layer to detect as ground

    private CapsuleCollider capsuleCollider;
    private float colliderRadius;
    private Vector3 spherePosition;
    private bool showGizmos = false;

    void Start()
    {
        // Get the CapsuleCollider component
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Calculate the collider's radius
        colliderRadius = capsuleCollider.radius * 0.9f; // Slightly reduce to avoid edge cases
    }

    void Update()
    {
        // Check for jump input (using the default "Jump" input axis)
        if (Input.GetButtonDown("Jump"))
        {
            // Calculate the sphere position for the SphereCast
            spherePosition = transform.position + Vector3.up * (colliderRadius - capsuleCollider.center.y);

            // Trigger Gizmo drawing
            showGizmos = true;

            // Call IsGrounded to perform the actual SphereCast (not mandatory)
            IsGrounded();
        }
    }

    public bool IsGrounded()
    {
        // Perform the SphereCast to check if the player is grounded
        return Physics.SphereCast(transform.position, colliderRadius, Vector3.down, out RaycastHit hit, groundCheckDistance);
    }

    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            // Set Gizmo color
            Gizmos.color = Color.red;

            // Draw the initial sphere at the start of the SphereCast
            Gizmos.DrawWireSphere(spherePosition, colliderRadius);

            // Draw a line from the start position downward
            Gizmos.DrawLine(spherePosition, spherePosition + Vector3.down * groundCheckDistance);

            // Draw the end sphere at the bottom of the SphereCast
            Gizmos.DrawWireSphere(spherePosition + Vector3.down * groundCheckDistance, colliderRadius);

            // Reset the flag after drawing the gizmo
            showGizmos = false;
        }
    }
}
