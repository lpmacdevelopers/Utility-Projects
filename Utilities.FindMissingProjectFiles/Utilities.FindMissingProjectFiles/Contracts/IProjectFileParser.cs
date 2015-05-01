using System.Collections.Generic;
using Utilities.FindMissingProjectFiles.Impl;

namespace Utilities.FindMissingProjectFiles.Contracts
{
    /// <summary>
    /// Defines the properties and methods to effectively parse a project file
    /// </summary>
    public interface IProjectFileParser
    {
        /// <summary>
        /// Gets the Project's base path
        /// </summary>
        string ProjectBasePath { get; }

        /// <summary>
        /// Initiates the parser instance with the Project file
        /// </summary>
        /// <param name="projectFilePath">Full path to the Project file</param>
        /// <returns>Instance of <see cref="IProjectFileParser"/></returns>
        IProjectFileParser Load(string projectFilePath);

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of ProjectFile objects
        /// </summary>
        IEnumerable<ProjectFile> ProjectFiles { get; }

        /// <summary>
        /// Removes the files from the Project file as specified in the <paramref name="filesToBeRemoved"/> parameter
        /// </summary>
        /// <param name="filesToBeRemoved">An <see cref="IEnumerable{T}"/> of ProjectFiles to be removed from the Project file</param>
        /// <param name="totalFiles">When the method succeeds, this out parameter will contain the count of files to be removed</param>
        /// <param name="removedFilesCount">When the method succeeds, this out parameter will contain the count of files that were removed</param>
        /// <returns></returns>
        IProjectFileParser RemoveFiles(IEnumerable<ProjectFile> filesToBeRemoved, out int totalFiles, out int removedFilesCount);

        /// <summary>
        /// Saves the Project file
        /// </summary>
        /// <returns></returns>
        IProjectFileParser SaveProjectFile();
    }
}