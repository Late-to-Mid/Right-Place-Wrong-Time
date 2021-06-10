using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FillBarColorChange))]
public class PlayerAbilityBar : MonoBehaviour
{
    [Tooltip("Image component displaying current ability")]
    public Image abilityFillImage;

    [Tooltip("Hud element to be active when the ability is active")]
    public GameObject hudElement;

    [Header("Feedback")]
    [Tooltip("Component to animate the color when empty or full")]
    public FillBarColorChange FillBarColorChange;
    [Tooltip("Sharpness for the fill ratio movements")]
    public float barFillMovementSharpness = 20f;

    CharacterAbility ability;

    private void Start()
    {
        PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

        ability = playerCharacterController.GetComponent<CharacterAbility>();

        FillBarColorChange.Initialize(1f, 0);

        ability.onAbilityUsed += OnAbilityUsed;
        ability.onAbilityOver += OnAbilityOver;
    }

    void Update()
    {
        // update ability bar value
        float currentFillRatio = ability.readyBar;
        abilityFillImage.fillAmount = Mathf.Lerp(abilityFillImage.fillAmount, currentFillRatio, Time.deltaTime * barFillMovementSharpness);
        FillBarColorChange.UpdateVisual(currentFillRatio);
    }

    void OnAbilityUsed() 
    {
        hudElement.SetActive(true);
    }

    void OnAbilityOver()
    {
        hudElement.SetActive(false);
    }
}
