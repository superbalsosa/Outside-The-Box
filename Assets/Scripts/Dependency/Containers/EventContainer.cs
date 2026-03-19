using UnityEngine;
namespace DependencyInjection
{
    public class EventContainer : BaseContainer
    {
        public void RegisterServices(InterfaceDependencyInjector injector)
        {
            //injector.Register<IBasicEvent>(() => FindAndValidate<BasicEvent>());
        }
    }
}
