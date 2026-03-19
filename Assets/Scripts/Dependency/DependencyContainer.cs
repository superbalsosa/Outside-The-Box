using UnityEngine;
using Utilities;

namespace DependencyInjection
{
    public class DependencyContainer : Singleton<DependencyContainer>
    {
        public PlayerContainer PlayerContainer { get; private set; } = new PlayerContainer();
        public EventContainer EventContainer { get; private set; } = new EventContainer();
        protected override void Awake()
        {
            base.Awake();

            RegisterAll();
        }
        private void RegisterAll()
        {
            var injector = InterfaceDependencyInjector.Instance;

            if (injector == null)
            {
                Debug.LogError("[DependencyContainer] No se encontró el Inyector en la escena.");
                return;
            }

            injector.ClearInstances();

            PlayerContainer.RegisterServices(injector);
            EventContainer.RegisterServices(injector);
        }
    }
}