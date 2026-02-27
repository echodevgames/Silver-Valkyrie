// -----FlipperInput.cs START-----
using UnityEngine;
using UnityEngine.InputSystem;

public class FlipperInput : MonoBehaviour
{
    public bool isLeftFlipper = true;

    private HingeJoint2D joint;

    void Awake()
    {
        joint = GetComponent<HingeJoint2D>();
    }

    void Update()
    {
        JointMotor2D motor = joint.motor;

        bool pressed = isLeftFlipper
            ? Keyboard.current.leftArrowKey.isPressed
            : Keyboard.current.rightArrowKey.isPressed;

        // Left and right flippers rotate opposite directions
        motor.motorSpeed = pressed
            ? (isLeftFlipper ? -900 : 900)
            : (isLeftFlipper ? 900 : -900);

        joint.motor = motor;
    }
}


// -----FlipperInput.cs END-----
