using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class ReactiveCounter : MonoBehaviour
{
    public Text counterText;
    public Button coinButton;
    public Button cancelButton;

    public Player player;
    
    IDisposable subscription;
    CancellationTokenSource cts;
    
    IDisposable dispossable;

    private void Awake()
    {
        counterText.text = "0";
        cts = new CancellationTokenSource();
        cancelButton.onClick.AddListener(() => cts.Cancel());
    }

    private void Start()
    {
        var d1 = Observable.FromEvent<int>(
            handler => player.OnPlayerDamaged += handler,
            handler => player.OnPlayerDamaged -= handler
        ).Subscribe(
            damage => Debug.Log($"Player took {damage} damage!")
        );

        var d2 = player.CurrentHp.Subscribe(x => Debug.Log("Current HP: " + x));
        //player.CurrentHp.Subscribe(x => Debug.Log("Current HP: " + x)).AddTo(this);
        var d3 = player.IsDead.Where(isDead => isDead == true).Subscribe(_ => coinButton.interactable = false);
        //player.IsDead.Where(isDead => isDead == true).Subscribe(_ => coinButton.interactable = false).AddTo(this);
        
        subscription = coinButton.onClick.AsObservable(cts.Token).Subscribe(_ =>
        {
            player.TakeDamage(99); //1
            counterText.text = (int.Parse(counterText.text) + 1).ToString();
        });
        
        dispossable = Disposable.Combine(d1,d2,d2,subscription);
    }

    private void OnDestroy()
    {
        subscription?.Dispose();
        cts?.Dispose();
    }
}
