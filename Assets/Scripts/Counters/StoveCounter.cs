using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }


    public enum State{
        Idle,
        Frying,
        Fried,
        Burned,


    }

    [SerializeField] private FryingRecipieSO[] fryingRecipieSOArray;
    [SerializeField] private BurningRecipieSO[] burningRecipieSOArray;
    private State state;
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipieSO fryingRecipieSO;
    private BurningRecipieSO burningRecipieSO;



    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
            switch (state)
        {
            case State.Idle:

                break;

            case State.Frying:

                fryingTimer += Time.deltaTime;


                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipieSO.fryingtimerMax
                    });

                    if (fryingTimer > fryingRecipieSO.fryingtimerMax)
                {
                    
                    // fried
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(fryingRecipieSO.output, this);
                        
                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipieSO = GetBurningRecipieSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this,new OnStateChangedEventArgs
                        {
                            state =state 
                        });
                        
                    }

                    break;

            case State.Fried:

                    burningTimer += Time.deltaTime;


                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipieSO.burningtimerMax
                    });

                    if (burningTimer > burningRecipieSO.burningtimerMax)
                    {

                        // fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipieSO.output, this);
                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }

                    break;

            case State.Burned:

                break;


        }

    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //There is no KitchenObject here
            if (player.HasKitchenObject())

            {
                //Player Has Kitchen Object
                if (HasRecipieWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipieSO = GetFryingRecipieSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    fryingTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipieSO.fryingtimerMax
                    });
                }
            }

            else

            {
                //Player not carrying anything
            }
        }

        else

        {
            // There is a KitchenObject here 
            if (player.HasKitchenObject())

            {
                //Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player is holding plate

                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }

                }
            }

            else

            {
                //Player not carring anything
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });

            }
        }

    }
    private bool HasRecipieWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipieSO fryingRecipieSO = GetFryingRecipieSOWithInput(inputKitchenObjectSO);
        return fryingRecipieSO != null;

    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipieSO fryingRecipieSO = GetFryingRecipieSOWithInput(inputKitchenObjectSO);
        if (fryingRecipieSO != null)
        {
            return fryingRecipieSO.output;
        }
        else
        {
            return null;
        }

    }
    private FryingRecipieSO GetFryingRecipieSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipieSO fryingRecipieSO in fryingRecipieSOArray)
        {
            if (fryingRecipieSO.input == inputKitchenObjectSO)
            {
                return fryingRecipieSO;
            }
        }
        return null;
    }

    private BurningRecipieSO GetBurningRecipieSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipieSO burningRecipieSO in burningRecipieSOArray)
        {
            if (burningRecipieSO.input == inputKitchenObjectSO)
            {
                return burningRecipieSO;
            }
        }
        return null;
    }


    public bool IsFried()
    {
        return state == State.Fried;
    }

}
