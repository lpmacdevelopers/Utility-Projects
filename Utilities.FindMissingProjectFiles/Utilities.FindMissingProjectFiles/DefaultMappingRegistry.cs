using StructureMap.Configuration.DSL;

namespace Utilities.FindMissingProjectFiles
{
    public class DefaultMappingRegistry : Registry
    {
        public DefaultMappingRegistry()
        {
            For<IProjectFileManager>().Use<FakeProjectFileManager>();
            For<IProjectFileParser>().Use<XmlProjectFileParser>();
        }
    }
}