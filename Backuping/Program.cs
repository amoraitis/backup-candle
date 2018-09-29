using System;
using System.IO;
using System.Linq;

namespace Backuping
{
    class Program
    {
        static void Main(string[] args)
        {
            string source_path = "";
            var docs_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var destination_path = Path.Combine(docs_path, "external_files");
            var drivesList = Environment.GetLogicalDrives().ToList();
            int i;
            Console.WriteLine("Select disk: ");
            for (i = 0; i < drivesList.Count; i++)
            {
                if (drivesList[i].Contains("C")) drivesList.RemoveAt(i);
                Console.WriteLine(i + " for " + drivesList[i]);
            }
            int desired_key = -1;
            Console.WriteLine("Insert a number between 0 and " + (drivesList.Count-1));
            while (true)
            {
                if (!int.TryParse(Console.ReadLine().First().ToString(), out desired_key))
                {
                    Console.WriteLine("Insert number!");
                    continue;
                }
                if (desired_key >= 0 && desired_key < drivesList.Count)
                    break;
                else
                {
                    Console.WriteLine("Write a number between the limits!");
                }

            }
            source_path = drivesList[desired_key];
            
            // Copy from the current directory, include subdirectories.
            DirectoryCopy(source_path, destination_path, true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (File.Exists(temppath))
                {
                    if (file.LastAccessTime.CompareTo(File.GetLastAccessTime(temppath)) <= 0 && file.LastWriteTime.CompareTo(File.GetLastWriteTime(temppath)) <= 0)
                    {
                        continue;
                    }
                }                
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    Console.WriteLine("Copying subdir: " + subdir.Name);
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
