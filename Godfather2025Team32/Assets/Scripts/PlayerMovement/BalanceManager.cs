using System;
using System.Collections.Generic;
using UnityEngine;

public class BalanceManager : MonoBehaviour
{
    [Header("Settings")]
    public float stepPerInput = 0.25f;
    public float maxImbalance = 1f;
    public float smoothTime = 0.15f;

    [Header("Optional - auto subscribe")]
    public EventManager eventManager;

    private float _target = 0f;
    private float _current = 0f;
    private float _vel = 0f;

    public event Action<float> OnBalanceChanged;

    void Awake()
    {
        _target = 0f;
        _current = 0f;
        _vel = 0f;
    }

    private void OnDestroy()
    {
        eventManager.OnMoveLeft -= OnCorrectLeft;
        eventManager.OnMoveRight -= OnCorrectRight;
    }

    private void Start()
    {
        eventManager.OnMoveLeft += OnCorrectLeft;
        eventManager.OnMoveRight += OnCorrectRight;
    }

    void Update()
    {
        _current = Mathf.SmoothDamp(_current, _target, ref _vel, smoothTime, Mathf.Infinity, Time.deltaTime);
        _current = Mathf.Clamp(_current, -maxImbalance, maxImbalance);

        OnBalanceChanged?.Invoke(_current);
    }

    public float GetImbalance() => _current;
    public float GetImbalance(Team team) => _current;

    public void OnCorrectLeft(Team team)  => AddToTarget(-stepPerInput);
    public void OnCorrectRight(Team team) => AddToTarget(stepPerInput);
    
    private void AddToTarget(float delta)
    {
        _target = Mathf.Clamp(_target + delta, -maxImbalance, maxImbalance);
    }

    public void SetTargetImbalance(float value) => _target = Mathf.Clamp(value, -maxImbalance, maxImbalance);

    public void ResetImbalance()
    {
        _target = 0f;
        _current = 0f;
        _vel = 0f;
        OnBalanceChanged?.Invoke(0f);
    }
}
