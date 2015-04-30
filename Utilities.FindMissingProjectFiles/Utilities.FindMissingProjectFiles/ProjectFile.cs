using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Utilities.FindMissingProjectFiles
{
    public class ProjectFile : IEquatable<ProjectFile>
    {
        public string Name { get; set; }
        public string FullPath { get; set; }

        public static ProjectFile Load(XmlNode contentXmlNode, string baseProjectPath)
        {
            if (contentXmlNode.Attributes == null
                || !contentXmlNode.Attributes.OfType<XmlAttribute>().Any())
                return null;

            var includeAttribute = contentXmlNode.Attributes.GetNamedItem("Include");

            if (includeAttribute == null)
                return null;

            var fullPath = Path.Combine(baseProjectPath, includeAttribute.Value);
            var fileInfo = new FileInfo(fullPath);

            return new ProjectFile
            {
                Name = fileInfo.Name,
                FullPath = fileInfo.FullName
            };
        }

        public bool Equals(ProjectFile other)
        {
            return this.FullPath.Equals(other.FullPath, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}