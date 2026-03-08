using UnityEngine;
using Utilities;

namespace DependencyInjection
{
    public class DependencyContainer : Singleton<DependencyContainer>
    {
        public PlayerContainer PlayerContainer { get; private set; } = new PlayerContainer();
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
        }
    }
}