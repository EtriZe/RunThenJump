using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterStateMachine : MonoBehaviour
{
    [Header("Character")]
    public Rigidbody rb;
    public Transform cam;
    public GameObject mainObject;
    public GameObject player;
    public CapsuleCollider capsule;

    [Header("UI")]
    public TextMeshProUGUI UIGrounded;
    public TextMeshProUGUI UIheight;

    [Header("Movement")]
    public float WalkSpeed;
    public float SprintSpeed;
    private Vector3 _direction;
    private Vector2 _input;
    private Vector3 _moveDir;
    private float _velocity;
    private float _turnSmoothVelocity;
    private float _turnSmoothTime = 0.1f;
    private float _speed = 5.0f;

    [Header("Jump")]
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _airMultiplier;
    private bool _readyToJump = true;
    private bool _jumping = false;
    private bool _isJumpPressed = false;

    [Header("Crouching")]
    public float CrouchSpeed;
    public float CrouchYScale;
    private float _startYScale;
    private bool _needToStandUp = false;
    
    [Header("Ground check")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private float _groundDrag;
    private bool _grounded;

    [Header("Slope Handling")]
    public float MaxSlopeAngle;
    private RaycastHit _slopeHit;

    [Header("Animation")]
    [SerializeField] private Animator _animator;



    //STATE VARIABLES
    CharacterBaseState _currentState;
    CharacterStateFactory _states;

    //getters and setters
    public CharacterBaseState CurrentState { get { return _currentState; } set {  _currentState = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } set { _isJumpPressed = value; } }
    public float JumpPower { get { return _jumpPower; } }
    public bool Grounded { get { return _grounded; } }
    public Rigidbody Rb { get { return rb; } }
    public GameObject Player { get { return player; } }
    public float GroundDrag { get { return _groundDrag; } set { _groundDrag = value; } }
    public float JumpCooldown { get { return _jumpCooldown; } }
    public bool ReadyToJump { get { return _readyToJump; } set { _readyToJump = value; } }
    public bool Jumping { get { return _jumping; } set { _jumping = value; } }

    // Start is called before the first frame update
    void Start()
    {
        _states = new CharacterStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        
        Cursor.lockState = CursorLockMode.Locked;
        _startYScale = player.transform.localScale.y;

        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateState();
        UIRefresh();
    }
    
    void FixedUpdate()
    {
        SpeedControl();
        ApplyRotation();
        ApplyMovement();
        IsGrounded();
    }

    void UIRefresh()
    {
        UIGrounded.text = _grounded.ToString();
        UIheight.text = (transform.position.y - 0.5).ToString("0.00");
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0, _input.y);
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton()) _speed = SprintSpeed;
        else _speed = WalkSpeed;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVel.magnitude > _speed)
        {
            Vector3 limitedVel = flatVel.normalized * _speed; 
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0)
        {
            _moveDir = Vector3.zero;
            return;
        }

        //Will rotate the character to the direction
        float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

        //Will smooth the rotation of the character
        float angle = Mathf.SmoothDampAngle(mainObject.transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
        mainObject.transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //To move in direction of the camera
        _moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    private void ApplyMovement()
    {
        if(OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * _speed * 500f * Time.deltaTime, ForceMode.Force);
        }
        else if (_grounded)
        {
            rb.AddForce(_moveDir.normalized * _speed * 1000f * Time.deltaTime, ForceMode.Force);
        }
        else
        {
            rb.AddForce(_moveDir.normalized * _speed * 1000f * _airMultiplier * Time.deltaTime, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(player.transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.1f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < MaxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDir, _slopeHit.normal).normalized;
    }


    private void IsGrounded()
    {
        RaycastHit hit;

        /*Boxcast : Center of box
         *          Size of box
         *          Direction
         *          Information "hit"
         *          Rotation
         *          Distance  
         *          Layermask id
        */
        if (Physics.BoxCast(new Vector3(player.transform.position.x, player.transform.position.y + 1f, player.transform.position.z),
                            new Vector3(capsule.radius * 2.0f + 0.2f, 0f, capsule.radius * 2.0f + 0.2f),
                            Vector3.down,
                            out hit,
                            Quaternion.Euler(0, 0, 0),
                            _playerHeight + 0.1f, //Parce que y a des variations de hauteur l�g�re quand on se d�place donc j'ajoute une fen�tre 
                            _whatIsGround.value))
        {
            if(_jumping)
            {
                _grounded = true;
                _jumping = false;
            }
            Rb.drag = _groundDrag;


        }
        else
        {
            _grounded = false;
            Rb.drag = 0;
        }

    }
}
