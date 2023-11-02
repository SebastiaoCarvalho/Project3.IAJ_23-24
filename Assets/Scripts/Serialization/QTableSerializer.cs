using System.Collections.Generic;
using System.IO;
using Assets.Scripts.IAJ.Unity.DecisionMaking.RL;
using UnityEngine;
using System.Globalization;

public class QTableSerializer
{
    public QTable qTable;
    public string fileName = "qtable.json";

    public void Save()
    {
        string json = SerializeQTable();
        File.WriteAllText(Application.dataPath + "/" + fileName, json);
    }

    public void Load()
    {
        try {
            string json = File.ReadAllText(Application.dataPath + "/" + fileName);
            qTable = new QTable(DeserealizeQTable(json));
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError("File not found : " + e.Message);
        }
    }

    private string SerializeQTable() {
        Dictionary<string, Dictionary<string, float>> qValues = qTable.GetQValues();
        int tabCount = 1;
        string json = "{\n";
        foreach (KeyValuePair<string, Dictionary<string, float>> state in qValues) {
            json += new string('\t', tabCount) + "\"" + state.Key + "\":{\n";
            tabCount++;
            foreach (KeyValuePair<string, float> action in state.Value) {
                json += new string('\t', tabCount) + "\"" + action.Key + "\":" + action.Value.ToString().Replace(",", ".") + ",\n";
            }
            tabCount--;
            json = json[..^2] + "\n";
            json += new string('\t', tabCount) + "},\n";
        }
        json = json[..^2] + "\n";
        json += "}";
        return json;
    }

    private Dictionary<string, Dictionary<string, float>> DeserealizeQTable(string json) {
        Dictionary<string, Dictionary<string, float>> qValues = new Dictionary<string, Dictionary<string, float>>();
        string[] lines = json.Split('\n');
        int tabCount = 0;
        string state = "";
        foreach (string line in lines) {
            if (line.Contains("{")) {
                tabCount++;
                string trimmed = line.Trim();
                if (trimmed != "{") {
                    string key = trimmed.Substring(0, trimmed.Length - 2).Trim('"');
                    state = key;
                    qValues.Add(key, new Dictionary<string, float>());
                }
            }
            else if (line.Contains("}")) {
                tabCount--;
            }
            else {
                string[] keyValue = line.Split(':');
                if (keyValue.Length == 2) {
                    if (keyValue[0].Contains("\"")) {
                        string key = keyValue[0].Trim().Trim('"');
                        if (tabCount == 2) {
                            string action = key;
                            float value = float.Parse(keyValue[1].Trim().Trim(','), CultureInfo.InvariantCulture);
                            qValues[state].Add(action, value);
                        }
                    }
                }
            }
        }
        return qValues;
    }
}
