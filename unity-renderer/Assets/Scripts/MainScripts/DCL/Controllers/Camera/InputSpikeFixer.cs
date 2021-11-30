using System;
using System.Linq;
using UnityEngine;

namespace DCL.Camera
{
    public class InputSpikeFixer
    {
        private const float INPUT_SPIKE_TOLERANCE = 10f;
        private const int HISTORY_SIZE = 8;
        
        private readonly Func<CursorLockMode> getLockMode;
        private readonly float[] history = new float[HISTORY_SIZE];
        
        private CursorLockMode lastLockState;
        private bool isLockStateDirty;
        private int currentHistoryIndex;

        public InputSpikeFixer(Func<CursorLockMode> getLockMode)
        {
            this.getLockMode = getLockMode;
            lastLockState = getLockMode();
            isLockStateDirty = false;
        }
        
        public float GetValue(float currentValue)
        {
            CheckLockState();
            
            history[currentHistoryIndex] = currentValue;
            currentHistoryIndex = (currentHistoryIndex + 1) % HISTORY_SIZE;
            currentValue = history.Average();

            float absoluteValue = Mathf.Abs(currentValue);

            if (ShouldIgnoreInputValue(absoluteValue))
            {
                if (IsInputValueTolerable(absoluteValue))
                    isLockStateDirty = false;
                return 0;
            }
            
            return currentValue;
        }
        
        private static bool IsInputValueTolerable(float value) { return value < INPUT_SPIKE_TOLERANCE; }
        private bool ShouldIgnoreInputValue(float value) { return value > 0 && isLockStateDirty; }

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