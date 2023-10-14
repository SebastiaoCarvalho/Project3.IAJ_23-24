using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEditor.Animations;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {
        public MCTSBiasedPlayout(CurrentStateWorldModelImproved CurrentStateWorldModelImproved) : base (CurrentStateWorldModelImproved)
        {
            this.MaxIterations = 10000;
            this.MaxIterationsPerFrame = 1000;
            this.MaxPlayoutIterations = 1;
            this.PlayoutDepthLimit = 5;
        }
        
        protected override float Playout(WorldModelImproved initialStateForPlayout)
        {
            CurrentDepth = 0;
            var currentState = initialStateForPlayout;
            currentState.CalculateNextPlayer();
            Action[] executableActions = currentState.GetExecutableActions();

            while (!currentState.IsTerminal() && CurrentDepth < PlayoutDepthLimit)
            {
                double[] weights = NormalizeWeights(executableActions, currentState);
                Action executableAction = RandomChoose(executableActions, weights, currentState);
                executableAction.ApplyActionEffects(currentState);
                currentState.CalculateNextPlayer();
                executableActions = currentState.GetExecutableActions();
                CurrentDepth++;
            }

            if (CurrentDepth > MaxPlayoutDepthReached)
            {
                MaxPlayoutDepthReached = CurrentDepth;
            }

            return currentState.GetScore();
        }

        private double[] NormalizeWeights(Action[] actions, WorldModelImproved WorldModelImproved)
        {
            double[] normalizedWeights = new double[actions.Length];

            double totalWeight = actions.Select(action => Math.Exp(-action.GetHValue(WorldModelImproved))).Sum();

            double lowerProbability = 0.0d;
            for (int i = 0; i < actions.Length; i++)
            {
                double normalizedWeight = Math.Exp(-actions[i].GetHValue(WorldModelImproved))/totalWeight;
                normalizedWeights[i] = normalizedWeight;
                lowerProbability += normalizedWeight;
            }

            return normalizedWeights;
        }

        private Action RandomChoose(Action[] actions, double[] weights, WorldModelImproved WorldModelImproved)
        {
            double eventProbability = RandomGenerator.NextDouble();

            double lowerProbability = 0.0d;
            Action candidate = null;
            for (int i = 0; i < actions.Length; i++)
            {
                if (lowerProbability <= eventProbability && eventProbability < (lowerProbability + weights[i]))
                {
                    candidate = actions[i];
                    break;
                }
                lowerProbability += weights[i];
            }
            
            if (candidate == null)
            {
                candidate = actions.Last();
            }

            return candidate;
        }

        private Action RandomChooseWithDebug(Action[] actions, double[] weights, WorldModelImproved WorldModelImproved)
        {
            double eventProbability = RandomGenerator.NextDouble();

            double lowerProbability = 0.0d;
            Action candidate = null;
            foreach(Action action in actions)
            {
                if (action.Name.Contains("EnemyAttack"))
                    Debug.Log("Action: " + action.Name + " with probability: " + Math.Exp(-action.GetHValue(WorldModelImproved)));
            }
            for (int i = 0; i < actions.Length; i++)
            {
                if (lowerProbability <= eventProbability && eventProbability < (lowerProbability + weights[i]))
                {
                    candidate = actions[i];
                    Debug.Log("Chosen action: " + candidate.Name + " with probability: " + weights[i]);
                    break;
                }
                lowerProbability += weights[i];
            }
            if (candidate == null)
            {
                candidate = actions.Last();
            }

            return candidate;
        }

        protected override float PlayoutWithDebug(WorldModelImproved initialStateForPlayout)
        {
            CurrentDepth = 0;
            var currentState = initialStateForPlayout;
            currentState.CalculateNextPlayer();
            Action[] executableActions = currentState.GetExecutableActions();
            Debug.Log(initialStateForPlayout.GetProperty(Properties.POSITION));
            while (!currentState.IsTerminal() && CurrentDepth < PlayoutDepthLimit)
            {
                double[] weights = NormalizeWeights(executableActions, currentState);
                Action executableAction = RandomChooseWithDebug(executableActions, weights, currentState);
                executableAction.ApplyActionEffects(currentState);
                currentState.CalculateNextPlayer();
                if (executableAction.Name.Contains("EnemyAttack"))
                {
                    Debug.Log(executableAction.Name);
                    Debug.Log(currentState.IsTerminal());
                    Debug.Log(currentState.GetProperty(Properties.HP));
                    Debug.Log(currentState.GetScore());
                }
                executableActions = currentState.GetExecutableActions();
                CurrentDepth++;
            }

            if (CurrentDepth > MaxPlayoutDepthReached)
            {
                MaxPlayoutDepthReached = CurrentDepth;
            }

            return currentState.GetScore();
        }
    }
}
