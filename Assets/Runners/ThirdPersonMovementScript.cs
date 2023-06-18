using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovementScript : MonoBehaviour
{
    #region Private variables

    [Header("Movement")]
    private float _velocity;
    private float _turnSmoothVelocity;
    private float _turnSmoothTime = 0.1f;
    private Vector3 _direction;
    private Vector2 _input;
    private Vector3 _moveDir;

    [Header("Jump")]
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _airMultiplier;
    private bool _readyToJump = true;

    [Header("Ground check")]
    [SerializeField] private float PlayerHeight;
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private float GroundDrag;
    private bool Grounded;

    #endregion
    #region Public variables

    public Rigidbody Rb;
    public Transform Cam;
    public float Speed;

    #endregion

    private void Start() {
        Rb.freezeRotation = true;
    }

    void FixedUpdate() {
        ApplyMovement();
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        //Ground check
        Grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, WhatIsGround);

        if(Grounded) Rb.drag = GroundDrag;
        else Rb.drag = 0;

        SpeedControl();
        ApplyRotation();
    }

    
    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        
        if(_readyToJump && Grounded)
        {
            Rb.velocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);
            Rb.AddForce(transform.up * _jumpPower, ForceMode.Impulse);

            Invoke(nameof(ResetJump), _jumpCooldown);
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
    
    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) 
        {
            _moveDir = Vector3.zero;
            return;
        }

        //Will rotate the character to the direction
        float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;

        //Will smooth the rotation of the character
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //To move in direction of the camera
        _moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;        
    }

    private void ApplyMovement()
    {
        //On ground
        if(Grounded)
            Rb.AddForce(_moveDir.normalized * Speed * 10f, ForceMode.Force);
        else
            Rb.AddForce(_moveDir.normalized * Speed * 10f * _airMultiplier, ForceMode.Force);
    }

    private void SpeedControl(){
        Vector3 flatVel = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);

        if(flatVel.magnitude > Speed)
        {
            Vector3 limitedVel = flatVel.normalized * Speed; 
            Rb.velocity = new Vector3(limitedVel.x, Rb.velocity.y, limitedVel.z);
        }
    }

}
