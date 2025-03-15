using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TarodevController;

public class PlayerInputController : MonoBehaviour {
    
    public InputActionReference moveAction; //returns Vector2
    public InputActionReference lightAttackAction;
    public InputActionReference heavyAttackAction;
    public InputActionReference blockAction; //TODO change from tap to hold
    public InputActionReference tapJumpAction;
    public InputActionReference holdJumpAction;

    private PlayerController playerController;
    private CombatController combatController;

    private FrameInput _frameInput;

    bool jumpHeld;
    bool jumpDown;

    private void Start() {
        
        playerController = GetComponent<PlayerController>();
        combatController = GetComponent<CombatController>();

        _frameInput = new FrameInput
            {
                JumpDown = false,
                JumpHeld = false,
                Move = Vector2.zero
            };

        GameEvents.GetInstance().OnWeaponEquipped += AddWeaponDrag;
        GameEvents.GetInstance().OnWeaponUnEquipped += RemoveWeaponDrag;
    }

    private void OnDestroy() {
        GameEvents.GetInstance().OnWeaponEquipped -= AddWeaponDrag;
        GameEvents.GetInstance().OnWeaponUnEquipped -= RemoveWeaponDrag;
    }

    private void AddWeaponDrag(float drag){
        Debug.Log("add weapon drag: "+drag);
        playerController.Drag(drag);
    }

    private void RemoveWeaponDrag(float drag){
        Debug.Log("Remove weapon drag: "+drag);
        playerController.EndDrag(drag);
    }

    private void Update() {
        
        /*_frameInput = new FrameInput
            {
                JumpDown = jumpDown,
                JumpHeld = jumpHeld,
                Move = moveAction.action.ReadValue<Vector2>()
            };
        */
        _frameInput.JumpDown = jumpDown;
        _frameInput.JumpHeld = jumpHeld;
        _frameInput.Move = moveAction.action.ReadValue<Vector2>();
        playerController.ProcessInput(_frameInput);
        //Debug.Log("move x: "+_frameInput.Move.x +" move y: "+_frameInput.Move.y+ " jump down: " +_frameInput.JumpDown +" jump held: "+_frameInput.JumpHeld);
    }

    private void OnEnable() {
        lightAttackAction.action.started += CallLightAttack;
        heavyAttackAction.action.started += CallHeavyAttack;
        tapJumpAction.action.started += CallTapJump;
        holdJumpAction.action.started += CallHoldJump;
        blockAction.action.started += CallBlock;
        tapJumpAction.action.canceled += StopTapJump;
        holdJumpAction.action.canceled += StopHoldJump;
        blockAction.action.canceled += StopBlock;

        
    }

    private void OnDisable() {
        lightAttackAction.action.started -= CallLightAttack;
        heavyAttackAction.action.started -= CallHeavyAttack;
        tapJumpAction.action.started -= CallTapJump;
        holdJumpAction.action.started -= CallHoldJump;
        blockAction.action.started -= CallBlock;
        tapJumpAction.action.canceled -= StopTapJump;
        holdJumpAction.action.canceled -= StopHoldJump;
        blockAction.action.canceled -= StopBlock;

        
    }

    private void StopTapJump (InputAction.CallbackContext context) {
        
        jumpDown=false;
    }

    private void StopHoldJump (InputAction.CallbackContext context) {
       
        jumpHeld=false;
    }

    private void CallTapJump (InputAction.CallbackContext context) {
        //Debug.Log("is tap jumping");
        
        jumpDown=true;
    }

    private void CallHoldJump (InputAction.CallbackContext context) {
        //Debug.Log("is jumping high");
        
        jumpHeld=true;
    }

    private void CallLightAttack (InputAction.CallbackContext context) {
       // Debug.Log("is attacking light");
        
        combatController.Attack(false, _frameInput.Move.y);
    }    

    private void CallHeavyAttack (InputAction.CallbackContext context) {
        //Debug.Log("is attacking heavy");
        
        combatController.Attack(true, _frameInput.Move.y);
    }

    private void CallBlock (InputAction.CallbackContext context) {
        //Debug.Log("is blocking");
        GameEvents.GetInstance().BlockChanged(true);
        combatController.Block(_frameInput.Move.y);
    }

    private void StopBlock(InputAction.CallbackContext context){
        GameEvents.GetInstance().BlockChanged(false);
    }

    

}