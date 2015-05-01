using System.Collections.Generic;
using Utilities.FindMissingProjectFiles.Impl;

namespace Utilities.FindMissingProjectFiles.Contracts
{
    /// <summary>
    /// Defines the properties and methods needed to manage the Project file
    /// </summary>
    public interface IProjectFileManager
    {
        /// <summary>
        /// Initiatlises the project file manager with the full project file path
        /// </summary>
        /// <param name="projectFilePath">Full path to the project file</param>
        void LoadProjectFile(string projectFilePath);

        /// <summary>
        /// Gets the Project files from the project file
        /// </summary>
        IEnumerable<ProjectFile> ProjectFiles { get; }

        /// <summary>
        /// Gets the list of <see cref="ProjectFile"/> objects from the Project file that are missing in the File System
        /// </summary>
        IEnumerable<ProjectFile> MissingFiles { get; }

        /// <summary>
        /// Removes the missing files from the Project file and saves the file
        /// </summary>
        /// <param name="totalFiles">When the methods succeeds, this out param will contain the count of missing files</param>
        /// <param name="removedFilesCount">When the methods succeeds, this out param will contain the count of missing files that were removed</param>
        void RemoveMissingFilesFromProject(out int totalFiles, out int removedFilesCount);
    }
}