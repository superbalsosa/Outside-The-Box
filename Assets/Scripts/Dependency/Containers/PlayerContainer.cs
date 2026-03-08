
namespace DependencyInjection
{
    public class PlayerContainer : BaseContainer
    {
        public void RegisterServices(InterfaceDependencyInjector injector)
        {
            //injector.Register<IPlayerInteractMarkerPrompt>(() => FindAndValidate<PlayerInteractMarkerPrompt>());
        }
    }
}