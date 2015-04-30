using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Utilities.FindMissingProjectFiles
{
    class XmlProjectFileParser : IProjectFileParser
    {
        private XmlDocument _xmlProjectFile;
        private string _baseProjectPath;
        private string _projectFilePath;

        public IProjectFileParser Load(string projectFilePath)
        {
            _xmlProjectFile = new XmlDocument();

            try
            {
                _xmlProjectFile.Load(projectFilePath);
            }
            catch (Exception)
            {
                throw new ArgumentException(string.Format("Invalid Project File: {0}", projectFilePath));
            }

            if (_xmlProjectFile.DocumentElement == null)
                throw new ArgumentException(string.Format("Project file contains no Root element: {0}", projectFilePath));


            _baseProjectPath = new FileInfo(projectFilePath)
                .DirectoryName;
            _projectFilePath = projectFilePath;

            return this;
        }

        private XmlNodeList GetContentNodes(string xpath)
        {
            var nsmgr = new XmlNamespaceManager(_xmlProjectFile.NameTable);
            nsmgr.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");

            var contentNodes = _xmlProjectFile.SelectNodes(xpath, nsmgr);

            return contentNodes;
        }

        private XmlNodeList GetAllContentNodes()
        {
            var contentNodes = GetContentNodes("//ns:Project/ns:ItemGroup/ns:Content");

            if (contentNodes == null || !contentNodes.OfType<XmlNode>().Any())
                throw new Exception("Project File contains no Content files!");

            return contentNodes;
        }

        public IEnumerable<ProjectFile> ProjectFiles
        {
            get
            {
                var contentNodes = GetAllContentNodes();

                return contentNodes.OfType<XmlNode>()
                    .Select(cn => ProjectFile.Load(cn, _baseProjectPath));
            }
        }

        public IProjectFileParser RemoveFiles(IEnumerable<ProjectFile> files, out int totalFiles, out int removedFilesCount)
        {
            var contentNodes = GetAllContentNodes();
            totalFiles = files.Count();
            var localRemovedFilesCount = 0;

            contentNodes.OfType<XmlNode>()
                .Where(cn => files.Any(f => ProjectFile.Load(cn, _baseProjectPath).Equals(f)))
                .ToList()
                .ForEach(cn =>
                {
                    var pn = cn.ParentNode;
                    pn.RemoveChild(cn);

                    localRemovedFilesCount++;
                });

            removedFilesCount = localRemovedFilesCount;

            return this;
        }

        public IProjectFileParser SaveProjectFile()
        {
            _xmlProjectFile.Save(_projectFilePath);

            return this;
        }
    }
}