using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{
    private const string Is_Flashing = "IsFlashing";

    [SerializeField] private StoveCounter stoveCounter;

    private Animator animator;

    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Start()
    {
        stoveCounter.OnProgressChanged += Stovecounter_OnProgressChanged;
        animator.SetBool(Is_Flashing, false);
    }

    private void Stovecounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = .5f;
        bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;
        animator.SetBool(Is_Flashing, show);

    }


  


}
