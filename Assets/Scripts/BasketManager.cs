using UnityEngine;

public class BasketManager : MonoBehaviour
{
    public GameController gameController;
    private bool passedTopTrigger = false;

    public void TriggerEntered(BasketTriggerType triggerType)
    {
        if (triggerType == BasketTriggerType.TopTrigger)
        {
            passedTopTrigger = true;
            Debug.Log("Ball passed top trigger.");
        }
        else if (triggerType == BasketTriggerType.BottomTrigger && passedTopTrigger)
        {
            gameController.AddPoints(15);
            Debug.Log("Basket Scored! +15 Points");
            passedTopTrigger = false; // Reset for next scoring attempt
        }
    }
}
