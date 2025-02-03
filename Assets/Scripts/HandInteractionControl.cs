using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandInteractionControl : MonoBehaviour
{
    public XRNode NodeController;
    private GameObject objectInHand;

    [Header("Grab Settings")]
    public Transform objectGrabTransform;
    public float grabRadius = 2f;
    public float throwForce = 10f;
    public LayerMask grabLayer;

    //Trigger controller
    private bool previousTriggerState = false;
    private bool triggerUp = false;

    //Hand velocity
    private Vector3 lastPosition;
    private Vector3 velocity;
    private readonly int velocitySampleSize = 8; // Adjustable size for smoothing
    private Queue<Vector3> velocitySamples = new Queue<Vector3>();
    private const float throwDamping = 0.85f;


    // Update is called once per frame
    void Update()
    {
        //Hand movement from device
        TrackInputHandMovement();

        if (objectInHand)
        {
            TrackHandMovement();

            if (triggerUp)
            {
                ReleaseObject();
                triggerUp = false;
            }
        }

        if (getClickInteraction())
        {
            GrabObject();
        }
    }

    public void triggerHaptics()
    {
        StartCoroutine(TriggerHaptics());
    }

    IEnumerator TriggerHaptics()
    {
        // Get the hand controller device
        InputDevice controller = InputDevices.GetDeviceAtXRNode(NodeController);

        // Trigger haptic feedback (intensity between 0 and 1, duration in seconds)
        if (controller.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse)
        {
            uint channel = 0; // Haptics usually use channel 0
            float amplitude = 0.5f; // Vibration intensity
            float duration = 0.2f; // Duration in seconds

            controller.SendHapticImpulse(channel, amplitude, duration);

            // Wait for the duration of the haptic feedback
            yield return new WaitForSeconds(duration);

            // Stop haptic feedback (optional, usually stops automatically after duration)
            controller.StopHaptics();
        }
    }

    private void TrackHandMovement()
    {
        Vector3 currentPosition = transform.position;
        Vector3 currentVelocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;

        // Smooth velocity by keeping a fixed number of samples
        velocitySamples.Enqueue(currentVelocity);
        //Remove the oldest sample below to always keep the same number of samples using Dequeue
        if (velocitySamples.Count > velocitySampleSize)
            velocitySamples.Dequeue();

        velocity = Vector3.zero;
        foreach (var sample in velocitySamples)
        {
            velocity += sample;
        }

        //calculate the average velocity by dividing the samples that was sum on velocity += sample 
        velocity /= velocitySamples.Count;
    }

    private void TrackInputHandMovement()
    {

        if (InputDevices.GetDeviceAtXRNode(NodeController).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePosition))
        {
            transform.localPosition = devicePosition;
        }
        if (InputDevices.GetDeviceAtXRNode(NodeController).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion deviceRotation))
        {
            transform.localRotation = deviceRotation;
        }

    }


    bool getClickInteraction()
    {
        if (InputDevices.GetDeviceAtXRNode(NodeController)
            .TryGetFeatureValue(CommonUsages.triggerButton, out bool isTriggerPressed))
        {
            bool triggerDown = isTriggerPressed && !previousTriggerState;
            triggerUp = !isTriggerPressed && previousTriggerState;
            previousTriggerState = isTriggerPressed;
            return triggerDown;
        }

        return false;
    }

    // Draw the grab radius sphere in the Scene view for debugging
    void OnDrawGizmos()
    {
        if (!objectGrabTransform) return;

        // Set the color of the Gizmo
        Gizmos.color = Color.red;

        // Draw a wireframe sphere at the hand's position with the grab radius
        Gizmos.DrawWireSphere(objectGrabTransform.position, grabRadius);
    }

    void GrabObject()
    {
        if (objectInHand) return;

        // Get all colliders within the grab radius
        Collider[] hitColliders = Physics.OverlapSphere(objectGrabTransform.position, grabRadius, grabLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            Debug.Log("Grab Ball");
            // Check if the object has the grab script
            ObjectGrab g_object = hitCollider.GetComponent<ObjectGrab>();

            if (g_object != null && !g_object.isGrabbed)
            {
                triggerUp = false;

                Rigidbody rb = g_object.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                g_object.transform.SetParent(objectGrabTransform);
                g_object.transform.localPosition = Vector3.zero;
                triggerHaptics();
                g_object.isGrabbed = true;
                g_object.grabAction();
                objectInHand = g_object.gameObject;
                lastPosition = objectGrabTransform.position;
                break;
            }
        }
    }

    private void ReleaseObject()
    {
        if (!objectInHand) return;

        Debug.Log("Object being released");
        objectInHand.transform.SetParent(null);

        Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            // Apply smoothed throw force with damping
            Vector3 smoothedVelocity = Vector3.Lerp(Vector3.zero, velocity, throwDamping);
            rb.AddForce(smoothedVelocity * throwForce, ForceMode.Impulse);
        }

        objectInHand.GetComponent<ObjectGrab>().isGrabbed = false;
        objectInHand.GetComponent<ObjectGrab>().releaseAction();
        objectInHand = null;
    }

}
