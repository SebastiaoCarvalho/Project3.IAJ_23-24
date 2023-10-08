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
        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base (currentStateWorldModel)
        {
            this.MaxIterations = 1000;
            this.MaxIterationsPerFrame = 10;
            this.MaxPlayoutIterations = 1;
        }

        protected override float Playout(WorldModel initialStateForPlayout)
        {
            // is terminal is always for player, getScore is always from player perspective
            // playout and return a result for initialStateForPlayout

            CurrentDepth = 0;
            var currentState = initialStateForPlayout;
            Action[] executableActions = currentState.GetExecutableActions();

            while (!currentState.IsTerminal())
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
        
        private double[] NormalizeWeights(Action[] items, WorldModel worldModel)
        {
            //Math.Exp(items.GetHValue(worldModel))
            double[] normalizedWeights = new double[items.Length];

            double totalWeight = items.Select(item => Math.Exp(-item.GetHValue(worldModel))).Sum();

            double lowerProbability = 0.0d;
            //Console.WriteLine("Normalization Debug");
            for (int i = 0; i < items.Length; i++)
            {
                double normalizedWeight = Math.Exp(-items[i].GetHValue(worldModel))/totalWeight;
                normalizedWeights[i] = normalizedWeight;
                //item.NormalizedWeight = item.GetHValue(worldModel) / totalWeight;
                //Console.WriteLine($"{item.Name} [{lowerProbability}-{lowerProbability + item.NormalizedWeight})");
                lowerProbability += normalizedWeight;
            }

            return normalizedWeights;
        }

        private Action RandomChoose(Action[] items, double[] weights, WorldModel worldModel)
        {
            double eventProbability = RandomGenerator.NextDouble();
            //Console.WriteLine("Extraction Debug : pEvent = " + eventProbability);

            double lowerProbability = 0.0d;
            Action candidate = null;
            for (int i = 0; i < items.Length; i++)
            {
                if (lowerProbability <= eventProbability && eventProbability < (lowerProbability + weights[i]))
                {
                    candidate = items[i];
                    break;
                }
                lowerProbability += weights[i];
            }

            if (candidate == null)
            {
                //you should never land here, but maybe rounding error may be unexpected, anyway if you are here you meant to choose the last peace
                //Console.WriteLine("Item not choosed");
                candidate = items.Last();
            }

            //Console.WriteLine("Extraction Debug : Item selected -> " + candidate.Name);
            return candidate;
        }

    }

}
