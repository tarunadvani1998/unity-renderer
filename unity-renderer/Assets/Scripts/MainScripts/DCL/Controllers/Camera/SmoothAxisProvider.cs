using Cinemachine;
using Cinemachine.Utility;
using DCL.Camera;
using DCL.Helpers;
using UnityEngine;

public class SmoothAxisProvider : MonoBehaviour, AxisState.IInputAxisProvider
{
    public Vector3 dampTime;

    private Vector3 axis = new Vector3();
    private Vector3 axisTarget = new Vector3();
    private CursorLockMode lastCursorLockState;
    private int framesAfterCursorUnlocked;

    public InputAction_Measurable axisX;
    public InputAction_Measurable axisY;
    private InputSpikeFixer[] inputSpikeFixer;

    private void Awake()
    {
        inputSpikeFixer = new []
        {
            new InputSpikeFixer(() => Cursor.lockState),
            new InputSpikeFixer(() => Cursor.lockState)
        };
    }
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.None && lastCursorLockState == CursorLockMode.Locked)
        {
            framesAfterCursorUnlocked++;
            
            if (framesAfterCursorUnlocked < 30)
            {
                axis = Vector3.zero;
                return;
            }
        }
            
        lastCursorLockState = Cursor.lockState;
        framesAfterCursorUnlocked = 0;
        axisTarget[0] = axisX.GetValue();
        axisTarget[1] = axisY.GetValue();
        axis += Damper.Damp(axisTarget - axis, dampTime, Time.deltaTime);
    }

    public float GetAxisValue(int axis)
    {
        return inputSpikeFixer[axis].GetValue(this.axis[axis]);
    }
}