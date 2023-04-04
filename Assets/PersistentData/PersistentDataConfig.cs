using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentDataConfig {
    static string StandaloneRoot = Application.persistentDataPath + "/";
    static string WebGlRoot = "idbfs/" + Application.productName + "/";

    public static string RootPath {
        get {
            string path = StandaloneRoot;

            #if UNITY_WEBGL
            path = WebGlRoot;
            #endif

            return path;
        }
    }

    public static string CourseDataPath {
        get { return RootPath + "/CourseData/"; }
    }

    static string FileExtension = ".json";
    static string BackupFileExtension = ".json.backup";

    static string SettingsDataFile = "settings" + FileExtension;
    static string SettingsDataFileBackup = "settings" + BackupFileExtension;

    public static string SettingsSaveDataPath {
        get { return RootPath + SettingsDataFile; }
    }

    public static string SettingsSaveDataBackupPath {
        get { return RootPath + SettingsDataFileBackup; }
    }

    public static string FileSearchPattern  {
        get { return "*" + FileExtension; }
    }

    public static string BackupFileSearchPattern {
        get { return "*" + BackupFileExtension; }
    }

    public static string GetCourseSaveDataPath(string courseSlug) {
        return $"{CourseDataPath}{courseSlug}{FileExtension}";
    }
    
    public static string GetCourseSaveDataBackupPath(string courseSlug) {
        return $"{CourseDataPath}{courseSlug}{BackupFileExtension}";
    }
}
