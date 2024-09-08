using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using R3.Triggers;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void Start()
    {
        this.OnCollisionEnterAsObservable()
            .Subscribe(_ => {
                    //play effect
                    Destroy(gameObject);
                })
            .AddTo(this);
    }
}
