using System.Collections.Generic;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL
{
    public class QTable
    {
        private Dictionary<RLState, Dictionary<Action, float>> QValues { get; set; } 

        public QTable()
        {
            this.QValues = new Dictionary<RLState, Dictionary<Action, float>>();
        }

        // TODO: Initialize QTable

        public float GetQValue(RLState state, Action action) {
            if (QValues.ContainsKey(state)) {
                var subTable = QValues[state];
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
            if (QValues.ContainsKey(state)) {
                var subTable = QValues[state];
                if (subTable.ContainsKey(action)) {
                    subTable[action] = newValue;
                }
                else {
                    subTable.Add(action, newValue);
                }
            }
            else {
                QValues.Add(state, new Dictionary<Action, float>() { { action, newValue } });
            }
        }

        public Action GetBestAction(RLState state) {
            if (!QValues.ContainsKey(state)) {
                // we have a problem
                return null;
            }
            else {
                var subTable = QValues[state];
                Action bestAction = null;
                float bestValue = float.MinValue;
                foreach (KeyValuePair<Action, float> kvp in subTable) {
                    if (kvp.Value > bestValue) {
                        bestAction = kvp.Key;
                        bestValue = kvp.Value;
                    }
                }

                return bestAction;
            }


        }
    }
}
