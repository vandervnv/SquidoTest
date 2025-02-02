using UnityEngine;
using UnityEngine.XR;

public class GameController : MonoBehaviour
{
    public int score = 0;
    public int pointsPerBasket = 15;
    public Transform VRroot;
    public Transform initPosition;

    [Header("Shader Material")]
    public Material scoreMaterial;
    private const string shaderProperty = "_numberCount";

    void Start()
    {
        UpdateScoreMaterial();
    }

    public void AddPoints(int points)
    {
        score += points;
        UpdateScoreMaterial();
    }

    private void UpdateScoreMaterial()
    {
        if (scoreMaterial != null)
        {
            //As score is a float inside the shader we need a workaround to keep it round adding 0.1f does the trick
            scoreMaterial.SetFloat(shaderProperty, (float)score+0.1f);
        }
        else
        {
            Debug.LogWarning("Score material not assigned!");
        }
    }

    //Reset Camera to the desired position
    public void resetVrCamera(Transform newPosition)
    {
        var rotationAngleY = initPosition.rotation.eulerAngles.y - Camera.main.transform.rotation.eulerAngles.y;

        // Set the cameraHolder position and basic rotation
        VRroot.transform.Rotate(0, rotationAngleY, 0);
        VRroot.position = newPosition.position;
    }

    private bool previousButtonState = false;

    bool getButtonInteraction()
    {
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand)
            .TryGetFeatureValue(CommonUsages.primaryButton, out bool isPrimaryPressed))
        {
            // Return true on button down, and reset when released
            bool buttonDown = isPrimaryPressed && !previousButtonState;
            previousButtonState = isPrimaryPressed;
            return buttonDown;
        }

        return false;
    }


    private void Update()
    {
        //reset the game and camera position
        if (getButtonInteraction())
        {
            resetVrCamera(initPosition);
            score = 0;
            UpdateScoreMaterial();
        }
    }
}

