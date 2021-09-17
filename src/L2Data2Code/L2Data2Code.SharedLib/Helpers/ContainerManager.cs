using Unity;

namespace L2Data2Code.SharedLib.Helpers
{
    public static class ContainerManager
    {
        private static IUnityContainer container;

        public static IUnityContainer Container => container;

        public static IUnityContainer SetupContainer(IUnityContainer unityContainer)
        {
            container = unityContainer;
            return container;
        }
    }
}
