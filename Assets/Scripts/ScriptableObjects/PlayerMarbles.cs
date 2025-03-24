using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMarbles", menuName = "ScriptableObjects/PlayerMarbles", order = 1)]
public class PlayerMarbles : ScriptableObject
{
    public List<MarbleData> marbles = new();
    public void SavePlayerMarbles(int selectedIndex)
    {
        string filePath = Application.persistentDataPath + "/Save.json";

        string json = System.IO.File.ReadAllText(filePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        if (marbles[selectedIndex].isUnlocked)
        {
            saveData.selectedMarbleIndex = selectedIndex;
        }

        if (marbles.Count != saveData.unlockedStatuses.Count)
        {
            saveData.unlockedStatuses = new List<bool>();
            for (int i = 0; i < marbles.Count; i++)
            {
                saveData.unlockedStatuses.Add(marbles[i].isUnlocked);
            }
        }

        else
        {
            for (int i = 0; i < marbles.Count && i < saveData.unlockedStatuses.Count; i++)
            {
                saveData.unlockedStatuses[i] = marbles[i].isUnlocked;
            }
        }

        string updatedJson = JsonUtility.ToJson(saveData, true);
        System.IO.File.WriteAllText(filePath, updatedJson);
    }

    public int LoadPlayerMarbles()
    {
        string filePath = Application.persistentDataPath + "/Save.json";

        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            int selectedIndex = saveData.selectedMarbleIndex;

            for (int i = 0; i < marbles.Count && i < saveData.unlockedStatuses.Count; i++)
            {
                marbles[i].isUnlocked = saveData.unlockedStatuses[i];
            }

            return selectedIndex;
        }
        else
        {
            Debug.LogWarning("No save file found at: " + filePath);
            return 0;
        }
    }
}

[System.Serializable]
public class MarbleData
{
    public GameObject marblePrefab;
    public bool isUnlocked;
}