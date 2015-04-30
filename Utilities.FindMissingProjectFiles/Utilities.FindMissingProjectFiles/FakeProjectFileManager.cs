using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utilities.FindMissingProjectFiles
{
    public class FakeProjectFileManager : IProjectFileManager
    {
        private readonly IProjectFileParser _projectFileParser;

        public FakeProjectFileManager(IProjectFileParser projectFileParser)
        {
            _projectFileParser = projectFileParser;
            ProjectFiles = null;
        }

        public void LoadProjectFile(string projectFilePath)
        {
            ProjectFiles = _projectFileParser
                .Load(projectFilePath)
                .ProjectFiles;
        }

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

        public IEnumerable<ProjectFile> ProjectFiles { get; private set; }

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
    }
}