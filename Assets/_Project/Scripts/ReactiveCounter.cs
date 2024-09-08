using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

    DisposableBag d; // Struct, best for performance
    
    private void Start()
    {
        Observable.FromEvent<int>(
            handler => player.OnPlayerDamaged += handler,
            handler => player.OnPlayerDamaged -= handler
        ).Subscribe(
            damage => Debug.Log($"Player took {damage} damage!")
        ).AddTo(ref d);

        player.CurrentHp.Subscribe(x => Debug.Log("Current HP: " + x)).AddTo(ref d);
        //player.CurrentHp.Subscribe(x => Debug.Log("Current HP: " + x)).AddTo(this);
        player.IsDead.Where(isDead => isDead == true).Subscribe(_ => coinButton.interactable = false).AddTo(ref d);
        //player.IsDead.Where(isDead => isDead == true).Subscribe(_ => coinButton.interactable = false).AddTo(this);
        
        subscription = coinButton.onClick.AsObservable(cts.Token).Subscribe(_ =>
        {
            player.TakeDamage(99); //1
            counterText.text = (int.Parse(counterText.text) + 1).ToString();
        });
    }

    private void OnDestroy()
    {
        subscription?.Dispose();
        cts?.Dispose();
    }
}
