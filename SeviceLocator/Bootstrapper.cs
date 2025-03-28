using UnityEngine;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Learn to Build an Advanced Event Bus | Unity Architecture] (https://www.youtube.com/watch?v=4_DTAnigmaQ)
// -----------------------------------------------------------------------


namespace UnityServiceLocator {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour {
        ServiceLocator container;
        internal ServiceLocator Container => container.OrNull() ?? (container = GetComponent<ServiceLocator>());
        
        bool hasBeenBootstrapped;

        void Awake() => BootstrapOnDemand();
        
        public void BootstrapOnDemand() {
            if (hasBeenBootstrapped) return;
            hasBeenBootstrapped = true;
            Bootstrap();
        }
        
        protected abstract void Bootstrap();
    }

    [AddComponentMenu("ServiceLocator/ServiceLocator Global")]
    public class ServiceLocatorGlobal : Bootstrapper {
        [SerializeField] bool dontDestroyOnLoad = true;
        
        protected override void Bootstrap() {
            Container.ConfigureAsGlobal(dontDestroyOnLoad);
        }
    }
    
    [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
    public class ServiceLocatorScene : Bootstrapper {
        protected override void Bootstrap() {
            Container.ConfigureForScene();            
        }
    }
}