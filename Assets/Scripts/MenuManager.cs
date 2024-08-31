using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject _mainMenuCanvasGO;
    [SerializeField] private GameObject _settingsMenuCanvasGO;
    [SerializeField] private PlayerInputController playerInputController;

    private bool isPaused;

    [Header("First Selected Options")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;

    [Header("Settings Sliders and texts")]
    [SerializeField] private Toggle camWobble;
    [SerializeField] private GameObject _verticalSliderGO;
    [SerializeField] private GameObject _horizontalSliderGO;
    private Slider _verticalSlider, _horizontalSlider;
    [SerializeField] private TextMeshProUGUI _verticalNumberText;
    [SerializeField] private TextMeshProUGUI _horizontalNumberText;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference gamepadAction;

    

    public float sliderSensitivity = 0.1f; // Adjust this to control how much the slider moves per input

    bool intToBool(int val)
    {
        if (val != 0)
            return true;
        else
            return false;
    }
    private void Awake()
    {
        _verticalSlider = _verticalSliderGO.GetComponent<Slider>();
        _horizontalSlider = _horizontalSliderGO.GetComponent<Slider>();

        _verticalSlider.value = PlayerPrefs.GetFloat("verticalSens");
        _verticalNumberText.text = _verticalSlider.value.ToString("0.00");
        _horizontalSlider.value = PlayerPrefs.GetFloat("horizontalSens");
        _horizontalNumberText.text = _horizontalSlider.value.ToString("0.00");
        camWobble.isOn = intToBool(PlayerPrefs.GetInt("camWobble"));
    }
    private void Start()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        playerInputController = GameObject.FindAnyObjectByType<PlayerInputController>();




        _verticalSlider.onValueChanged.AddListener((v1) =>
        {
            playerInputController._lookSensitivityVertical = v1;
            _verticalNumberText.text = v1.ToString("0.00");
            PlayerPrefs.SetFloat("verticalSens", v1);
            PlayerPrefs.Save();
        });
        _horizontalSlider.onValueChanged.AddListener((v2) =>
        {
            playerInputController._lookSensitivityHorizontal = v2;
            _horizontalNumberText.text = v2.ToString("0.00");
            PlayerPrefs.SetFloat("horizontalSens", v2);
            PlayerPrefs.Save();
        });


    }

    private void OnEnable()
    {
       // gamepadAction.action.performed += OnGamepadInput;
        InputSystem.onAnyButtonPress.Call(UpdateInputDevice);

        //gamepadAction.action.Enable();
        
    }

    private void OnDisable()
    {
        //gamepadAction.action.performed -= OnGamepadInput;
        InputSystem.onAnyButtonPress.Call(null);

       // gamepadAction.action.Disable();
        
    }

    private void OnGamepadInput()
    {
        
        if (EventSystem.current.currentSelectedGameObject == null )
        {
            SetSelectedGameObjectForGamepad();
        }
        else
        {
            // Force re-selection to trigger visual update
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }
    }

    

    private void SetSelectedGameObjectForGamepad()
    {
        // Logic to determine which object should be selected when using a gamepad
        if (EventSystem.current.currentSelectedGameObject == null) 
        {
            if (_settingsMenuCanvasGO.activeSelf) 
            {
                EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
            } 
            else if(_mainMenuCanvasGO.activeSelf) 
            {
                EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
            }
           
        }
        
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // Check if the device was added or became the current device
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected || change == InputDeviceChange.ConfigurationChanged)
        {
            
            UpdateInputDevice(device.allControls[0]);
        }
    }

    private void UpdateInputDevice(InputControl control)
    {
        if (control.device is Gamepad)
        {
            if (EventSystem.current.currentSelectedGameObject == null )
            {
                OnGamepadInput();
            }
            //else
            //{
            //    GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            //    EventSystem.current.SetSelectedGameObject(null);
            //    EventSystem.current.SetSelectedGameObject(currentSelected);
            //}
        }
        //else if (control.device is Mouse || control.device is Keyboard)
        //{
        //    EventSystem.current.SetSelectedGameObject(null);
        //}
    }


   

    private void Update()
    {
        if (InputManage.Instance.MenuOpenCloseInput)
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }

        // Handle slider input for gamepad
        if (EventSystem.current.currentSelectedGameObject == _verticalSliderGO && Gamepad.current != null && isPaused)
        {
            float input = Input.GetAxis("Horizontal");
            if (Mathf.Abs(input) > 0.01f)
            {
                _verticalSlider.value += input * sliderSensitivity;
            }
        }
        else if (EventSystem.current.currentSelectedGameObject == _horizontalSliderGO && Gamepad.current != null && isPaused)
        {
            float input = Input.GetAxis("Horizontal");
            if (Mathf.Abs(input) > 0.01f)
            {
                _horizontalSlider.value += input * sliderSensitivity;
            }
        }
    }

    #region Pause/Unpause Functions

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0.0f;

        playerInputController.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        OpenMainMenu();
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1.0f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        CloseAllMenu();
        playerInputController.enabled = true;
    }

    #endregion

    #region Canvas Activations

    private void OpenMainMenu()
    {
        _mainMenuCanvasGO.SetActive(true);
        _settingsMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    private void CloseAllMenu()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OpenSettingsMenuHandle()
    {
        _settingsMenuCanvasGO.SetActive(true);
        _mainMenuCanvasGO.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }

    #endregion

    #region Main Menu Button Actions

    public void OnSettingsPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnResumePress()
    {
        Unpause();
    }

    #endregion

    #region Settings Menu Button Actions

    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }

    #endregion
}