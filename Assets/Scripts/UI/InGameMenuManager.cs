using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using PlayerScripts;

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
    public InputField lookSensitivityInput;
    [Tooltip("Toggle component for invincibility")]
    public Toggle invincibilityToggle;
    [Tooltip("Toggle component for framerate display")]
    public Toggle framerateToggle;
    [Tooltip("GameObject for the controls")]
    public GameObject controlImage;

    [Header("Tabs for menu navigation")]
    [Tooltip("General Menu")]
    public GameObject menuGeneral;
    [Tooltip("Controls Menu")]
    public GameObject menuControls;
    

    PlayerInputHandler m_PlayerInputHandler;
    PlayerCharacterController m_PlayerCharacterController;
    Health m_PlayerHealth;
    FramerateCounter m_FramerateCounter;

    public UnityAction<bool> onPause;

    GameObject activeMenu;

    void Start()
    {
        m_PlayerInputHandler = FindObjectOfType<PlayerInputHandler>();
        m_PlayerInputHandler.onMenu += OnMenu;

        m_PlayerHealth = m_PlayerInputHandler.GetComponent<Health>();
        m_PlayerCharacterController = m_PlayerInputHandler.GetComponent<PlayerCharacterController>();

        m_FramerateCounter = FindObjectOfType<FramerateCounter>();

        menuRoot.SetActive(false);
        menuGeneral.SetActive(true);
        menuControls.SetActive(false);
        activeMenu = menuGeneral;

        LoadSensitivityValue();

        SetPauseMenuActivation(false);
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

    public void OnSensitivityValueChanged()
    {
        if (m_PlayerCharacterController)
        {
            m_PlayerCharacterController.lookSensitivity = lookSensitivitySlider.value;
            lookSensitivityInput.text = lookSensitivitySlider.value.ToString();
        }

        SaveSensitivityValue();
    }

    public void OnMouseSensitivityChangedInput()
    {
        string stringValue = lookSensitivityInput.text;
        if (stringValue != null)
        {
            float newValue = float.Parse(stringValue);
            m_PlayerCharacterController.lookSensitivity = newValue;
            lookSensitivitySlider.value = newValue;
        }

        SaveSensitivityValue();
    }

    void SaveSensitivityValue()
    {
        if (m_PlayerCharacterController)
        {
            PlayerPrefs.SetFloat("Mouse Sensitivity", m_PlayerCharacterController.lookSensitivity);        
        }
    }

    void LoadSensitivityValue()
    {
        float lookSensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity", 1);
        m_PlayerCharacterController.lookSensitivity = lookSensitivity;
        lookSensitivitySlider.value = lookSensitivity;
        lookSensitivityInput.text = lookSensitivity.ToString();
    }

    public void OnGeneralMenuClicked()
    {
        if (activeMenu != menuGeneral)
        {
            activeMenu.SetActive(false);
            menuGeneral.SetActive(true);
            activeMenu = menuGeneral;
        }
    }

    public void OnControlsMenuClicked()
    {
        if (activeMenu != menuControls)
        {
            activeMenu.SetActive(false);
            menuControls.SetActive(true);
            activeMenu = menuControls;
        }
    }
}
