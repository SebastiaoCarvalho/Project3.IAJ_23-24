using System.IO;
using Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN;
using UnityEngine;

public class NeuralNetworkSerializer {
    public NeuralNetwork neuralNetwork;
    public string fileName = "neuralnetwork.json";

    public void Save() {
        string json = SerializeNeuralNetwork();
        File.WriteAllText(Application.dataPath + "/" + fileName, json);
    }

    /* public void Load() {
        try {
            string json = File.ReadAllText(Application.dataPath + "/" + fileName);
            neuralNetwork = new NeuralNetwork(DeserealizeNeuralNetwork(json));
        }
        catch (FileNotFoundException e) {
            Debug.LogError("File not found : " + e.Message);
        }
    } */

    private string SerializeNeuralNetwork() {
        string json = "[\n";
        int tabCount = 1;
        foreach (Layer layer in neuralNetwork.Layers) {
            json += new string('\t', tabCount) + "{\n";
            tabCount++;
            json += new string('\t', tabCount) + "\"Weights\": [\n";
            tabCount++;
            var weights = layer.Weights;
            for (int i = 0; i < weights.GetLength(0); i++) {
                json += new string('\t', tabCount) + "[";
                for (int j = 0; j < weights.GetLength(1); j++) {
                    json += weights[i, j].ToString().Replace(",", ".") + ",";
                }
                json = json[..^1] + "],\n";
            }
            json = json[..^2] + "\n";
            tabCount--;
            json += new string('\t', tabCount) + "],\n";
            json += new string('\t', tabCount) + "\"Bias\": [\n";
            tabCount++;
            var bias = layer.Bias;
            for (int i = 0; i < bias.Length; i++) {
                json += new string('\t', tabCount) + bias[i].ToString().Replace(",", ".") + ",\n";
            }
            json = json[..^2] + "\n";
            tabCount--;
            json += new string('\t', tabCount) + "]\n";
            tabCount--;
            json += new string('\t', tabCount) + "},\n";
        }
        json = json[..^2] + "\n";
        json += "]";
        return json;
    }
}