using System;
using System.IO;
using System.Linq;

namespace BuckUp.Candle
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourcePath = "";
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var destinationPath = Path.Combine(docsPath, "external_files");
            var drivesList = Environment.GetLogicalDrives().ToList();
            int i;

            Console.WriteLine("Select disk: ");

            for (i = 0; i < drivesList.Count; i++)
            {
                if (drivesList[i].Contains("C")) drivesList.RemoveAt(i);
                Console.WriteLine(i + " for " + drivesList[i]);
            }

            Console.WriteLine("Insert a number between 0 and " + (drivesList.Count - 1));

            var desiredKey = -1;

            while (true)
            {
                var input = Console.ReadLine();

                if (int.TryParse(input?.First().ToString(), out desiredKey) == false)
                {
                    Console.WriteLine("Insert number!");
                    continue;
                }

                if (desiredKey >= 0 && desiredKey < drivesList.Count)
                {
                    break;
                }

                Console.WriteLine("Write a number between the limits!");

            }

            sourcePath = drivesList[desiredKey];

            // Copy from the current directory, include subdirectories.
            DirectoryCopy(sourcePath, destinationPath, true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (Directory.Exists(destDirName) == false)
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                var tempPath = Path.Combine(destDirName, file.Name);

                if (File.Exists(tempPath) && file.LastAccessTime.CompareTo(File.GetLastAccessTime(tempPath)) <= 0 && file.LastWriteTime.CompareTo(File.GetLastWriteTime(tempPath)) <= 0)
                {
                    continue;
                }

                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (var subDir in dirs)
                {
                    Console.WriteLine("Copying subdir: " + subDir.Name);

                    string tempPath = Path.Combine(destDirName, subDir.Name);

                    DirectoryCopy(subDir.FullName, tempPath, true);
                }
            }
        }
    }
}
