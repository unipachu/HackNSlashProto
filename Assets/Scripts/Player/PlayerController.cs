using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Walking,
    Attacking,
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Units per second.")]
    [SerializeField] private float maxLinearSpeed = 5;
    [Tooltip("Degrees per second.")]
    [SerializeField] private float maxAngularSpeed = 800;
    [Tooltip("Units per second squared.")]
    [SerializeField] private float acceleration = 100;

    [Header("Attack Settings")]
    [Tooltip("In seconds.")]
    [SerializeField] private int swordSwingDmg = 1;

    [Header("Refs")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionProperty moveActionProperty;
    [SerializeField] private InputActionProperty attackActionProperty;
    [SerializeField] private Transform sword;
    [SerializeField] private CharacterVisualsAnimationController _characterVisualsAnimationController;
    [SerializeField] private WeaponColliderHitSensor _weaponColliderHitSensor;

    private Vector2 velocity = Vector2.zero;
    private Vector2 moveInput = Vector2.zero;
    private bool attackInput = false;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void Update()
    {
        ReadInputs();
        if (attackInput) _characterVisualsAnimationController.TryBufferAttack();


        // TODO: Make better system using animator states instead, maybe.
        if (_characterVisualsAnimationController.IsPlaying_Idle())
        {
            if(attackInput)
            {
                _characterVisualsAnimationController.Play_SwingAttack();
            }
            else if (moveInput != Vector2.zero)
            {
                _characterVisualsAnimationController.Play_Walk();
            }
        }
        else if (_characterVisualsAnimationController.IsPlaying_Walk())
        {
            SolveMovement(moveInput);

            if (attackInput)
            {
                _characterVisualsAnimationController.Play_SwingAttack();
            }
            else if (moveInput == Vector2.zero)
            {
                _characterVisualsAnimationController.Play_Idle();
            }
        }
        else if (_characterVisualsAnimationController.IsPlaying_KnockBackBackward())
        {
            //???
        }
        else if(_characterVisualsAnimationController.IsPlaying_SwingAttack())
        {
            if(_characterVisualsAnimationController.ComboWindowEnded())
            {
                // Set correct animations.
                if (moveInput != Vector2.zero && !_characterVisualsAnimationController.IsPlaying_Walk())
                {
                    _characterVisualsAnimationController.Play_Walk();
                }
            }
        }

        // TODO: Check animation states instead?
        //switch (state)
        //{
        //    case PlayerState.Walking:
        //        SolveMovement(moveInput);
        //        if(attackInput)
        //        {
        //            ChangeState(PlayerState.Attacking);
        //            break;
        //        }
        //        break;
        //    // TODO: Change this to use animations instead.
        //    case PlayerState.Attacking:
        //        float t = attackTimer / attackDuration;
        //        float half = attackDuration / 2;
        //        if (swordSide == Side.Left)
        //        {
        //            Quaternion startRot = Quaternion.Euler(0f, -100f, 0f);
        //            Quaternion endRot = Quaternion.Euler(0f, 100f, 0f);
        //            if (attackTimer / attackDuration >= 1)
        //            {
        //                sword.localRotation = Quaternion.Euler(0f, 90f, 0f);
        //                swordSide = Side.Right;
        //                ChangeState(PlayerState.Walking);
        //                break;
        //            }
        //            if (t < 0.5f)
        //            {
        //                sword.localRotation = Quaternion.Slerp(startRot, Quaternion.identity, attackTimer * 2 / attackDuration);
        //            }
        //            else
        //            {
        //                sword.localRotation = Quaternion.Slerp(Quaternion.identity, endRot, (attackTimer - half) * 2 / attackDuration);
        //            }
        //        }
        //        else
        //        {
        //            Quaternion startRot = Quaternion.Euler(0f, 100f, 0f);
        //            Quaternion endRot = Quaternion.Euler(0f, -100f, 0f);
        //            if (attackTimer / attackDuration >= 1)
        //            {
        //                sword.localRotation = Quaternion.Euler(0f, -90f, 0f);
        //                swordSide = Side.Left;
        //                ChangeState(PlayerState.Walking);
        //                break;
        //            }
        //            if (t < 0.5f)
        //            {
        //                sword.localRotation = Quaternion.Slerp(startRot, Quaternion.identity, attackTimer * 2 / attackDuration);
        //            }
        //            else
        //            {
        //                sword.localRotation = Quaternion.Slerp(Quaternion.identity, endRot, (attackTimer - half) * 2 / attackDuration);
        //            }
        //        }

        //        _weaponColliderHitSensor.CheckHits(swordSwingDmg, transform);

        //        attackTimer += Time.deltaTime;
        //        break;
        //    default:
        //        Debug.LogError("Switch defaulted.");
        //        break;
        //}
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    //private void ChangeState(PlayerState newState)
    //{
    //    Debug.Assert(!isChangingState, "Tried to change state while already changing state.");
    //    isChangingState = true;

    //    switch (newState)
    //    {
    //        case PlayerState.Walking:
    //            break;
    //        case PlayerState.Attacking:
    //            _weaponColliderHitSensor.BeginAttack();
    //            attackTimer = 0;
    //            break;
    //        default:
    //            Debug.LogError("Switch defaulted.");
    //            break;
    //    }

    //    state = newState;
    //    isChangingState = false;
    //}

    private void ReadInputs()
    {
        moveInput = moveActionProperty.action.ReadValue<Vector2>();
        attackInput = attackActionProperty.action.WasPressedThisFrame();
    }

    private void SolveMovement(Vector2 movementInput)
    {
        UpdateVelocity(movementInput);
        Vector3 XYVelocity = new Vector3(velocity.x, 0, velocity.y);
        characterController.SimpleMove(XYVelocity);
        RotateForward();
    }

    private void UpdateVelocity(Vector2 movementInput)
    {
        velocity = Vector2.MoveTowards(velocity, movementInput * maxLinearSpeed, acceleration * Time.deltaTime);
    }

    public void RotateForward()
    {
        if(velocity ==  Vector2.zero) return;

        Vector3 dir3D = new(velocity.x, 0, velocity.y);

        Quaternion targetRotation = Quaternion.LookRotation(dir3D, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngularSpeed * Time.deltaTime);
    }
}
