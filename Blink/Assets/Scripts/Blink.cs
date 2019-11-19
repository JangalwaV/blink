using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]

public class Blink : MonoBehaviour
{
    InputManager _input = null;
    Rigidbody _rb = null;

    [SerializeField] float _blinkStrength = 100f;

    void Awake() {
        _input = GetComponent<InputManager>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _input.BlinkInput += BlinkForce;
    }

    private void OnDisable()
    {
        _input.BlinkInput -= BlinkForce;
    }

    void BlinkForce()
    {
        _rb.AddForce(Vector3.forward * _blinkStrength, ForceMode.Force);
    }

}
