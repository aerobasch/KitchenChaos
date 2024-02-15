using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour ,IKitchenObjectParent
{
    
    public static Player Instance 
    { 
        get; private set; 
    }
    public event EventHandler OnPickedSomething;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    //Set Movement speed value and expose it only to the inspector
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    
    [SerializeField] private Transform KitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;



    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.Log("There is more than one Player instance.");
        }

        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }



    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
             
            if (selectedCounter != null && KitchenGameManager.Instance.IsGamePlaying())
            {
                selectedCounter.Interact(this);
            }
        
      
                
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null && KitchenGameManager.Instance.IsGamePlaying())
        {
            selectedCounter.InteractAlternate(this);
        }

    }


    private void Update()
    {
        HandleMovement();
        HandleInteraction();

    }

    public bool IsWalking()
    {
        return isWalking;
    }



    private void HandleInteraction()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        //Create new Variable moveDir and set it as a float 3 with input vector X mapped to X , Y set to 0 and Input vectorY set to Z
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Has ClearCounter 
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                    
                }
            }
            else
            {
                SetSelectedCounter(null);

               
            }
      
        
        }
        else
        {
            SetSelectedCounter(null);
           
        }

        

    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();



        //Create new Variable moveDir and set it as a float 3 with input vector X mapped to X , Y set to 0 and Input vectorY set to Z
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Cannot move towards move dir
            //attempt only on the X movement

            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;

            canMove = (moveDir.x  < -.5f || moveDir.x >+.5f)  && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);


            if (canMove)
            {
                // Can only move on the X
                moveDir = moveDirX;
            }

            else
            {
                //Cannot move only on the X

                //Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;

                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)

                {
                    //can move only on the z

                    moveDir = moveDirZ;

                }
                else
                {
                    //Cannot move in any direction 
                }


            }

        }


        if (canMove)
        {
            //Apply trasnformation based on moveDir multiplied by movespeed and Delta time 
            transform.position += moveDir * moveDistance;
        }



        isWalking = moveDir != Vector3.zero;
        float rotateSpeed = 10f;

        //Apply rotation to character to ensure its looking in the direction its facing. speed set by rotateSpeed
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);



    }
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return KitchenObjectHoldPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this,EventArgs.Empty);
        }

    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

}
