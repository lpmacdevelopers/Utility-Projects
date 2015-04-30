using System.Collections.Generic;

namespace Utilities.FindMissingProjectFiles
{
    public interface IProjectFileParser
    {
        IProjectFileParser Load(string projectFilePath);
        IEnumerable<ProjectFile> ProjectFiles { get; }
        IProjectFileParser RemoveFiles(IEnumerable<ProjectFile> files, out int totalFiles, out int removedFilesCount);
        IProjectFileParser SaveProjectFile();
    }
}