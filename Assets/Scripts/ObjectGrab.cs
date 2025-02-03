using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectGrab : MonoBehaviour
{
    private Vector3 startPosition;
    public bool isGrabbed = false;
    public UnityEvent releaseEvent;
    public UnityEvent grabEvent;
    public bool returnToStart = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void releaseAction()
    {
        if(returnToStart) Invoke("ReturnToStartPosition", 5f);
        releaseEvent?.Invoke();
    }

    public void grabAction()
    {
        CancelInvoke();
        grabEvent?.Invoke();
    }

    public void ReturnToStartPosition()
    {
        if(isGrabbed) return;

        Rigidbody rb = GetComponent<Rigidbody>();

        transform.position = startPosition;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }

    // Function that sets the layer and adds the Rigidbody when called from the Inspector when the script is added
    private void Reset()
    {
        // Automatically set the layer to "GrabMe"
        int layerIndex = LayerMask.NameToLayer("GrabMe");
        if (layerIndex != -1)
        {
            gameObject.layer = layerIndex;
        }
        else
        {
            Debug.LogWarning("The 'GrabMe' layer does not exist. Please add it in the Tags and Layers settings.");
        }

        // Ensure the GameObject has a Rigidbody, add one if necessary
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;  // Optional: Make the Rigidbody kinematic (no physics simulation)
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        // Check if the object has any Collider component attached
        if (GetComponent<Collider>() == null)
        {
            // If no collider is found, add a BoxCollider (you can choose other types too)
            gameObject.AddComponent<BoxCollider>();
            Debug.Log("No Collider found. Adding BoxCollider.");
        }
    }
}
