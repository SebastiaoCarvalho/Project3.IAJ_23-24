using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class GOBDecisionMaking
    {
        public bool InProgress { get; set; }
        private List<Goal> goals { get; set; }
        private List<Action> actions { get; set; }

        public Dictionary<Action,float> ActionDiscontentment { get; set; }

        public Action secondBestAction;
        public Action thirdBestAction;

        // Utility based GOB
        public GOBDecisionMaking(List<Action> _actions, List<Goal> goals)
        {
            this.actions = _actions;
            this.goals = goals;
            secondBestAction = new Action("yo");
            thirdBestAction = new Action("yo too");
            this.ActionDiscontentment = new Dictionary<Action,float>();
        }


        public static float CalculateDiscontentment(Action action, List<Goal> goals, AutonomousCharacter character)
        {
            // Keep a running total
            var discontentment = 0.0f;
            var duration = action.GetDuration();
            float max = 10f;

            foreach (var goal in goals)
            {
                if (goal.Name == "BeQuick")
                    max = 150f;
                else if (goal.Name == "GetRich")
                    max = 40f;
                else
                    max = character.baseStats.Level * 10;
                
                // Calculate the new value after the action
                float changeValue = action.GetGoalChange(goal) + duration * goal.ChangeRate;
                
                // The change rate is how much the goals changes per time
                changeValue = character.NormalizeGoalValues(goal.InsistenceValue + changeValue, 0, max);
   
                discontentment += goal.GetDiscontentment(changeValue);
            }

            return discontentment;
        }

        public Action ChooseAction(AutonomousCharacter character)
        {
            // Find the action leading to the lowest discontentment
            InProgress = true;
            Action bestAction = null;
            var bestValue = float.PositiveInfinity;
            secondBestAction = null;
            thirdBestAction = null;
            ActionDiscontentment.Clear();

            float value;
            foreach (var action in actions)
            {
                if (action.CanExecute())
                {
                    value = CalculateDiscontentment(action, goals, character);
                    ActionDiscontentment.Add(action, value);
                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestAction = action;
                    }
                }
            }

            var BestActions = ActionDiscontentment.OrderBy(pair => pair.Value).Take(3).ToList();
            secondBestAction = BestActions.Count >= 2 ? BestActions[1].Key : null;
            thirdBestAction = BestActions.Count >= 3 ? BestActions[2].Key : null;
            InProgress = false;
            return bestAction;
        }
    }
}
