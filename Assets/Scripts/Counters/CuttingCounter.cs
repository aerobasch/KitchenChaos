using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{

    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public  event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;


    [SerializeField] private CuttingRecipieSO[] cuttingRecipieSOArray;

    private int cuttingProgress;



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
                    cuttingProgress = 0;

                    CuttingRecipieSO cuttingRecipieSO = GetCuttingRecipieSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    OnProgressChanged.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingRecipieSO.cuttingProgressMax
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
                    }

                }

            }

            else

            {
                //Player not carring anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }


    }
    private bool HasRecipieWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipieSO cuttingRecipieSO = GetCuttingRecipieSOWithInput(inputKitchenObjectSO);
        return cuttingRecipieSO != null;

    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject()&& HasRecipieWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //there is a kitchen object here and it can be cut
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            CuttingRecipieSO cuttingRecipieSO = GetCuttingRecipieSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipieSO.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRecipieSO.cuttingProgressMax)
            {

                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                //Their is something on this counter cut it 
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
                
            }

        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipieSO cuttingRecipieSO = GetCuttingRecipieSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipieSO != null)
        {
            return cuttingRecipieSO.output;
        }
        else
        {
            return null;
        }
                
    }
    private CuttingRecipieSO GetCuttingRecipieSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipieSO cuttingRecipieSO in cuttingRecipieSOArray)
        {
            if (cuttingRecipieSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipieSO;
            }
        }
        return null;
    }
}
