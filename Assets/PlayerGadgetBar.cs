using UnityEngine;
using UnityEngine.UI;

public class PlayerGadgetBar : MonoBehaviour
{
    [Tooltip("Image component displaying current ability")]
    public Image abilityFillImage;

    [Header("Feedback")]
    [Tooltip("Component to animate the color when empty or full")]
    public FillBarColorChange FillBarColorChange;
    [Tooltip("Sharpness for the fill ratio movements")]
    public float barFillMovementSharpness = 20f;

    ThrowGrenadeAbility ability;

    private void Start()
    {
        PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

        ability = playerCharacterController.GetComponent<ThrowGrenadeAbility>();

        FillBarColorChange.Initialize(1f, 0);
    }

    void Update()
    {
        // update ability bar value
        float currentFillRatio = ability.readyBar;
        abilityFillImage.fillAmount = Mathf.Lerp(abilityFillImage.fillAmount, currentFillRatio, Time.deltaTime * barFillMovementSharpness);
        FillBarColorChange.UpdateVisual(currentFillRatio);
    }
}
