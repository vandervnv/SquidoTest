using UnityEngine;

public class BasketTrigger : MonoBehaviour
{
    public BasketTriggerType triggerType;
    private BasketManager basketManager;

    private void Start()
    {
        basketManager = GetComponentInParent<BasketManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            basketManager.TriggerEntered(triggerType);
        }
    }
}