using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovementScript : MonoBehaviour
{
    #region Private variables

    private float _gravity = -9.81f;
    private float _velocity;
    private float _turnSmoothVelocity;
    private Vector3 _direction;
    private Vector2 _input;
    private Vector3 _moveDir;

    [SerializeField] private float _gravityMultiplier = 5.0f;
    [SerializeField] private float _jumpPower;

    #endregion
    #region Public variables

    public CharacterController Controller;
    public Transform Cam;
    public float Speed = 6.0f;
    public float TurnSmoothTime = 0.1f;

    #endregion


    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        
        ApplyRotation();
        ApplyGravity();
        ApplyMovement();
    }

    
    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGrounded()) return;

            _velocity += _jumpPower;
        Debug.Log("Jump");

    }
    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) return;

        //Will rotate the character to the direction
        float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;

        //Will smooth the rotation of the character
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //To move in direction of the camera
        _moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;        
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * _gravityMultiplier * Time.deltaTime;
        }

        _moveDir.y += _velocity;
    }

    private void ApplyMovement()
    {
        Controller.Move(_moveDir.normalized * Speed * Time.deltaTime);
    }

    private bool IsGrounded() { return Controller.isGrounded; }
}
