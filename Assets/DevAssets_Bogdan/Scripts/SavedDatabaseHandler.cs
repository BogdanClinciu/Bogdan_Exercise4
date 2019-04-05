using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedDatabaseHandler
{
    private static string persistentPath = Application.persistentDataPath;
	private const string itemDBPath = "/itemDatabase.json";
	private const string orderDBpath = "/orderDatabase.json";
	private const string historyDBpath = "/orderDatabase.json";

    private const string NO_VALUE_ERROR = "Description not found";
    private const string WRITE_FAILIURE = "WRITE FALIURE!";

    private static TextAsset DefaultItemDB;
    private static TextAsset DefaultOrderDB;
    private static TextAsset DefaultHistoryDB;

    private delegate void GetPointers();
    private static GetPointers OnGetPointers;


    ///<summary>
    ///Convets the curently loaded database to a SaveData object and writes it to the persistent data path.
    ///</summary>
    public static void SaveDatabase<T>(List<T> datalist, bool isOrderHistory)
    {
		string contents = JsonUtility.ToJson(new SaveDatabase<T>(datalist), true);
        Debug.Log(FilePath<T>(false));
		try
        {
            System.IO.File.WriteAllText (FilePath<T>(isOrderHistory), contents);
        }
        catch (System.Exception e)
        {
            Debug.Log(WRITE_FAILIURE + e.Message);
        }
	}

    ///<summary>
    ///Loads a SaveData type from the persistent data path and convets it to a Dictionary of string string, if loading fails we load the default database and save that.
    ///</summary>
    public static void LoadDatabase<T>(out List<T> loadedData, bool isOrderHistory)
    {
        LoadDefaluts();
        loadedData = null;

        if (System.IO.File.Exists(FilePath<T>(isOrderHistory)))
        {
            try
            {
				loadedData = JsonUtility.FromJson<SaveDatabase<T>>(System.IO.File.ReadAllText(FilePath<T>(isOrderHistory))).serializedList;
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
                Debug.Log("Unable to find word database file, writing default database");
                loadedData = JsonUtility.FromJson<SaveDatabase<T>>(DefaultDB<T>(isOrderHistory).ToString()).serializedList;
                SaveDatabase<T>(loadedData, isOrderHistory);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
	}

    private static string FilePath<T>(bool isOrderHistory)
    {
        if(typeof(T) == typeof(InventoryItemInstance))
        {
            return persistentPath + itemDBPath;
        }

        if(typeof(T) == typeof(Order))
        {
            return (isOrderHistory) ? persistentPath + historyDBpath : persistentPath + orderDBpath;
        }

        return string.Empty;
    }

    private static TextAsset DefaultDB<T>(bool isOrderHistory)
    {
        if(typeof(T) == typeof(InventoryItemInstance))
        {
            return DefaultItemDB;
        }

        if(typeof(T) == typeof(Order))
        {
            return (isOrderHistory) ? DefaultHistoryDB : DefaultOrderDB;
        }

        return null;
    }

    private static void LoadDefaluts()
    {
        if(DefaultItemDB == null)
        {
            DefaultItemDB = (TextAsset)Resources.Load("ItemDB", typeof(TextAsset));
        }

        if(DefaultOrderDB == null)
        {
            DefaultOrderDB = (TextAsset)Resources.Load("OrderDB", typeof(TextAsset));
        }

        if(DefaultHistoryDB == null)
        {
            DefaultHistoryDB = (TextAsset)Resources.Load("HistoryDB", typeof(TextAsset));
        }
    }
}
