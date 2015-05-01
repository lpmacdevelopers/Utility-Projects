using StructureMap.Configuration.DSL;
using Utilities.FindMissingProjectFiles.Contracts;
using Utilities.FindMissingProjectFiles.Impl;

namespace Utilities.FindMissingProjectFiles.DI
{
    public class DefaultMappingRegistry : Registry
    {
        public DefaultMappingRegistry()
        {
            For<IProjectFileManager>().Use<DotNetProjectFileManager>();
            For<IProjectFileParser>().Use<DotNetProjectFileParser>();
        }
    }
}