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
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsPerFrame { get; set; }
        public int MaxPlayoutIterations { get; set; }
        public int MaxPlayoutDepthReached { get; set; }
        public int MaxSelectionDepthReached { get; set; }
        public float TotalProcessingTime { get; set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<Action> BestActionSequence { get; set; }
        public WorldModelImproved BestActionSequenceEndState { get; set; }
        protected int CurrentIterations { get; set; }
        protected int CurrentDepth { get; set; }
        protected int FrameCurrentIterations { get; set; }
        protected CurrentStateWorldModelImproved InitialState { get; set; }
        protected MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }
        protected int PlayoutDepthLimit { get; set; }

        public MCTS(CurrentStateWorldModelImproved CurrentStateWorldModelImproved)
        {
            this.InProgress = false;
            this.InitialState = CurrentStateWorldModelImproved;
            this.MaxIterations = 1000;
            this.MaxIterationsPerFrame = 500;
            this.MaxPlayoutIterations = 10;
            this.RandomGenerator = new System.Random();
            this.PlayoutDepthLimit = 10;
        }


        public void InitializeMCTSearch()
        {
            this.InitialState.Initialize();
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.FrameCurrentIterations = 0;
            this.TotalProcessingTime = 0.0f;
 
            // create root node n0 for state s0
            this.InitialNode = new MCTSNode(this.InitialState)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<Action>();
        }

        public Action ChooseAction()
        {
            MCTSNode selectedNode;
            float reward;

            var startTime = Time.realtimeSinceStartup;
            FrameCurrentIterations = 0;
            var CurrentPlayoutIterations = 0;
            while (CurrentIterations <= MaxIterations)
            {
                if (FrameCurrentIterations > MaxIterationsPerFrame)
                {
                    this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
                    return null;
                }
                selectedNode = Selection(InitialNode);
                CurrentPlayoutIterations = 0;
                while (CurrentPlayoutIterations++ < MaxPlayoutIterations) {
                    reward = Playout(selectedNode.State.GenerateChildWorldModel());
                    Backpropagate(selectedNode, reward);
                }

                CurrentIterations++;
                FrameCurrentIterations++;
            }

            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            this.InProgress = false;

            return BestAction(InitialNode);
        }

        // Selection and Expantion
        protected MCTSNode Selection(MCTSNode initialNode)
        {
            // select a node, if not all expanded expand, otherwise choose best
            Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild;

            CurrentDepth = 0;
            while (!currentNode.State.IsTerminal()) {
                nextAction = currentNode.State.GetNextAction();
                if (nextAction != null)
                {
                    return this.Expand(currentNode, nextAction);
                }
                bestChild = BestUCTChild(currentNode);
                currentNode = bestChild;
                CurrentDepth++;
            }

            if (CurrentDepth > MaxSelectionDepthReached)
            {
                MaxSelectionDepthReached = CurrentDepth;
            }

            return currentNode;
        }

        protected virtual float Playout(WorldModelImproved initialStateForPlayout)
        {
            // is terminal is always for player, getScore is always from player perspective
            // playout and return a result for initialStateForPlayout

            CurrentDepth = 0;
            var currentState = initialStateForPlayout;
            Action[] executableActions = currentState.GetExecutableActions();

            while (!currentState.IsTerminal() && CurrentDepth < PlayoutDepthLimit)
            {
                var index = RandomGenerator.Next(executableActions.Length);
                executableActions[index].ApplyActionEffects(currentState);
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

        protected virtual void Backpropagate(MCTSNode node, float reward)
        {
            var currentNode = node;

            while (currentNode != null) {
                if (currentNode.Parent == null || currentNode.Parent.PlayerID == 0)
                {
                    currentNode.Q += reward;
                }
                else
                {
                    currentNode.Q += 1 - reward;
                }
                currentNode.N++;
                currentNode = currentNode.Parent;
            }
        }

        protected MCTSNode Expand(MCTSNode parent, Action action)
        {
            // here we create a new node from applying the action to the parent and do playout
            WorldModelImproved newState = parent.State.GenerateChildWorldModel();
            action.ApplyActionEffects(newState);
            newState.CalculateNextPlayer();
            MCTSNode newNode = new MCTSNode(newState) {
                Parent = parent,
                Action = action,
                PlayerID = newState.GetNextPlayer(),
                N = 0,
                Q = 0
            };

            parent.ChildNodes.Add(newNode);

            return newNode;
        }

        protected virtual MCTSNode BestUCTChild(MCTSNode node)
        {
            // go for each child and check which value is best
            List<MCTSNode> childNodes = node.ChildNodes;
            MCTSNode bestChild = null;
            double bestChildValue = - 1;
            double childValue;

            foreach (MCTSNode child in childNodes)
            {
                childValue = child.N != 0 ? child.Q/child.N + C * Math.Sqrt(Math.Log(node.N)/child.N) : 0;

                if (childValue > bestChildValue)
                {
                    bestChild = child;
                    bestChildValue = childValue;
                }
            }
            return bestChild;
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        protected MCTSNode BestChild(MCTSNode node)
        {
            List<MCTSNode> childNodes = node.ChildNodes;
            MCTSNode bestChild = null;
            float bestChildWinRate = -1;
            float childWinRate;

            foreach (MCTSNode child in childNodes)
            {
                childWinRate = child.N != 0 ? child.Q/child.N : 0;
                if (childWinRate > bestChildWinRate)
                {
                    bestChild = child;
                    bestChildWinRate = childWinRate;
                }
            }

            return bestChild;
        }


        protected Action BestAction(MCTSNode node)
        {
            var bestChild = this.BestChild(node);
            if (bestChild == null) return null;

            this.BestFirstChild = bestChild;
           /*  Debug.Log("-----------------------------------------------------------------------------------------------------------");
            Debug.Log("Best child: " + bestChild.Action.Name + " with winrate: " + bestChild.Q / bestChild.N);
            Debug.Log("Next bro : " + bestChild.State.GetNextPlayer());
            PlayoutWithDebug(bestChild.State.GenerateChildWorldModel()); */

            //this is done for debugging proposes only
            this.BestActionSequence = new List<Action>();
            this.BestActionSequence.Add(bestChild.Action);
            node = bestChild;
            this.BestActionSequenceEndState = node.State;

            while(!node.State.IsTerminal())
            {
                bestChild = this.BestChild(node);
                if (bestChild == null) {
                    break;
                }
                this.BestActionSequence.Add(bestChild.Action);
                node = bestChild;
                this.BestActionSequenceEndState = node.State;
            }
            return this.BestFirstChild.Action;
        }

        protected virtual float PlayoutWithDebug(WorldModelImproved WorldModelImproved) {
            return 0.0f;
        }

    }
}
