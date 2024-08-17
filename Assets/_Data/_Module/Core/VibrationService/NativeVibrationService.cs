using UnityEngine;

namespace Core.VibrationService
{
    public class NativeVibrationService : IVibrationService
    {
        private bool enabled;

        public NativeVibrationService()
        {
            this.enabled = true;
        }
        
        public void Vibrate()
        {
            if (this.enabled)
            {
                Handheld.Vibrate();
            }
        }

        public void SetEnable(bool enabled)
        {
            this.enabled = enabled;
        }
    }
}