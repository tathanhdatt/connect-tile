using Core.Service;

namespace Core.VibrationService
{
    public interface IVibrationService : IService
    {
        void Vibrate();
        void SetEnable(bool enabled);
    }
}