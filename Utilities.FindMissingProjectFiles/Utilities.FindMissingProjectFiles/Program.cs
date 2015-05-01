using System;
using System.Linq;
using System.Text;
using CommandLine;
using Utilities.FindMissingProjectFiles.Contracts;
using Utilities.FindMissingProjectFiles.DI;

namespace Utilities.FindMissingProjectFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            var parser = new Parser();

            if (parser.ParseArguments(args, options))
            {
                var projectFileManager = ObjectFactory.Container.GetInstance<IProjectFileManager>();
                projectFileManager.LoadProjectFile(options.ProjectFile);

                if (options.DisplayMissingFiles)
                {
                    var missingFiles = projectFileManager.MissingFiles.ToList();

                    Console.WriteLine("Displaying Missing Files: {0} in total", missingFiles.Count());
                    Console.WriteLine();

                    missingFiles
                        .ForEach(mf => Console.WriteLine("{0}\t{1}", mf.Name, mf.FullPath));
                }

                if (options.RemoveMissingFiles)
                {
                    int totalFiles;
                    int removedFilesCount;

                    projectFileManager.RemoveMissingFilesFromProject(out totalFiles, out removedFilesCount);

                    Console.WriteLine("Total Missing Files: {0}; Removed: {1}", totalFiles, removedFilesCount);
                }
            }
            else
            {
                Console.WriteLine(options.GetUsage());
            }

            Console.WriteLine();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private class Options
        {
            [Option('p', "project file", Required = true, HelpText = "Project file to read.")]
            public string ProjectFile { get; set; }

            [Option('d', "display", Required = false, HelpText = "Display missing files.")]
            public bool DisplayMissingFiles { get; set; }

            [Option('r', "remove", Required = false, HelpText = "Remove missing files.")]
            public bool RemoveMissingFiles { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                // this without using CommandLine.Text
                var usage = new StringBuilder();
                usage.AppendLine("FindMissingProjectFiles v1.0");
                usage.AppendLine("Options:");
                usage.AppendLine("\t-p:\t(Required) Follow up with the full path to the .Net Project File");
                usage.AppendLine("\t-d:\t(Optional) Apply this argument if you need the list of Missing files");
                usage.AppendLine("\t-r:\t(Optional) Apply this argument if you need the Missing files to be removed from the Project File");
                return usage.ToString();
            }
        }
    }
}
