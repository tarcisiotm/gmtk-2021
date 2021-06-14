using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TG.Core;
using TMPro;
using TG.GameJamTemplate;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;

    public float runSpeed = 20f;
    public float flySpeed = 20f;

    public float baseSpeed = 20f;
    public float boostRunSpeed = 20f;
    public float wingsNegativeMultiplier = .75f;

    private float runModifier = 0f;
    private float runBoostModifier = 0f;

    public float speed;

    public float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    private Animator _anim;

    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private float _jumpSoundVolume = .5f;
    [Space]
    [SerializeField] Animator _bodyAnim;
    [SerializeField] Animator _legsAnim;
    [SerializeField] Animator _wingsAnim;
    [Space]
    [SerializeField] Transform _bodyGroundCheck;
    [SerializeField] Transform _legsGroundCheck;

    [SerializeField] TMP_Text _timeText;

    // Constants 
    protected const string ANIM_IS_JUMPING = "isJumping";
    protected const string ANIM_SPEED = "Speed";

    public bool _isMoving = false;
    private bool _wasMoving = false;

    private Player _player;

    public delegate void PressedUp();
    public event PressedUp OnPressedUpCallback;

    private bool _canMove = true;
    public bool CanMove => _canMove;

    private bool _canJump = false;
    public bool CanJump => _canJump;

    private bool _canFly = false;
    public bool CanFly => _canFly;

    public bool _flyUsed = false;
    public bool _isFlying = false;

    private const float MAX_FLY_TIME = 2;
    private const float BOOST_FLY_TIME = 2;
    private float _flyTime = 0f;
    private float _flyTimeModifier = 0f;

    bool legsBoostActive = false;
    bool wingsBoostActive = false;

    #region Gameplay Movement

    #endregion Gameplay Movement

    private void Awake()
    {
        _player = GetComponent<Player>();

        _anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {

    }

    private void OnCanceled()//InputAction.CallbackContext value)
    {
        _isMoving = false;
        horizontalMove = 0;
        _anim.SetFloat(ANIM_SPEED, horizontalMove);
    }

    private void Jump()
    {
        if (!_canMove || !_canJump) return;
        var spaceBarDown = Input.GetKeyDown(KeyCode.Space);
        if (!jump)
        {
            if (!jump && spaceBarDown && controller.m_Grounded) AudioManager.I.CreateOneShot(_jumpSound, transform.position, _jumpSoundVolume);
            jump = spaceBarDown;
        }
        //_anim.SetBool(ANIM_IS_JUMPING, true);

    }

    private void ProcessFlight()
    {
        if (!_canMove || !_canFly) return;

        var spaceBarDown = _isFlying ? Input.GetKey(KeyCode.Space) : Input.GetKeyDown(KeyCode.Space);
        spaceBarDown = Input.GetKey(KeyCode.Space);

        if (!_isFlying && !_flyUsed && spaceBarDown)
        {
            _flyTime = controller.IsGrounded ? MAX_FLY_TIME + _flyTimeModifier : _flyTime; // calculate fly time
            if (_flyTime > 0) _wingsAnim.SetBool("IsFlying", true);
        }

        _isFlying = spaceBarDown;
    }

    private void SetText(string text)
    {
        _timeText.text = text;
    }

    // From CharacterController2D OnLandEvent
    public void OnLanding()
    {
        //Debug.LogError("On landing: " + _isFlying);
        //_anim.SetBool(ANIM_IS_JUMPING, false);
        _isFlying = false;
        _flyUsed = false;
        _wingsAnim.SetBool("IsFlying", false);
        SetText("");

    }

    private void OnPressedUp()//InputAction.CallbackContext value)
    {
        if (!_canMove) return;

        OnPressedUpCallback?.Invoke();
    }

    private void FixedUpdate()
    {
        //if (!_isMoving && !jump) return;
        speed = baseSpeed + runModifier;

        if (_canJump && legsBoostActive) speed += runBoostModifier;

        if (_canFly && controller.m_Grounded) speed *= wingsNegativeMultiplier;
        else if (_canFly && !controller.m_Grounded) speed = baseSpeed + flySpeed;

        // Move our character
        var horMove = _isMoving ? horizontalMove * Time.fixedDeltaTime * speed : 0;
        controller.Move(horMove, crouch, jump, _isFlying && !_flyUsed);
        jump = false;
    }

    private void Update()
    {
        if (!_canMove)
        {
            _isMoving = false;
            return;
        }
        _wasMoving = _isMoving;

        horizontalMove = Input.GetAxisRaw("Horizontal");

        if (!_isMoving && Mathf.Abs(horizontalMove) > float.Epsilon)
        {
            _isMoving = true;
            _bodyAnim.speed = 1f;

            _legsAnim.speed = 1f;
        }
        else if (_wasMoving && !_isMoving)
        {
            _bodyAnim.speed = .25f;

            _legsAnim.speed = .25f;
        }

        Jump();
        ProcessFlight();

        if (_isFlying)
        {
            var time = Mathf.CeilToInt(_flyTime);
            var text = "<color=yellow>" + time + "</color>";
            if (time <= 1) text = "<color=red>" + time + "</color>";

            SetText(text);

            _flyTime -= Time.deltaTime;
            if (_flyTime <= 0) _flyUsed = true;
        }

        // _anim.SetFloat(ANIM_SPEED, Mathf.Abs(horizontalMove));
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }

    public void SetCanJump(bool canJump)
    {
        _canJump = canJump;

        _legsAnim.gameObject.SetActive(canJump);
        if (canJump)
        {
            _legsAnim.enabled = true;
            transform.position += new Vector3(0, .2f, 0);
        }

        if (runModifier > runSpeed)
        {
            // deactivate run str
        }

        runModifier = canJump ? runSpeed : 0;
        //runSpeed = canJump ? baseRunSpeed + runSpeed : baseRunSpeed - runSpeed;

        controller.m_GroundCheck = canJump ? _legsGroundCheck : _bodyGroundCheck;
    }

    public void SetCanFly(bool canFly)
    {
        _canFly = canFly;
        _wingsAnim.gameObject.SetActive(canFly);
        if (canFly)
        {
            _wingsAnim.enabled = true;
        }
    }

    public void Fly()
    {
        //_isFlying = true;
    }

    public void SetLegsStrength(bool active)
    {
        runBoostModifier = active ? boostRunSpeed : 0;

        legsBoostActive = active;
    }

    public void SetWingsStrength(bool active)
    {
        _flyTimeModifier = active ? BOOST_FLY_TIME : 0;

        wingsBoostActive = active;
    }
}