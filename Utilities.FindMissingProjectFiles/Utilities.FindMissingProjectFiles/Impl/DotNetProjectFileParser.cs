using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Utilities.FindMissingProjectFiles.Contracts;

namespace Utilities.FindMissingProjectFiles.Impl
{
    class DotNetProjectFileParser : IProjectFileParser
    {
        private XmlDocument _xmlProjectFile;
        private string _baseProjectPath;
        private string _projectFilePath;
        private string _projectFileXmlNamespace;

        #region Private methods

        private string ProjectFileXmlNamespace
        {
            get
            {
                if (!string.IsNullOrEmpty(_projectFileXmlNamespace)) return _projectFileXmlNamespace;

                if (_xmlProjectFile == null || _xmlProjectFile.DocumentElement == null)
                    throw new InvalidOperationException("Project file is not initialised");

                if (_xmlProjectFile.DocumentElement.HasAttributes)
                {
                    _projectFileXmlNamespace = _xmlProjectFile.DocumentElement
                        .Attributes.GetNamedItem("xmlns")
                        .Value;
                }

                return _projectFileXmlNamespace;
            }
        }

        /// <summary>
        /// Gets the <see cref="XmlNodeList"/> of all elements from the Project file, filtered by an xpath expression
        /// </summary>
        /// <param name="xpath">XPath string expression to filter only the required elements</param>
        /// <param name="nsmgr">Optional <see cref="XmlNamespaceManager"/> to be used while selecting nodes from the Project file</param>
        /// <returns></returns>
        private XmlNodeList GetNodes(string xpath, XmlNamespaceManager nsmgr = null)
        {
            var contentNodes = nsmgr == null
                ? _xmlProjectFile.SelectNodes(xpath)
                : _xmlProjectFile.SelectNodes(xpath, nsmgr);

            return contentNodes;
        }

        /// <summary>
        /// Helper method to get all Content elements from the Project file
        /// </summary>
        /// <returns></returns>
        private XmlNodeList GetAllContentNodes()
        {
            XmlNodeList contentNodes;

            if (!string.IsNullOrEmpty(ProjectFileXmlNamespace))
            {
                var nsmgr = new XmlNamespaceManager(_xmlProjectFile.NameTable);
                nsmgr.AddNamespace("ns", ProjectFileXmlNamespace);

                contentNodes = GetNodes("//ns:Project/ns:ItemGroup/ns:Content", nsmgr);
            }
            else
            {
                contentNodes = GetNodes("//Project/ItemGroup/Content");
            }

            if (contentNodes == null || !contentNodes.OfType<XmlNode>().Any())
                throw new Exception("Project File contains no Content files!");

            return contentNodes;
        }

        #endregion

        #region IProjectFileParser implementations

        /// <summary>
        /// Gets the Project's base path
        /// </summary>
        public string ProjectBasePath
        {
            get
            {
                return _baseProjectPath ??
                       (_baseProjectPath = new FileInfo(_projectFilePath).DirectoryName);
            }
        }

        /// <summary>
        /// Initiates the parser instance with the Project file
        /// </summary>
        /// <param name="projectFilePath">Full path to the Project file</param>
        /// <returns>Instance of <see cref="IProjectFileParser"/></returns>
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

            _projectFilePath = projectFilePath;

            return this;
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of ProjectFile objects
        /// </summary>
        public IEnumerable<ProjectFile> ProjectFiles
        {
            get
            {
                var contentNodes = GetAllContentNodes();

                return contentNodes.OfType<XmlNode>()
                    .Select(cn => ProjectFile.Load(cn, ProjectBasePath));
            }
        }

        /// <summary>
        /// Removes the files from the Project file as specified in the <paramref name="filesToBeRemoved"/> parameter
        /// </summary>
        /// <param name="filesToBeRemoved">An <see cref="IEnumerable{T}"/> of ProjectFiles to be removed from the Project file</param>
        /// <param name="totalFiles">When the method succeeds, this out parameter will contain the count of files to be removed</param>
        /// <param name="removedFilesCount">When the method succeeds, this out parameter will contain the count of files that were removed</param>
        /// <returns></returns>
        public IProjectFileParser RemoveFiles(IEnumerable<ProjectFile> filesToBeRemoved, out int totalFiles,
            out int removedFilesCount)
        {
            var contentNodes = GetAllContentNodes();
            var localFilesToBeRemoved = filesToBeRemoved.ToList();
            var localRemovedFilesCount = 0;

            totalFiles = localFilesToBeRemoved.Count();
            
            contentNodes.OfType<XmlNode>()
                .Where(cn => localFilesToBeRemoved.Any(f => ProjectFile.Load(cn, ProjectBasePath).Equals(f)))
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

        /// <summary>
        /// Saves the Project file
        /// </summary>
        /// <returns></returns>
        public IProjectFileParser SaveProjectFile()
        {
            _xmlProjectFile.Save(_projectFilePath);

            return this;
        }

        #endregion

    }
}