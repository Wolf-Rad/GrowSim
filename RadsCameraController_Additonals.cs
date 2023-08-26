using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Cameras;
using UnityEngine.InputSystem;

public class RadsCameraController_Additonals : MonoBehaviour
{
    private CameraController cameraController;
    private InputAction shiftAction;
    public InputActionAsset inputActions;

    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float shiftSpeed = 69f;
    [SerializeField]
    private float rotationSpeed = 10f;
    [SerializeField]
    private float shiftRotationSpeed = 69f;
    [SerializeField]
    private float zoomSpeed = 10f;
    [SerializeField]
    private float shiftZoomSpeed = 69f;
    [SerializeField]
    private float zoomMin = 1f;
    [SerializeField]
    private float zoomMax = 10f;
    [SerializeField]
    private float zoomDefault = 5f;
    [SerializeField]  
    private float zoomSensitivity = 1f;
    [SerializeField]
    private float zoomSmoothing = 0.1f;
    [SerializeField]
    private float zoomSmoothingSpeed = 10f;
    [SerializeField]
    private float zoomSmoothingShiftSpeed = 69f;
    [SerializeField]
    private float zoomSmoothingMin = 0.1f;
    [SerializeField]
    private float zoomSmoothingMax = 1f;
    [SerializeField]    
    private float zoomSmoothingDefault = 0.1f;
    [SerializeField]
    private float zoomSmoothingSensitivity = 1f;
    [SerializeField]
    private float zoomSmoothingSensitivityShift = 69f;
    [SerializeField]
    private float zoomSmoothingSensitivityMin = 0.1f;
    [SerializeField]
    private float zoomSmoothingSensitivityMax = 1f;

    void Start()
    {
        cameraController = GetComponent<CameraController>();
        shiftAction = inputActions.FindAction("ModifierShift");
    }

    void Update()
    {
        FreeFlightMotor freeCamMotor = cameraController.GetMotor("Free Cam") as FreeFlightMotor;
        if (freeCamMotor != null)
        {
            if (shiftAction.IsPressed())
            {
                freeCamMotor.Speed = shiftSpeed;
            }
            else
            {
                freeCamMotor.Speed = speed;
            }
        }
    }
}
