using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilityBar : MonoBehaviour
{
    [Tooltip("Image component displaying current ability")]
    public Image abilityFillImage;

    CharacterAbility ability;

    private void Start()
    {
        PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

        ability = playerCharacterController.GetComponent<CharacterAbility>();
    }

    void Update()
    {
        // update ability bar value
        abilityFillImage.fillAmount = ability.readyBar;
    }
}
