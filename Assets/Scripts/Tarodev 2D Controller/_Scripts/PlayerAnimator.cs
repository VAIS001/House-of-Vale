using System.Collections;
using UnityEngine;

namespace TarodevController
{
    /// <summary>
    /// VERY primitive animator example.
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("References")] //[SerializeField]
        private Animator anim;
        [SerializeField] private IPlayerController _player;

        [SerializeField] private SpriteRenderer _sprite;

        [Header("Settings")] [SerializeField, Range(1f, 3f)]
        private float _maxIdleSpeed = 2;

        [SerializeField] private float _maxTilt = 5;
        [SerializeField] private float _tiltSpeed = 20;

        [Header("Particles")] [SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private ParticleSystem _launchParticles;
        [SerializeField] private ParticleSystem _moveParticles;
        [SerializeField] private ParticleSystem _landParticles;

        [Header("Audio Clips")] [SerializeField]
        private AudioClip[] _footsteps;

        private AudioSource _source;
        
        private bool _grounded;
        private ParticleSystem.MinMaxGradient _currentGradient;

        //animation stuff
        private float transitionTime = 0.35f;
        private AnimationType currentAnimationState;
        private Coroutine blockCoroutine;
        private Coroutine speedChangeCoroutine;
        private bool isBlocking;

        private void Awake()
        {
           // _source = GetComponent<AudioSource>();
            _player = GetComponentInParent<IPlayerController>();
        }

        private void Start () {
            _source = GetComponent<AudioSource>();
            //_player = GetComponentInParent<IPlayerController>();
            anim = GetComponent<Animator>();
            GameEvents.GetInstance().OnBlockChanged += BlockChanged;
        }

        private void OnDestroy() {
            GameEvents.GetInstance().OnBlockChanged -= BlockChanged;
        }


        private void OnEnable()
        {
            
            //_player.Jumped += OnJumped;
            //_player.GroundedChanged += OnGroundedChanged;
            


            //_moveParticles.Play();
        }

        private void OnDisable()
        {
            
            //_player.Jumped -= OnJumped;
            //_player.GroundedChanged -= OnGroundedChanged;

            //_moveParticles.Stop();
        }

        private void Update()
        {
            if (_player == null) return;

            DetectGroundColor();

            HandleSpriteFlip();

            HandleIdleSpeed();

            HandleCharacterTilt();
        }

        private void HandleSpriteFlip()
        {
            if (_player.FrameInput.x != 0) _sprite.flipX = _player.FrameInput.x > 0;
        }

        private void HandleIdleSpeed()
        {
            anim.SetFloat(IdleSpeedKey, GameStats.GetInstance().speed );
            //_moveParticles.transform.localScale = Vector3.MoveTowards(_moveParticles.transform.localScale, Vector3.one * inputStrength, 2 * Time.deltaTime);
        }

        private void HandleCharacterTilt()
        {
            var runningTilt = _grounded ? Quaternion.Euler(0, 0, _maxTilt * _player.FrameInput.x) : Quaternion.identity;
            anim.transform.up = Vector3.RotateTowards(anim.transform.up, runningTilt * Vector2.up, _tiltSpeed * Time.deltaTime, 0f);
        }

        //TODO add to new event
        private void OnJumped()
        {
            //anim.SetTrigger(JumpKey);
            //anim.ResetTrigger(GroundedKey);
            PlayAnimation(AnimationType.Jump);


            if (_grounded) // Avoid coyote
            {
                //SetColor(_jumpParticles);
                //SetColor(_launchParticles);
                //_jumpParticles.Play();
            }
        }

        //TODO Add to new event
        private void OnGroundedChanged(bool grounded, float impact)
        {
            _grounded = grounded;
            
            if (grounded)
            {
                DetectGroundColor();
                //SetColor(_landParticles);

                //anim.SetTrigger(GroundedKey);
                PlayAnimation(AnimationType.Land);
                //_source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
                //_moveParticles.Play();

                //_landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
                //_landParticles.Play();
            }
            else
            {
               // _moveParticles.Stop();
            }
        }

        private void DetectGroundColor()
        {
            var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

            if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) return;
            var color = r.color;
            _currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
            //SetColor(_moveParticles);
        }

        private void SetColor(ParticleSystem ps)
        {
            var main = ps.main;
            main.startColor = _currentGradient;
        }

        // use this for null check?
        private void PlayParticle(ParticleSystem ps, bool play=true){
            if (ps == null)return;
            if (play)
            {
                ps.Play();
            } else
            {
                ps.Stop();
            }

        }

        private void BlockChanged(bool blocking){
            isBlocking = blocking;
        }

        public void PlayWeaponAnimation(Animator weaponAnim, AnimationType animationState){
            //
        }

        //TODO Restore animation system to unity animator
        public void PlayAnimation(AnimationType animationType){
            //_anim.ResetTrigger();
            if (currentAnimationState == animationType)
            {
                return;
            }
            OnAnimationEnd(); //resets all coroutines
            switch (animationType) {
                case AnimationType.Idle:
                    
                    break;
                case AnimationType.Walk:
                    
                    break;
                case AnimationType.Run:
                    break;
                case AnimationType.Jump:
                    Play(JumpKey);
                    break;
                case AnimationType.SwitchWeapon:
                    Play(SwitchWeaponKey);
                    break;
                case AnimationType.Land:
                    Play(LandKey);
                    break;
                case AnimationType.AttackUp:
                    Play(AttackUpKey);
                    break;
                case AnimationType.AttackMid:
                    Play(AttackMidKey);
                    break;
                case AnimationType.AttackDown:
                    Play(AttackDownKey);
                    break;
                case AnimationType.HeavyAttackUp:
                    Play(HeavyAttackUpKey);
                    break;
                case AnimationType.HeavyAttackDown:
                    Play(HeavyAttackDownKey);
                    break;
                case AnimationType.HeavyAttackMid:
                    Play(HeavyAttackMidKey);
                    break;
                case AnimationType.BlockUp:
                    Play(BlockUpKey);
                    break;
                case AnimationType.BlockDown:
                    Play(BlockDownKey);
                    break;
                case AnimationType.BlockMid:
                    Play(BlockMidKey);
                    break;
                case AnimationType.DialogueAccuse:
                    Play(DialogueAccuseKey);
                    break;
                default :
                    
                    break;
            }
        }

        private void Play(int animKey){
            anim.CrossFade(animKey, transitionTime);
        }

        #region AnimationEvents
        
        public void ChangeAnimationSpeed (float speedFactor, float duration) {
            ResetCoroutine(speedChangeCoroutine);
            speedChangeCoroutine = StartCoroutine(ChangeSpeed(speedFactor, duration));
        }

        public void OnAnimationEnd () {
            //reset all coroutines
            ResetCoroutine(blockCoroutine);
            ResetCoroutine(speedChangeCoroutine);
        }

        public void FreezeOnBlock(){
            ResetCoroutine(blockCoroutine);
            blockCoroutine = StartCoroutine(FreezeOnCondition(currentAnimationState, isBlocking));
        }

        private void ResetCoroutine(Coroutine coroutine){
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = null;
        }

        private IEnumerator FreezeOnCondition(AnimationType animationState, bool condition){
            while(currentAnimationState == animationState && condition) {
                anim.speed = 0f;
                yield return null;    
            }
            anim.speed = 1.0f;
        }

        private IEnumerator ChangeSpeed (float speedFactor, float duration){
            anim.speed = speedFactor;

            yield return new WaitForSeconds(duration);

            anim.speed = 1.0f;
        }


        #endregion
    

        private static readonly int GroundedKey = Animator.StringToHash("Base Layer.Grounded");
        private static readonly int IdleSpeedKey = Animator.StringToHash("Base Layer.Speed");
        private static readonly int WalkKey = Animator.StringToHash("Base Layer.Walk");
        private static readonly int JumpKey = Animator.StringToHash("Base Layer.Jump");
        private static readonly int LandKey = Animator.StringToHash("Base Layer.Land");// TODO create landing animation
        private static readonly int AttackUpKey = Animator.StringToHash("Base Layer.AttackUp");
        private static readonly int AttackMidKey = Animator.StringToHash("Base Layer.AttackMid");
        private static readonly int AttackDownKey = Animator.StringToHash("Base Layer.AttackDown");
        private static readonly int HeavyAttackUpKey = Animator.StringToHash("Base Layer.HeavyAttackUp");
        private static readonly int HeavyAttackMidKey = Animator.StringToHash("Base Layer.HeavyAttackMid");
        private static readonly int HeavyAttackDownKey = Animator.StringToHash("Base Layer.HeavyAttackDown");
        private static readonly int BlockUpKey = Animator.StringToHash("Base Layer.BlockUp");
        private static readonly int BlockMidKey = Animator.StringToHash("Base Layer.BlockMid");
        private static readonly int BlockDownKey = Animator.StringToHash("Base Layer.BlockDown");
        private static readonly int SwitchWeaponKey = Animator.StringToHash("Base Layer.SwitchWeapon");
        private static readonly int DialogueAccuseKey = Animator.StringToHash("Base Layer.DialogueAccuse");
        private static readonly int DialogueConverseKey = Animator.StringToHash("Base Layer.DialogueConverse");

        //sword anim
        private static readonly int SwingClockwiseKey = Animator.StringToHash("SwingClockwise");
        private static readonly int SwingAntiClockwiseKey = Animator.StringToHash("SwingAntiClockwise");

    }

    public enum AnimationType{
        Idle,
        Walk,
        Run,
        Jump,
        Land,
        AttackUp,
        AttackMid,
        AttackDown,
        BlockUp,
        BlockMid,
        BlockDown,
        HeavyAttackUp,
        HeavyAttackMid,
        HeavyAttackDown,
        SwitchWeapon,
        DialogueAccuse,
        DialogueConverse
    }
}