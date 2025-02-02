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
        if(releaseEvent != null) releaseEvent.Invoke();
    }

    public void grabAction()
    {
        CancelInvoke();
        if (grabEvent != null) grabEvent.Invoke();
    }

    public void ReturnToStartPosition()
    {
        Debug.Log("volta");
        if(isGrabbed) return;

        transform.position = startPosition;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
