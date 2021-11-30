using System;
using UnityEngine;

namespace DCL.Camera
{
    public class InputSpikeFixer
    {
        private const float INPUT_SPIKE_TOLERANCE = 10f;
        private readonly Func<CursorLockMode> getLockMode;

        private CursorLockMode lastLockState;
        private bool isLockStateDirty;
        private int framesAfterDirty;

        public InputSpikeFixer(Func<CursorLockMode> getLockMode)
        {
            this.getLockMode = getLockMode;
            lastLockState = getLockMode();
            isLockStateDirty = false;
        }
        public float GetValue(float currentValue)
        {
            CheckLockState();

            float absoluteValue = Mathf.Abs(currentValue);

            if (ShouldIgnoreInputValue(absoluteValue))
            {
                framesAfterDirty++;
                if (IsInputValueTolerable(absoluteValue))
                    isLockStateDirty = false;
                return 0;
            }
            
            framesAfterDirty = 0;
            return currentValue;
        }
        private static bool IsInputValueTolerable(float value) { return value < INPUT_SPIKE_TOLERANCE; }
        private bool ShouldIgnoreInputValue(float value) { return value > 0 && isLockStateDirty && framesAfterDirty < 30; }

        private void CheckLockState()
        {
            var currentLockState = getLockMode();

            if (currentLockState != lastLockState)
            {
                isLockStateDirty = true;
            }

            lastLockState = currentLockState;
        }
    }
}