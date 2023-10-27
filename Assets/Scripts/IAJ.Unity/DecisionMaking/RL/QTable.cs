using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL
{
    [Serializable]
    public class QTable
    {
        private Dictionary<string, Dictionary<string, float>> QValues; /* Dictionary<stateName, Dictionary<actionName, QValue>> */
        public Dictionary<string, Dictionary<string, float>> GetQValues() { return QValues; }
        public QTable()
        {
            this.QValues = new Dictionary<string, Dictionary<string, float>>();
        }

        public QTable(Dictionary<string, Dictionary<string, float>> qValues)
        {
            this.QValues = qValues;
        }

        // TODO: Initialize QTable

        public float GetQValue(RLState state, Action action) {
            if (QValues.ContainsKey(state.ToString())) {
                var subTable = QValues[state.ToString()];
                if (subTable.ContainsKey(action.Name)) {
                    var QValue = subTable[action.Name];
                    return QValue;
                }
                else {
                    return 0.0f;
                }
            }
            else {
                return 0.0f;
            }
        }

        public void SetQValue(RLState state, Action action, float newValue) {
            if (QValues.ContainsKey(state.ToString())) {
                var subTable = QValues[state.ToString()];
                if (subTable.ContainsKey(action.Name)) {
                    subTable[action.Name] = newValue;
                }
                else {
                    subTable.Add(action.Name, newValue);
                }
            }
            else {
                Debug.Log("Create state " + state.ToString());
                QValues.Add(state.ToString(), new Dictionary<string, float>() { { action.Name, newValue } });
            }
        }

        public Action GetBestAction(RLState state) {
            if (!QValues.ContainsKey(state.ToString())) { 
                QValues.Add(state.ToString(), new Dictionary<string, float>());
            }
            
            var subTable = QValues[state.ToString()];
            var actions = state.GetExecutableActions();
            foreach (var action in actions) {
                if (!subTable.ContainsKey(action.Name))
                    subTable.Add(action.Name, 0.0f);
            }
            for (int i = subTable.Count - 1; i >= 0; i--) {
                string key = subTable.ElementAt(i).Key;
                if (!actions.ToList().Exists(a => a.Name == key)) {
                    subTable.Remove(key);
                }
            }
            string bestAction = null;
            float bestValue = float.MinValue;
            foreach (KeyValuePair<string, float> kvp in subTable) {
                if (kvp.Value > bestValue) {
                    bestAction = kvp.Key;
                    bestValue = kvp.Value;
                }
            }

            return actions.ToList().Find(a => a.Name == bestAction);
        }
    }
}
