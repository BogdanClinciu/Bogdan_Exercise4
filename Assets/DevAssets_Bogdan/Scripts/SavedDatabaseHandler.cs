using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedDatabaseHandler
{
    private static string persistentPath = Application.persistentDataPath;
	private const string itemDBPath = "/itemDatabase.json";
	private const string historyDBpath = "/orderDatabase.json";

    private const string NO_VALUE_ERROR = "Description not found";
    private const string WRITE_FAILIURE = "WRITE FALIURE!";

    private static TextAsset DefaultItemDB;
    private static TextAsset DefaultHistoryDB;

    private delegate void GetPointers();
    private static GetPointers OnGetPointers;


    ///<summary>
    ///Convets the curently loaded database to a SaveData object and writes it to the persistent data path.
    ///</summary>
    public static void SaveDatabase<T>(List<T> datalist)
    {
		string contents = JsonUtility.ToJson(new SaveDatabase<T>(datalist), true);
		try
        {
            System.IO.File.WriteAllText (FilePath<T>(), contents);
        }
        catch (System.Exception e)
        {
            Debug.Log(WRITE_FAILIURE + e.Message);
        }
	}

    ///<summary>
    ///Loads a SaveData type from the persistent data path and convets it to a Dictionary of string string, if loading fails we load the default database and save that.
    ///</summary>
    public static void LoadDatabase<T>(out List<T> loadedData)
    {
        LoadDefaluts();
        loadedData = null;

        if (System.IO.File.Exists(FilePath<T>()))
        {
            try
            {
				loadedData = JsonUtility.FromJson<SaveDatabase<T>>(System.IO.File.ReadAllText(FilePath<T>())).serializedList;
			}
            catch (System.Exception ex)
            {
                Debug.Log (ex.Message);
            }
        }
        else
        {
            try
            {
                // TODO: Add a "Unable to find word database file"  UI message, with the posibility to load defaults or exit the application (also anounce the location of the database json)
                Debug.Log("Unable to find " + typeof(T).ToString() + " database file, writing default database");
                loadedData = JsonUtility.FromJson<SaveDatabase<T>>(DefaultDB<T>().ToString()).serializedList;
                SaveDatabase<T>(loadedData);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
	}

    ///<summary>
    ///Returns the corect database filepath for the given type T, if it exists.
    ///</summary>
    private static string FilePath<T>()
    {
        if(typeof(T) == typeof(InventoryItemInstance))
        {
            return persistentPath + itemDBPath;
        }

        if(typeof(T) == typeof(Order))
        {
            return persistentPath + historyDBpath;
        }

        return string.Empty;
    }

    ///<summary>
    ///Returns the corect default database text asset for the given type T, from the resources folder.
    ///</summary>
    private static TextAsset DefaultDB<T>()
    {
        if(typeof(T) == typeof(InventoryItemInstance))
        {
            return DefaultItemDB;
        }

        if(typeof(T) == typeof(Order))
        {
            return DefaultHistoryDB;
        }

        return null;
    }

    ///<summary>
    ///Loads the default database text asset files if they have not already been loaded.
    ///</summary>
    private static void LoadDefaluts()
    {
        if(DefaultItemDB == null)
        {
            DefaultItemDB = (TextAsset)Resources.Load("DefaultItemDB", typeof(TextAsset));
        }

        if(DefaultHistoryDB == null)
        {
            DefaultHistoryDB = (TextAsset)Resources.Load("DefaultHistoryDB", typeof(TextAsset));
        }

    }
}
