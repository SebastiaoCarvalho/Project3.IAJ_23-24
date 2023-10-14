using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Game;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 2;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModelImproved InitialWorldModel { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModelImproved[] Models { get; set; }
        private Action[] LevelAction { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModelImproved CurrentStateWorldModelImproved, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Goals = goals;
            this.InitialWorldModel = CurrentStateWorldModelImproved;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModelImproved[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.LevelAction = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {

            var processedActions = 0;

            var startTime = Time.realtimeSinceStartup;
            while (this.CurrentDepth >= 0)
            {
                if (processedActions >= this.ActionCombinationsProcessedPerFrame) {
                    this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
                    return null;
                }
                if (this.CurrentDepth >= MAX_DEPTH) {
                    var currentValue = Models[CurrentDepth].CalculateDiscontentment(Goals);
                    TotalActionCombinationsProcessed++;
                    if (currentValue < BestDiscontentmentValue) {
                        BestDiscontentmentValue = currentValue;
                        this.BestAction = LevelAction[0];
                        for (int i = 0; i < MAX_DEPTH; i++) {
                            if (LevelAction[i] == null) break;
                            BestActionSequence[i] = LevelAction[i];
                        }
                    }
                    CurrentDepth--;
                    continue;
                }
                var nextAction = Models[CurrentDepth].GetNextAction();
                if (nextAction != null) {
                    Models[CurrentDepth + 1] = Models[CurrentDepth].GenerateChildWorldModel();
                    nextAction.ApplyActionEffects(Models[CurrentDepth + 1]);
                    processedActions++;
                    if ((int) Models[CurrentDepth + 1].GetProperty(Properties.HP) <= 0) 
                        continue;
                    if ((float) Models[CurrentDepth + 1].GetProperty(Properties.TIME) >= 150) 
                        continue;
                    LevelAction[CurrentDepth] = nextAction;
                    CurrentDepth++;
                }
                else {
                    CurrentDepth--;
                }
            }
            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            this.InProgress = false;
            return this.BestAction;
        }
    }
}
