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
    private CursorLockMode lastLockState;
    private bool wasLocked;

    public InputAction_Measurable axisX;
    public InputAction_Measurable axisY;
    private InputSpikeFixer[] inputSpikeFixer;

    private void Awake()
    {
        inputSpikeFixer = new[]
        {
            new InputSpikeFixer(() => Cursor.lockState),
            new InputSpikeFixer(() => Cursor.lockState)
        };
    }

    void Update()
    {
        if (lastLockState != Cursor.lockState)
            Debug.Log($"lockState changed to {Cursor.lockState}");
        if (Utils.isCursorLocked != wasLocked)
            Debug.Log($"locked changed to {Utils.isCursorLocked}");
        wasLocked = Utils.isCursorLocked;
        lastLockState = Cursor.lockState;
        Debug.Log($"lockState: {Cursor.lockState}, axisX: {axisX.GetValue()}, axisY: {axisY.GetValue()}, unityAxisX: {Input.GetAxis("Mouse X")}, unityAxisY: {Input.GetAxis("Mouse Y")}");

        axisTarget[0] = axisX.GetValue();
        axisTarget[1] = axisY.GetValue();

        axis += Damper.Damp(axisTarget - axis, dampTime, Time.deltaTime);
    }

    public float GetAxisValue(int axis)
    {
        return inputSpikeFixer[axis].GetValue(this.axis[axis]);
    }
}