using System;
using System.Threading;
using StructureMap;

namespace Utilities.FindMissingProjectFiles
{
    static class ObjectFactory
    {
        private static readonly Lazy<Container> ContainerBuilder =
            new Lazy<Container>(DefaultContainer, LazyThreadSafetyMode.ExecutionAndPublication);

        private static Container DefaultContainer()
        {
            return new Container(ce => ce.AddRegistry<DefaultMappingRegistry>());
        }

        public static IContainer Container
        {
            get { return ContainerBuilder.Value; }
        }
    }
}