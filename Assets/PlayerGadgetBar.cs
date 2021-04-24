using UnityEngine;
using UnityEngine.UI;

public class PlayerGadgetBar : MonoBehaviour
{
    [Tooltip("Image component displaying current ability")]
    public Image abilityFillImage;

    ThrowGrenadeAbility ability;

    private void Start()
    {
        PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

        ability = playerCharacterController.GetComponent<ThrowGrenadeAbility>();
    }

    void Update()
    {
        // update ability bar value
        abilityFillImage.fillAmount = ability.readyBar;
    }
}
