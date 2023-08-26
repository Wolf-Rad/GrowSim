using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectInteractor : MonoBehaviour
{
    public GameObject canvasObject;

    public TMP_Text objectName;
    public TMP_Text objectDescription;
    private InputAction clickAction;
    private ObjectStats objectStats;

    private void Awake()
    {
        clickAction = new InputAction("Click", InputActionType.Button, "<Mouse>/leftButton");
        clickAction.performed += ctx => OnClick();
        clickAction.Enable();
    }

    void Start()
    {
        objectStats = GetComponent<ObjectStats>();
        

        // Initially disable the Canvas
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
        
    }

    void Update()
    {
        if (canvasObject != null && canvasObject.activeSelf)
        {
            // Make the canvasObject face towards the camera
            canvasObject.transform.LookAt(
                canvasObject.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }


    private void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == this.transform)
            {
                Debug.Log("Clicked on this GameObject: " + gameObject.name);
                
                // Enable the Canvas
                if (canvasObject != null)
                {
                    canvasObject.SetActive(true);
                    objectName.text = objectStats.name;
                    objectDescription.text = objectStats.objectDescription;
                }
            }
            else
            {
                // If we clicked somewhere else, disable the Canvas
                if (canvasObject != null)
                {
                    canvasObject.SetActive(false);
                }
            }
        }
    }

    private void OnDestroy()
    {
        clickAction.performed -= ctx => OnClick();
        clickAction.Disable();
    }
}
