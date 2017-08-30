using System;
using System.IO;
using UnityEngine;

public static class SettingsManager
{
    public enum Language
    {
        Spanish,
        English
    }

    public enum Gender
    {
        Male,
        Female
    }

    //Values by Default
    private static string userName= "Logan";
    private static bool isMale= true;
    private static Gender userGender= Gender.Male;
    private static Language userLanguage= Language.English;

    private static string settingsFileName ="SettingsFile.txt";
    private static string settingsFilePath;

    private static bool wasDataLoaded = false;//To avoid reloading data when no needing

    #region Getters and Setters

    public static string UserName
    {
        get {  return userName; }
        set {  userName = value; }
    }    
    public static bool IsMale
    {
        get { return isMale; }
        set { isMale = value; }
    }
    public static Language UserLanguage
    {
        get { return userLanguage; }
        set { userLanguage = value; }
    }

    public static Gender UserGender
    {
        get{ return userGender; }
        set{ userGender = value; }
    }

    public static bool WasDataLoaded
    {
        get{ return wasDataLoaded; }
        set{ wasDataLoaded = value; }
    }

    #endregion

    /// <summary>
    /// Set the Path directory where the settings file is, depending on execution platform: Android or Editor
    /// </summary>
    private static void setSettingsFilePath()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        settingsFilePath = Application.persistentDataPath + "/" + settingsFileName;
#elif UNITY_EDITOR
        settingsFilePath = Application.dataPath + "/Resources/Files/" + settingsFileName;
#endif
    }

    /// <summary>
    /// Load settings from the settings file
    /// </summary>
    public static void loadSettings()
    {
        setSettingsFilePath();
        MapEditor.setWorldsFilesPath();
        if (File.Exists(settingsFilePath))//Load previous settings
        {
            loadInfoFromFile();
        }
        else//create the settings file with the default value. (To creae the file when first installed, or if deleted
        {
            createSettingsFile();
        }
        //Reading a txt file inside resources folder of the roject un adroid build (Android)

        //TextAsset level = Resources.Load<TextAsset>("Files/SettingsFile");
        //using(StreamReader sr= new StreamReader(new MemoryStream(level.bytes)))
        //{
        //    string line="";
        //    while (!sr.EndOfStream)
        //    {
        //        line = sr.ReadLine();
        //    }
        //    Debug.Log(line);
        //}
    }

    /// <summary>
    /// Creates the Settings file
    /// </summary>
    private static void createSettingsFile()
    {
        try { 
            StreamWriter sw = new StreamWriter(settingsFilePath);
            sw.WriteLine("Language:" + userLanguage.ToString());
            sw.WriteLine("Gender:" + userGender.ToString());
            sw.WriteLine("Name:" + userName);
            sw.Close();
        }
        catch(Exception e)
        {
            Message.showMessageText(e.ToString());
        }
    }

    /// <summary>
    /// Loads data from Settings File
    /// </summary>
    private static void loadInfoFromFile()
    {
        try
        {
            StreamReader sr = new StreamReader(settingsFilePath);
            string line = sr.ReadLine();
            userLanguage = stringToLanguage(line.Split(':')[1]);
            line = sr.ReadLine();
            userGender = stringToGender(line.Split(':')[1]);
            line = sr.ReadLine();
            userName = line.Split(':')[1];
            sr.Close();
        }
        catch(Exception e)
        {
            Message.showMessageText(e.ToString());
        }
    }

    /// <summary>
    /// Saves(Rewrite) new settings on the settings file
    /// </summary>
    public static void saveSettings()
    {
        //to delete the last info
        if(File.Exists(settingsFilePath))
        {
            //File.Delete(Application.dataPath + "/Resources/Files/" + settingsFileName);
            File.Delete(settingsFilePath);
        }
        //Rewrite data in file when aply button pressed
        createSettingsFile();

    }

    /// <summary>
    /// Convert the language from string to Language type from Language enum
    /// </summary>
    /// <param name="languageInString">The language in string type</param>
    /// <returns>The language in Language type from Language enum</returns>
    private static Language stringToLanguage(string languageInString)
    {
        if (languageInString.Equals(Language.English.ToString()))
        {
            return Language.English;
        }
        else
        {
            return Language.Spanish;
        }
    }

    /// <summary>
    /// Convert the gender from string to Gender type from Gender enum
    /// </summary>
    /// <param name="genderInString">The Gender in string type</param>
    /// <returns>The gender in Gender type from Gender enum</returns>
    private static Gender stringToGender(string genderInString)
    {
        if (genderInString.Equals(Gender.Female.ToString())){
            return Gender.Female;
        }
        else
        {
            return Gender.Male;
        }
    } 
}

