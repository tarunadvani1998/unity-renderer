using System.Collections;
using Cinemachine;
using Cinemachine.Utility;
using DCL.Camera;
using UnityEngine;

public class SmoothAxisProvider : MonoBehaviour, AxisState.IInputAxisProvider
{
    public Vector3 dampTime;

    private Vector3 axis = new Vector3();
    private Vector3 axisTarget = new Vector3();
    private CursorLockMode lastCursorLockState;
    private int framesAfterCursorLock;

    public InputAction_Measurable axisX;
    public InputAction_Measurable axisY;
    private InputSpikeFixer[] inputSpikeFixer;
    private Vector2 fakeAxis;

    private void Awake()
    {
        inputSpikeFixer = new []
        {
            new InputSpikeFixer(() => Cursor.lockState),
            new InputSpikeFixer(() => Cursor.lockState)
        };

        StartCoroutine(FakeAxisOverTime());
    }
    
    void Update()
    {
        /*if (Cursor.lockState == CursorLockMode.Locked && lastCursorLockState == CursorLockMode.None)
        {
            framesAfterCursorLock++;
            
            if (framesAfterCursorLock < 30)
            {
                axis = Vector3.zero;
                return;
            }
        }*/
        
        Debug.Log($"axisX: {axisX.GetValue()}, axisY: {axisY.GetValue()}");
            
        lastCursorLockState = Cursor.lockState;
        framesAfterCursorLock = 0;
        
        if (!DCL.Helpers.Utils.isCursorLocked)
        {
            axisTarget[0] = 0f;
            axisTarget[1] = 0f;
        }
        else
        {
            axisTarget[0] = fakeAxis.x;
            axisTarget[1] = fakeAxis.y;
        }
        
        axis += Damper.Damp(axisTarget - axis, dampTime, Time.deltaTime);
    }

    public float GetAxisValue(int axis)
    {
        return inputSpikeFixer[axis].GetValue(this.axis[axis]);
    }

    private IEnumerator FakeAxisOverTime()
    {
        while (gameObject)
        {
            fakeAxis = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            yield return new WaitForSeconds(2.5f);
        }
    }
}