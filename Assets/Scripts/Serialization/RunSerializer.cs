using System.IO;
using UnityEngine;

public class RunSerializer {
    public string fileName = "run.json";

    public void Save()
    {
        string json = SerializeGame();
        File.WriteAllText(Application.dataPath + "/" + fileName, json);
    }

    public void Load()
    {
        try {
            string json = File.ReadAllText(Application.dataPath + "/" + fileName);
            LoadGame(json);
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError("File not found : " + e.Message);
        }
    }

    private string SerializeGame() {
        GameManager gameManager = GameManager.Instance;
        int tabCount = 1;
        string json = "{\n";
        json += new string('\t', tabCount) + "\"runs\":" + gameManager.runCounter + ",\n";
        json += new string('\t', tabCount) + "\"deaths\":" + gameManager.deathCounter + ",\n";
        json += new string('\t', tabCount) + "\"timeouts\":" + gameManager.timeoutCounter + ",\n";
        json += new string('\t', tabCount) + "\"wins\":" + gameManager.winCounter + "\n";
        json += "}";
        return json;
    }

    private void LoadGame(string json) {
        GameManager gameManager = GameManager.Instance;
        string[] lines = json.Split('\n');
        foreach (string line in lines) {
            if (line.Contains("runs")) {
                string trimmed = line.Trim();
                string value = trimmed.Split(":")[1].Trim()[..^1];
                if (value.Equals("0")) gameManager.runCounter = 0;
                else gameManager.runCounter = int.Parse(value);
            }
            else if (line.Contains("deaths")) {
                string trimmed = line.Trim();
                string value = trimmed.Split(":")[1].Trim()[..^1];
                if (value.Equals("0")) gameManager.deathCounter = 0;
                else gameManager.deathCounter = int.Parse(value);
            }
            else if (line.Contains("timeouts")) {
                string trimmed = line.Trim();
                string value = trimmed.Split(":")[1].Trim()[..^1];
                if (value.Equals("0")) gameManager.timeoutCounter = 0;
                else gameManager.timeoutCounter = int.Parse(value);
            }
            else if (line.Contains("wins")) {
                string trimmed = line.Trim();
                string value = trimmed.Split(":")[1].Trim();
                if (value.Equals("0")) gameManager.winCounter = 0;
                else gameManager.winCounter = int.Parse(value);
            }
        }
    }
}