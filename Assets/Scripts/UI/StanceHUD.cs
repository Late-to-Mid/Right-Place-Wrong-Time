using UnityEngine;
using UnityEngine.UI;

public class StanceHUD : MonoBehaviour
{
    [Tooltip("Image component for the stance sprites")]
    public Image stanceImage;
    [Tooltip("Sprite to display when standing")]
    public Sprite standingSprite;
    [Tooltip("Sprite to display when crouching")]
    public Sprite crouchingSprite;
    [Tooltip("Sprite to display when sprinting")]
    public Sprite sprintingSprite;

    private void Start()
    {
        PlayerCharacterController character = FindObjectOfType<PlayerCharacterController>();
        character.onStanceChanged += OnStanceChanged;

        OnStanceChanged(character.isCrouching, character.isSprinting);
    }

    void OnStanceChanged(bool crouched, bool sprinting)
    {
        if (crouched)
        {
            stanceImage.sprite = crouchingSprite;
        }
        else if (sprinting)
        {
            stanceImage.sprite = sprintingSprite;
        }
        else
        {
            stanceImage.sprite = standingSprite;
        }
    }
}
