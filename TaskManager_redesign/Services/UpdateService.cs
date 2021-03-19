using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace TaskManager_redesign.Services
{
    public static class UpdateService
    {
#if !DevAtHome
        static double currentVer;
        static double targetVer;
        const string APP_NAME = "TaskManager_redesign.exe";
        const string ROOT_PATH = @"\\moscow\hdfs\WORK\Архив необычных операций\ОРППА\2. Направление автоматизации\Programs\Tasks";
        const string CHANGELOG_FILEPATH = @"\\moscow\hdfs\WORK\Архив необычных операций\ОРППА\2. Направление автоматизации\Programs\Tasks\Changelog.txt";
        const string UPDATE_TEXT = "Обнаружена новая версия программы. TaskManager будет перезапущен после обновления";
#endif
        public static void CheckForUpdate()
        {
#if !DevAtHome
            if (!IsServerFileVersionIsNewer(Assembly.GetExecutingAssembly().GetName().Version, FileVersionInfo.GetVersionInfo($"{ROOT_PATH}\\{APP_NAME}")))
            {
                return;
            }

            MessageBox.Show(UPDATE_TEXT, "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
            List<string> updateText = new List<string>();
            updateText.Add("-g");
            foreach (string fileName in Directory.GetFiles(ROOT_PATH))
            {
                if (fileName.IndexOf("Changelog.txt") > -1)
                    continue;
                updateText.Add($"\"{fileName}\"");
            }
            updateText.Add("-k");
            updateText.Add($"{Process.GetCurrentProcess().ProcessName}");
            updateText.Add("-r");
            updateText.Add(APP_NAME);
            updateText.Add("-d");
            using (StreamReader reader = new StreamReader(CHANGELOG_FILEPATH))
            {
                string readedLine = string.Empty;
                while ((readedLine = reader.ReadLine()) != null)
                {
                    updateText.Add(readedLine);
                }
            }
            updateText.Add(APP_NAME);
            Process.Start("updaterForm.exe", string.Join(" ", updateText.ToArray()));

#endif
        }

        private static bool IsServerFileVersionIsNewer(Version currentVersion, FileVersionInfo serverFileVersion)
        {
            Version serverVersion = new Version($"{serverFileVersion.FileMajorPart}.{serverFileVersion.FileMinorPart}.{serverFileVersion.FileBuildPart}.{serverFileVersion.FilePrivatePart}");
            return currentVersion < serverVersion;
        }
    }
}
