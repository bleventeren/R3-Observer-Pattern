using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;
using R3.Triggers;

public class Player : MonoBehaviour
{
    public event Action<int> OnPlayerDamaged;

    public SerializableReactiveProperty<int> playerScore = new(0);

    public int maxHp = 150;
    
    public ReactiveProperty<int> CurrentHp { get; private set; }
    public ReactiveProperty<bool> IsDead { get; private set; }

    private void Start()
    {
        CurrentHp = new ReactiveProperty<int>(maxHp);
        IsDead = new ReactiveProperty<bool>(false);

        CurrentHp.Subscribe(hp => IsDead.Value = hp <= 0).AddTo(this);
        
        this.OnCollisionEnterAsObservable()
            .Where(collision => collision.gameObject.CompareTag("Reward"))
            .Do(collision => Debug.Log("Collision with reward!"))
            .Select(collision => collision.contacts[0].point)
            .Subscribe(collisionPoint => {
                Debug.Log($"Collision point : {collisionPoint}");
                playerScore.Value += 1;
            }).AddTo(this);
        
        playerScore.Subscribe(score => Debug.Log("Player score: " + score)).AddTo(this);
    }

    public void TakeDamage(int damage)
    {
        OnPlayerDamaged?.Invoke(damage);
        CurrentHp.Value -= damage;
    }
}
