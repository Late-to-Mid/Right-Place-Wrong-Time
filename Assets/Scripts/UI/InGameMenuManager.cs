using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InGameMenuManager : MonoBehaviour
{
    [Tooltip("Root GameObject of the menu used to toggle its activation")]
    public GameObject menuRoot;
    [Tooltip("Master volume when menu is open")]
    [Range(0.001f, 1f)]
    public float volumeWhenMenuOpen = 0.5f;
    [Tooltip("Slider component for look sensitivity")]
    public Slider lookSensitivitySlider;
    [Tooltip("Input Text field for sensitivity")]
    public TMPro.TMP_InputField lookSensitivityInput;
    [Tooltip("Toggle component for invincibility")]
    public Toggle invincibilityToggle;
    [Tooltip("Toggle component for framerate display")]
    public Toggle framerateToggle;
    [Tooltip("GameObject for the controls")]
    public GameObject controlImage;

    PlayerInputHandler m_PlayerInputHandler;
    PlayerCharacterController m_PlayerCharacterController;
    Health m_PlayerHealth;
    FramerateCounter m_FramerateCounter;

    public UnityAction<bool> onPause;

    void Start()
    {
        m_PlayerInputHandler = FindObjectOfType<PlayerInputHandler>();
        m_PlayerInputHandler.onMenu += OnMenu;

        m_PlayerHealth = m_PlayerInputHandler.GetComponent<Health>();
        m_PlayerCharacterController = m_PlayerInputHandler.GetComponent<PlayerCharacterController>();

        m_FramerateCounter = FindObjectOfType<FramerateCounter>();

        menuRoot.SetActive(false);
    }
    public void OnMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            SetPauseMenuActivation(!menuRoot.activeSelf);
        }
    }

    public void ClosePauseMenu()
    {
        SetPauseMenuActivation(false);
    }

    void SetPauseMenuActivation(bool active)
    {
        menuRoot.SetActive(active);

        if (menuRoot.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            // AudioUtility.SetMasterVolume(volumeWhenMenuOpen);

            // EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            // AudioUtility.SetMasterVolume(1);
        }

    }

    public void OnMouseSensitivityChangedInput(string stringValue)
    {
        // if (stringValue != null)
        // {
        //     float newValue = float.Parse(stringValue);
        //     m_PlayerInputHandler.lookSensitivity = newValue;
        //     lookSensitivitySlider.value = newValue;
        // }
    }

    void OnShadowsChanged(bool newValue)
    {
        QualitySettings.shadows = newValue ? ShadowQuality.All : ShadowQuality.Disable;
    }

    public void OnInvincibilityChanged(bool newValue)
    {
        if (m_PlayerHealth)
        {
            m_PlayerHealth.invincible = newValue;
        }
    }

    void OnFramerateCounterChanged(bool newValue)
    {
        m_FramerateCounter.uiText.gameObject.SetActive(newValue);
    }

    public void OnShowControlButtonClicked(bool show)
    {
        controlImage.SetActive(show);
    }

    public void OnSensitivityValueChanged()
    {
        if (m_PlayerCharacterController)
        {
            m_PlayerCharacterController.lookSensitivity = lookSensitivitySlider.value;
            lookSensitivityInput.text = lookSensitivitySlider.value.ToString();
        }
    }
}
