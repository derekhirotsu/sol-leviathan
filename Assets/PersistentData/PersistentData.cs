using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;

public static class PersistentData {
    [DllImport("__Internal")]
    private static extern void JS_FileSystem_Sync();

    static void SyncFs() {
        #if UNITY_WEBGL
        Debug.Log("WebGL: Flushing files to disk...");
        JS_FileSystem_Sync();
        #endif
    }

    static void TryCopyFile(string sourcePath, string targetPath) {
        try {
            File.Copy(sourcePath, targetPath, true);
            SyncFs();

        } catch (Exception ex) {
            Debug.Log("Unable to copy file.");
            Debug.LogException(ex);
        }
    }

    static string GetStringContentFromFile(string path) {
        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
            byte[] result = new byte[fileStream.Length];
            
            // Get contents of file.
            fileStream.Read(result, 0, (int)fileStream.Length);
            return System.Text.Encoding.UTF8.GetString(result);
        }
    }

    static void WriteDataToDisk(string path, string content) {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);

        // Write content buffer to disk. If file does not exist, it will be created; Otherwise it will be overwritten.
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write)) {
            fileStream.Write(buffer, 0, buffer.Length);
            SyncFs();
        }
    }

    /*
    Opens a file stream with a given path parameter and reads the contents of the file.
    Uses JsonUtility to deserialize the string contents of the file into a CourseSaveData
    object. If string contents are null/empty will return a CourseSaveData instance with
    default values, as this would normally deserialize to a null value. If an exception
    is encountered during this process then null is returned to signal an error has occured.
    */
    static CourseSaveData TryDeserializeCourseSaveData(string path) {
        try {
            // Get contents of file.
            string dataString = GetStringContentFromFile(path);

            // Handle empty file content;
            if (String.IsNullOrEmpty(dataString)) {
                return new CourseSaveData();
            }

            return JsonUtility.FromJson<CourseSaveData>(dataString);
        } catch (Exception ex) {
            Debug.Log("Exception encountered while loading course save data.");
            Debug.LogException(ex);    

            return null;
        }
    }

    static SettingsSaveData TryDeserializeSettingsSaveData(string path) {
        try {
            // Get contents of file.
            string dataString = GetStringContentFromFile(path);

            // Handle empty file content;
            if (String.IsNullOrEmpty(dataString)) {
                return new SettingsSaveData();
            }

            return JsonUtility.FromJson<SettingsSaveData>(dataString);
        } catch (Exception ex) {
            Debug.Log("Exception encountered while loading course save data.");
            Debug.LogException(ex);    

            return null;
        }
    }

    public static bool TryValidateCourseSaveDirectory() {
        try {
            if (!Directory.Exists(PersistentDataConfig.CourseDataPath)) {
                Directory.CreateDirectory(PersistentDataConfig.CourseDataPath);
            }
            return true;
        } catch (Exception ex) {
            Debug.LogException(ex);
            Debug.Log("Unable to create save directory.");
            return false;
        }
    }

    public static bool TryValidateRootDirectory() {
        try {
            if (!Directory.Exists(PersistentDataConfig.RootPath)) {
                Directory.CreateDirectory(PersistentDataConfig.RootPath);
            }
            return true;    
        } catch (Exception ex) {
            Debug.LogException(ex);
            Debug.Log("Unable to create root directory.");
            return false;
        }
    }

    public static bool TrySaveCourseData(CourseData courseData) {
        string saveFilePath = PersistentDataConfig.GetCourseSaveDataPath(courseData.Slug);
        string saveBackupFilePath = PersistentDataConfig.GetCourseSaveDataBackupPath(courseData.Slug);
        string jsonSaveData = JsonUtility.ToJson(courseData.ToCourseSaveData());

        if (File.Exists(saveFilePath)) {
            Debug.Log("Save file already exists; Backing up save file in preparation for save.");
            TryCopyFile(saveFilePath, saveBackupFilePath);
        }

        try {
            WriteDataToDisk(saveFilePath, jsonSaveData);

            Debug.Log("Data saved; Backing up save file.");
            TryCopyFile(saveFilePath, saveBackupFilePath);

            return true;
        } catch (Exception ex) {
            Debug.Log("Exception encountered while saving course data.");
            Debug.LogException(ex);
            return false;
        }
    }

    public static CourseSaveData TryLoadCourseData(string courseSlug) {
        string saveFilePath = PersistentDataConfig.GetCourseSaveDataPath(courseSlug);
        string saveBackupFilePath = PersistentDataConfig.GetCourseSaveDataBackupPath(courseSlug);        
        
        // if there is no save or backup save found for a course slug; return default save data.
        if (!File.Exists(saveFilePath) && !File.Exists(saveBackupFilePath)) {
            Debug.Log("No save or backup found for course: " + courseSlug);
            return new CourseSaveData();
        }

        // if there is only a backup save file then attempt to read that.
        if (!File.Exists(saveFilePath) && File.Exists(saveBackupFilePath)) {
            Debug.Log("Couldn't find save but found backup. Using backup file for course: " + courseSlug);
            return TryDeserializeCourseSaveData(saveBackupFilePath);
        }

        Debug.Log("Loading save for course: " + courseSlug);
        CourseSaveData result = TryDeserializeCourseSaveData(saveFilePath);

        // if save file could not be read and a backup exists, attempt to read from the backup.
        if (result == null && File.Exists(saveBackupFilePath)) {
            Debug.Log("Unable to read save data for course: " + courseSlug + "; Trying backup file.");
            return TryDeserializeCourseSaveData(saveBackupFilePath);
        }

        // return resulting course save data instance.
        return result;
    }

    public static bool TrySaveSettingsData(SettingsData settingsData) {
        string settingsFilePath = PersistentDataConfig.SettingsSaveDataPath;
        string settingsBackupFilePath = PersistentDataConfig.SettingsSaveDataBackupPath;
        string jsonSaveData = JsonUtility.ToJson(settingsData.ToSettingsSaveData());

        if (File.Exists(settingsFilePath)) {
            Debug.Log("Settings file already exists; Backing up settings file in preparation for save.");
            TryCopyFile(settingsFilePath, settingsBackupFilePath);
        }

        try {
            WriteDataToDisk(settingsFilePath, jsonSaveData);
            
            Debug.Log("Data saved; Backing up settings file.");
            TryCopyFile(settingsFilePath, settingsBackupFilePath);

            return true;
        } catch (Exception ex) {
            Debug.Log("Exception encountered while saving settings data.");
            Debug.LogException(ex);
            return false;
        }
    }

    public static SettingsSaveData TryLoadSettingsData() {
        string settingsFilePath = PersistentDataConfig.SettingsSaveDataPath;
        string settingsBackupFilePath = PersistentDataConfig.SettingsSaveDataBackupPath;

        // if there is no save or backup save found for a course slug; return default save data.
        if (!File.Exists(settingsFilePath) && !File.Exists(settingsBackupFilePath)) {
            Debug.Log("No save or backup found for settings.");
            return new SettingsSaveData();
        }

        // if there is only a backup save file then attempt to read that.
        if (!File.Exists(settingsFilePath) && File.Exists(settingsBackupFilePath)) {
            Debug.Log("Couldn't find save but found backup. Using backup file for settings.");
            return TryDeserializeSettingsSaveData(settingsBackupFilePath);
        }

        Debug.Log("Loading saved settings.");
        SettingsSaveData result = TryDeserializeSettingsSaveData(settingsFilePath);

        // if save file could not be read and a backup exists, attempt to read from the backup.
        if (result == null && File.Exists(settingsBackupFilePath)) {
            Debug.Log("Unable to read settings save data; Trying backup file.");
            return TryDeserializeSettingsSaveData(settingsBackupFilePath);
        }

        // return resulting course save data instance.
        return result;
    }
}
