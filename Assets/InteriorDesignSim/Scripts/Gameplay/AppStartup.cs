using UnityEngine;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
    public class AppStartup : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.RegisterService(new ConfigsProvider());

            Destroy(this);
        }

        // TODO Arthur: Deregister services?
    }
}