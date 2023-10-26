using System;
using System.Collections.Generic;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL
{
    public class QTable
    {
        private Dictionary<string, Dictionary<Action, float>> QValues { get; set; } 

        public QTable()
        {
            this.QValues = new Dictionary<string, Dictionary<Action, float>>();
        }

        // TODO: Initialize QTable

        public float GetQValue(RLState state, Action action) {
            if (QValues.ContainsKey(state.ToString())) {
                var subTable = QValues[state.ToString()];
                if (subTable.ContainsKey(action)) {
                    var QValue = subTable[action];
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
                if (subTable.ContainsKey(action)) {
                    subTable[action] = newValue;
                }
                else {
                    subTable.Add(action, newValue);
                }
            }
            else {
                Debug.Log("Create state " + state.ToString());
                QValues.Add(state.ToString(), new Dictionary<Action, float>() { { action, newValue } });
            }
        }

        public Action GetBestAction(RLState state) {
            if (!QValues.ContainsKey(state.ToString())) {
                Dictionary<Action, float> newSubTable = new Dictionary<Action, float>();
                var actions = state.GetExecutableActions();
                foreach (var action in actions) {
                    newSubTable.Add(action, 0.0f);
                }

                QValues.Add(state.ToString(), newSubTable);
            }

            var subTable = QValues[state.ToString()];
            Action bestAction = null;
            float bestValue = float.MinValue;
            foreach (KeyValuePair<Action, float> kvp in subTable) {
                if (kvp.Value > bestValue && kvp.Key.CanExecute()) {
                    bestAction = kvp.Key;
                    bestValue = kvp.Value;
                }
            }

            return bestAction;
        }
    }
}
