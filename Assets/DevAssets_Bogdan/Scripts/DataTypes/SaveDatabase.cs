using System.Collections.Generic;

[System.Serializable]
public class SaveDatabase<T>
{
    public List<T> serializedList;

    public SaveDatabase(List<T> listToSerialize)
    {
        serializedList = listToSerialize;
    }
}
