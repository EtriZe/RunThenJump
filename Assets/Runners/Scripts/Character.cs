using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum MovementState
{
    walking,
    sprinting,
    crouching,
    air
}

public class Character : MonoBehaviour
{
    [Header("Character")]
    public Rigidbody Rb;
    public Transform Cam;
    public GameObject MainObject;
    public GameObject Player;
    public CapsuleCollider Capsule;

    [Header("UI")]
    public TextMeshProUGUI UIGrounded;
    public TextMeshProUGUI UIheight;

    [Header("Movement")]
    public float WalkSpeed;
    public float SprintSpeed;
    public MovementState State;

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
    public float maxSlopeAngle;
    private RaycastHit slopeHit;


    [Header("Animation")]
    [SerializeField] private Animator _animator;




    private void Start() {
        Rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;

        _startYScale = Player.transform.localScale.y;

    }

    void FixedUpdate() 
    {
        ApplyRotation();
        ApplyMovement();
        IsGrounded();
    }

    void UIRefresh()
    {
        UIGrounded.text = _grounded.ToString();
        UIheight.text = (transform.position.y - 0.5).ToString("0.00");
    }

    // Update is called once per frame
    void Update()
    {
        
        UIRefresh();
        SpeedControl();
        if(_needToStandUp) CanStandUp();
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
        if (Physics.BoxCast(new Vector3(Player.transform.position.x, Player.transform.position.y + 1f, Player.transform.position.z),
                            new Vector3(Capsule.radius * 2.0f, 0f, Capsule.radius * 2.0f),
                            Vector3.down,
                            out hit,
                            Quaternion.Euler(0, 0, 0),
                            _playerHeight + 0.1f, //Parce que y a des variations de hauteur légère quand on se déplace donc j'ajoute une fenêtre 
                            _whatIsGround.value))
        {
            _grounded = true;
            Rb.drag = _groundDrag;
        }
        else
        {
            _grounded = false;
            Rb.drag = 0;
        }
    }



    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0, _input.y);
    }
    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            _speed = SprintSpeed;
        }
        else
        { 
            _speed = WalkSpeed;
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
        float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;

        //Will smooth the rotation of the character
        float angle = Mathf.SmoothDampAngle(MainObject.transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
        MainObject.transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //To move in direction of the camera
        _moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }
    private void ApplyMovement()
    {
        if(OnSlope())
        {
            Rb.AddForce(GetSlopeMoveDirection() * _speed * 5f, ForceMode.Force);
        }
        else if (_grounded)
        {
            Rb.AddForce(_moveDir.normalized * _speed * 10f, ForceMode.Force);
        }
        else
        {
            Rb.AddForce(_moveDir.normalized * _speed * 10f * _airMultiplier, ForceMode.Force);
        }

        Rb.useGravity = !OnSlope();
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton() && _readyToJump && _grounded)
        {
             
            Rb.velocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);
            Rb.AddForce(Player.transform.up * _jumpPower, ForceMode.Impulse);

            Invoke(nameof(ResetJump), _jumpCooldown);
            StartCoroutine(GettingOutOfFloor());
        }
    }
    IEnumerator GettingOutOfFloor()
    {
        yield return new WaitForSeconds(0.1f);
        _jumping = true;
    }
    private void ResetJump()
    {
        _readyToJump = true;
    }


    public void Crouching(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            _speed = CrouchSpeed;
            _needToStandUp = false;

            Player.transform.localScale = new Vector3(Player.transform.localScale.x, CrouchYScale, Player.transform.localScale.z);
            Rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        else
        {
            CanStandUp();
        }
    }

    private void CanStandUp()
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
        if (Physics.BoxCast(new Vector3(Player.transform.position.x, Player.transform.position.y + 1f, Player.transform.position.z),
                            new Vector3(Capsule.radius * 2.0f, 0f, Capsule.radius * 2.0f),
                            Vector3.up,
                            out hit,
                            Quaternion.Euler(0, 0, 0),
                            _playerHeight,
                            _whatIsGround.value))
        {
            _needToStandUp = true;
        }
        else
        {
            // Can stand up
            StandUp();
        }
    }
    private void StandUp()
    {
        Player.transform.localScale = new Vector3(Player.transform.localScale.x, _startYScale, Player.transform.localScale.z);
        _needToStandUp = false;
        _speed = WalkSpeed;
    }

    private void SpeedControl(){
        Vector3 flatVel = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);

        if(flatVel.magnitude > _speed)
        {
            Vector3 limitedVel = flatVel.normalized * _speed; 
            Rb.velocity = new Vector3(limitedVel.x, Rb.velocity.y, limitedVel.z);
        }
    }


    private bool OnSlope()
    {
        if(Physics.Raycast(Player.transform.position, Vector3.down, out slopeHit, _playerHeight * 0.5f + 0.1f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDir, slopeHit.normal).normalized;
    }


}
