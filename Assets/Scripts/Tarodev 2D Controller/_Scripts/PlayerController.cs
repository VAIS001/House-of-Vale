using System;
using System.Collections;
using UnityEngine;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// I have a premium version on Patreon, which has every feature you'd expect from a polished controller. Link: https://www.patreon.com/tarodev
    /// You can play and compete for best times here: https://tarodev.itch.io/extended-ultimate-2d-controller
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/tarodev
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        private  float dragMagnitude = 0f;
        //dash 
        private float dashMultiplier = 2f;
        private float dashTimer = 0.5f;
        private const float dashCoolDownTime=10f;
        private float dashCoolDownTimer = 0f;
        private bool isDashing = false;

        //momentum 
        private float speedBoost=0f;
        private bool hasMomentum = false;
        private const float MOMENTUM_LOSS_PER_SEC = 0.2f;//actually speed lost while in momentum

        //crouch - drag
        private bool isCrouching = false;
        private float originalSpeed;
        private float originalAcceleration;
        private float originalJumpPower;

        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            if(!isDashing){
                if(dashCoolDownTimer>0f)dashCoolDownTimer-=Time.deltaTime;

                if(hasMomentum){
                    //lose momentum gradually
                    LoseSpeed(MOMENTUM_LOSS_PER_SEC * Time.deltaTime);
                }
            }
            
            //GatherInput();
        }

        //converted from original (private) method name: GatherInput
        public void ProcessInput(FrameInput frameInput)
        {
            /*
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };
            */
            this._frameInput = frameInput;

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
            //Debug.Log("Frame input: x: "+_frameInput.Move.x +" y: "+_frameInput.Move.y);
        }

        #region Drag 
        //just added
        public void Drag(float magnitude){
            dragMagnitude += magnitude;

            _stats.FallAcceleration += magnitude;
            _stats.MaxSpeed -= magnitude;
            _stats.Acceleration -= magnitude;
            _stats.GroundDeceleration += magnitude;
            _stats.AirDeceleration += magnitude;
            _stats.JumpPower -= magnitude;
        }

        public void EndDrag(float magnitude){

            _stats.FallAcceleration -= magnitude;
            _stats.MaxSpeed += magnitude;
            _stats.Acceleration += magnitude;
            _stats.GroundDeceleration -= magnitude;
            _stats.AirDeceleration -= magnitude;
            _stats.JumpPower += magnitude;

            dragMagnitude -= magnitude;
        }

        #endregion

        #region Dash 

        public void StartDash(){
            if(isCrouching)return;
            if(dashCoolDownTimer>0f)return;
            StartCoroutine(DashCoroutine()); //can  not be terminated midway
            dashCoolDownTimer = dashCoolDownTime;
        }

        private IEnumerator DashCoroutine(){

            isDashing = true;
            //add force
            _rb.AddForce(_frameVelocity*dashMultiplier, ForceMode2D.Impulse);
            yield return new WaitForSeconds(dashTimer);
            //remove force
            _rb.velocity = Vector2.zero;
            isDashing = false;

        }

        #endregion

        #region Crouch

        public void Crouch(){
            isCrouching = true;

            originalSpeed = _stats.MaxSpeed;
            originalAcceleration = _stats.Acceleration;
            originalJumpPower = _stats.JumpPower;

            _stats.MaxSpeed /= 3f;
            _stats.Acceleration /= 3f;
            _stats.JumpPower /= 3f;

        }

        public void UnCrouch () {
            isCrouching = false;

            _stats.MaxSpeed = originalSpeed;
            _stats.Acceleration = originalAcceleration;
            _stats.JumpPower = originalJumpPower;

        }

        #endregion

        #region Momentum

        public void SpeedBoost(float boost){
            speedBoost+=boost;
            _stats.MaxSpeed += boost;
            hasMomentum = true;
        }

        public void LoseSpeed(float speedLoss){
            if(speedLoss>=speedBoost){
                NormalSpeed();
                return;
            }
            speedBoost-=speedLoss;
            _stats.MaxSpeed -= speedLoss;
        }

        public void NormalSpeed(){
            _stats.MaxSpeed-=speedBoost;
            speedBoost = 0f;
            hasMomentum=false;
        }

        #endregion

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();
            
            ApplyMovement();
            //Debug.Log("Frame input: x: "+_frameInput.Move.x +" y: "+_frameInput.Move.y);

        }

        #region Collisions
        
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion


        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }

            //Debug.Log("frame velocity: x: "+_frameVelocity.x +" y: "+_frameVelocity.y);
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}