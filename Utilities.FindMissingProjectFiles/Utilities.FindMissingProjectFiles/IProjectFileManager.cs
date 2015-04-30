using System.Collections.Generic;

namespace Utilities.FindMissingProjectFiles
{
    public interface IProjectFileManager
    {
        void LoadProjectFile(string projectFilePath);
        IEnumerable<ProjectFile> ProjectFiles { get; }
        IEnumerable<ProjectFile> MissingFiles { get; }
        void RemoveMissingFilesFromProject(out int totalFiles, out int removedFilesCount);
    }
}