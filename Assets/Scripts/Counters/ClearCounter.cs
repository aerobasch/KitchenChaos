using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ClearCounter : BaseCounter
{

    [SerializeField]  private KitchenObjectSO kitchenObjectSO;
 
  




   
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //There is no KitchenObject here
            if (player.HasKitchenObject())
            
            {
                //Player Has Kitchen Object
                player.GetKitchenObject().SetKitchenObjectParent(this);
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
                else
                {
                    //player is not carryign Plate but something else

                    if (GetKitchenObject().TryGetPlate(out  plateKitchenObject))
                    {
                        //counter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())){
                            player.GetKitchenObject().DestroySelf();
                        }

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
    

}
