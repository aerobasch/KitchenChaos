using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter :  BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;


    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if (KitchenGameManager.Instance.IsGamePlaying() && spawnPlateTimer > spawnPlateTimerMax && platesSpawnedAmount < platesSpawnedAmountMax) 
        {
            spawnPlateTimer = 0f;
            platesSpawnedAmount++;
            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            

        }
    }





   public override void Interact(Player player)
 {
     if (!player.HasKitchenObject() && platesSpawnedAmount >0 )
     {
         KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
            platesSpawnedAmount--;

            
         OnPlateRemoved?.Invoke(this, EventArgs.Empty);

    }

  }




}
