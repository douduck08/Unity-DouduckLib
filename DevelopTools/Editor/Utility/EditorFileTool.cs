using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DouduckGame.Editor {
    public class EditorFileTool {
        public static void CopyDirectory (string sourceDirName, string destDirName, bool copySubDirs) {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo (sourceDirName);
            if (!dir.Exists) {
                throw new DirectoryNotFoundException ("Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories ();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists (destDirName)) {
                Directory.CreateDirectory (destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles ("*.cs");
            foreach (FileInfo file in files) {
                string temppath = Path.Combine (destDirName, file.Name);
                file.CopyTo (temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    string temppath = Path.Combine (destDirName, subdir.Name);
                    CopyDirectory (subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void ReplaceStringInFile (string fileName, string sourceString, string targetString) {
            string text = File.ReadAllText (fileName);
            text = text.Replace (sourceString, targetString);
            File.WriteAllText (fileName, text);
        }

        public static void ReplaceStringInAllFile (string sourceDirName, string searchPattern, string sourceString, string targetString) {
            DirectoryInfo dir = new DirectoryInfo (sourceDirName);
            if (!dir.Exists) {
                throw new DirectoryNotFoundException ("Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories ();
            FileInfo[] files = dir.GetFiles (searchPattern);
            foreach (FileInfo file in files) {
                string temppath = Path.Combine (sourceDirName, file.Name);
                ReplaceStringInFile (temppath, sourceString, targetString);
            }
            foreach (DirectoryInfo subdir in dirs) {
                ReplaceStringInAllFile (subdir.FullName, searchPattern, sourceString, targetString);
            }
        }
    }
}
