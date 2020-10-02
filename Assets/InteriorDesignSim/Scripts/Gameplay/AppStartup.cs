using Signals;
using UnityEngine;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
    public class AppStartup : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.RegisterService(new ConfigsProvider());
            ServiceLocator.RegisterService(new SignalDispatcher());

            Destroy(this);
        }

        // TODO Arthur: Deregister services?
    }
}