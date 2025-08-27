using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInteraction : MonoBehaviour
{
    private PlayerControls playerInput;
    private InputAction plyPointerPos;
    private InputAction plyLeftClick;

    [Header("Interact Variables")]
    [SerializeField] private GameObject interactArea; // this is an invisible collider that follows mouse pointer and is used to check object it "collides" with
    private GameObject interactHovered; // stores what item player is hovering over with pointer
    private GameObject interactItemHeld; // stores what item the player clicked on to pick up
    private bool isGrabbable;

    private enum HandState
    {
        HELD,
        EMPTY
    }
    private HandState currentHandState;


    private void Awake()
    {
        playerInput = new PlayerControls();
    }
    private void Start()
    {
        isGrabbable = false;
        currentHandState = HandState.EMPTY;
    }
    private void OnEnable()
    {
        plyPointerPos = playerInput.Player.MousePosition;
        plyPointerPos.Enable();

        plyLeftClick = playerInput.Player.LeftClick;
        plyLeftClick.Enable();
    }
    private void OnDisable()
    {
        plyLeftClick.Disable();
        plyPointerPos.Disable();
    }

    public void PlayerLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed && currentHandState == HandState.EMPTY)
        {
            if (isGrabbable)
            {
                interactHovered.transform.SetParent(this.transform);
                interactItemHeld = interactHovered;
                ChangeCurrentHandState(HandState.HELD);
            }
            else
            {
                return;
            }

        }
        else if (context.performed && currentHandState == HandState.HELD)
        {
            interactItemHeld.transform.SetParent(null);
            ChangeCurrentHandState(HandState.EMPTY);

        }
    }
    private void Update()
    {
        InteractAreaPosition();
        HoldState();

        Debug.Log("Grabble?: " + isGrabbable);
        Debug.Log("Object Hovered: " + interactHovered);
        Debug.Log("Current State: " + currentHandState);
    }
    private void InteractAreaPosition()
    {
        Vector2 mousePos = plyPointerPos.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        interactArea.transform.position = mousePos;
    }

    private void HoldState()
    {
        switch (currentHandState)
        {
            case HandState.EMPTY:
                interactItemHeld = null; // we set ItemHeld here to null to make sure we clear the variable before attempting to pick up antoher item
                break;

            case HandState.HELD:
                isGrabbable = false; // if an item is held, nothing should be grabbable regardless of if there is a grabbable item returning from trigger check
                break;
        }
    }
    private void ChangeCurrentHandState(HandState handState)
    {
        if (currentHandState != handState)
        {
            currentHandState = handState;
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        interactHovered = null;

        if (currentHandState == HandState.HELD)
        {
            return;
        }
        else
        {
            if (other.CompareTag("Grabbable"))
            {
                isGrabbable = true;
                interactHovered = other.gameObject;
            }
            else
            {
                isGrabbable = false;
                interactHovered = null;
            }
        }
      
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        isGrabbable = false;
        interactHovered = null;
    }






}
