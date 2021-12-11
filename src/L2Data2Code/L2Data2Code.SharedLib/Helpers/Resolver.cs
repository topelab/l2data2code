using Unity;

namespace L2Data2Code.SharedLib.Helpers
{
    public class Resolver
    {
        private static IUnityContainer container;

        public static T Get<T>()
        {
            return container.Resolve<T>();
        }

        public static void Initialize(IUnityContainer container)
        {
            Resolver.container = container;
        }
    }
}
