using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities.FindMissingProjectFiles.Contracts;

namespace Utilities.FindMissingProjectFiles.Impl
{
    public class DotNetProjectFileManager : IProjectFileManager
    {
        private readonly IProjectFileParser _projectFileParser;
        private IEnumerable<ProjectFile> _projectFiles;

        public DotNetProjectFileManager(IProjectFileParser projectFileParser)
        {
            _projectFileParser = projectFileParser;
        }

        #region IProjectFileManager implementations

        /// <summary>
        /// Initiatlises the project file manager with the full project file path
        /// </summary>
        /// <param name="projectFilePath">Full path to the project file</param>
        public void LoadProjectFile(string projectFilePath)
        {
            _projectFileParser
                .Load(projectFilePath);
        }

        /// <summary>
        /// Gets the Project files from the project file
        /// </summary>
        public IEnumerable<ProjectFile> ProjectFiles
        {
            get { return _projectFiles ?? (_projectFiles = _projectFileParser.ProjectFiles); }
        }

        /// <summary>
        /// Gets the list of <see cref="ProjectFile"/> objects from the Project file that are missing in the File System
        /// </summary>
        public IEnumerable<ProjectFile> MissingFiles
        {
            get
            {
                return ProjectFiles
                    .AsParallel()
                    .Where(pf => !File.Exists(pf.FullPath))
                    .ToList();
            }
        }

        /// <summary>
        /// Removes the missing files from the Project file and saves the file
        /// </summary>
        /// <param name="totalFiles">When the methods succeeds, this out param will contain the count of missing files</param>
        /// <param name="removedFilesCount">When the methods succeeds, this out param will contain the count of missing files that were removed</param>
        public void RemoveMissingFilesFromProject(out int totalFiles, out int removedFilesCount)
        {
            int localTotalFiles;
            int localRemovedFilesCount;

            _projectFileParser
                .RemoveFiles(MissingFiles, out localTotalFiles, out localRemovedFilesCount)
                .SaveProjectFile();

            totalFiles = localTotalFiles;
            removedFilesCount = localRemovedFilesCount;
        }

        #endregion

    }
}