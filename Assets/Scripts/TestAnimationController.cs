using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Used for testing the Unity's enigmatic animator.
/// </summary>
public class TestAnimationController : MonoBehaviour
{
    [SerializeField] private CharacterVisualsAnimationController _characterVisualsAnimationController;
    [SerializeField] private InputActionProperty moveActionProperty;

    private Vector2 previousMoveInput = Vector2.zero;
    private Vector2 moveInput = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        moveInput = moveActionProperty.action.ReadValue<Vector2>();

        //if(moveInput != previousMoveInput)
        //{
        //    if (moveInput.x > 0)
        //    {
        //        _characterVisualsAnimationController.Play_Test2();
        //    }
        //    else if (moveInput.x < 0)
        //    {
        //        _characterVisualsAnimationController.Play_Test1();
        //    }
        //    else
        //        _characterVisualsAnimationController.Play_Test3();
        //        //_characterVisualsAnimationController.Play_KnockBackBackward();

        //    //_characterVisualsAnimationController.Play_Test1();
        //    //_characterVisualsAnimationController.Play_Walk();
        //}

        //else
        //{
        //    _characterVisualsAnimationController.Play_Idle();
        //}

        //_characterVisualsAnimationController.Play_Walk();
        //_characterVisualsAnimationController.Play_Idle();

        //if (moveInput.x > 0)
        //{
        //    _characterVisualsAnimationController.Play_Test2();
        //}
        //else if (moveInput.x < 0)
        //{
        //    _characterVisualsAnimationController.Play_Test1();
        //}


        if (moveInput != previousMoveInput)
        {
            _characterVisualsAnimationController.Play_Test2();
        }


        previousMoveInput = moveInput;

        Debug.Log("Current anim hash: " + _characterVisualsAnimationController.CurrentAnimHash(0));
        Debug.Log("Is in transition: " + _characterVisualsAnimationController.IsInTransition(0));
        Debug.Log("Next anim hash: " + _characterVisualsAnimationController.NextAnimationHash(0));
    }
}
