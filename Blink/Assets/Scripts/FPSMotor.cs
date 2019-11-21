using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerController))]

public class FPSMotor : MonoBehaviour
{
    public event Action Land = delegate { };

    [SerializeField] Camera _camera = null;
    [SerializeField] float _cameraAngleLimit = 70f;
    [SerializeField] GroundDetector _groundDetector = null;

    //tracking camera angle to avoid weird conversions
    private float _currentCameraRotationX = 0;

    Rigidbody _rigidbody = null;

    //store temp variables to calculate movement/rotation
    Vector3 _movementThisFrame = Vector3.zero;
    float _turnAmountThisFrame = 0;
    float _lookAmountThisFrame = 0;
    bool _isGrounded = false;

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody>();
        _groundDetector = GetComponentInChildren<GroundDetector>();
    }

    private void OnEnable()
    {
        _groundDetector.GroundDetected += OnGroundDetected;
        _groundDetector.GroundVanished += OnGroundVanished;
    }

    private void OnDisable()
    {
        _groundDetector.GroundDetected -= OnGroundDetected;
        _groundDetector.GroundVanished -= OnGroundVanished;
    }

    private void FixedUpdate()
    {
        ApplyMovement(_movementThisFrame);
        ApplyTurn(_turnAmountThisFrame);
        ApplyLook(_lookAmountThisFrame);
    }
    public void Move(Vector3 requestedMovement)
    {
        //store movement for next FixedUpdate tick
        _movementThisFrame = requestedMovement;
    }

    public void Turn(float turnAmount)
    {
        //store rotation for next FixedUpdate tick
        _turnAmountThisFrame = turnAmount;
    }

    public void Look(float lookAmount)
    {
        //store rotation for next FixedUpdate tick
        _lookAmountThisFrame = lookAmount;
    }

    public void Jump(float jumpForce)
    {
        if (_isGrounded == false)
            return;

        _rigidbody.AddForce(Vector3.up * jumpForce);
        Debug.Log("Motor Jump. Grounded: " + _isGrounded);
    }

    void ApplyMovement(Vector3 moveVector)
    {
        //confirm we have movement, exit early if not
        if (moveVector == Vector3.zero)
            return;
        //move rigidbody
        _rigidbody.MovePosition(_rigidbody.position + moveVector);
        //clear out movement until we get a new request
        _movementThisFrame = Vector3.zero;
    }

    void ApplyTurn(float rotateAmount)
    {
        //confirm we have rotation, exit early if not
        if (rotateAmount == 0)
            return;
        //rotate the body, covnert x,y,z to quaternion
        Quaternion newRotation = Quaternion.Euler(0, rotateAmount, 0);
        _rigidbody.MoveRotation(_rigidbody.rotation * newRotation);
        //clear out turn until new request
        _turnAmountThisFrame = 0;
    }

    void ApplyLook(float lookAmount)
    {
        //confirm we have a look, exit early if not
        if (lookAmount == 0)
            return;
        //calculate and clamp new rotation before apply
        _currentCameraRotationX -= lookAmount;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -_cameraAngleLimit, _cameraAngleLimit);
        _camera.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0, 0);
        //clear out x movement until new request
        _lookAmountThisFrame = 0;
    }

    void OnGroundDetected()
    {
        _isGrounded = true;
        Land?.Invoke();
    }

    void OnGroundVanished()
    {
        _isGrounded = false;
    }
}
