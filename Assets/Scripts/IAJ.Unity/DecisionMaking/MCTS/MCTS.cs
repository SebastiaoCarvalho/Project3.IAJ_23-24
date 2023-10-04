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
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<Action> BestActionSequence { get; private set; }
        public WorldModel BestActionSequenceEndState { get; private set; }
        protected int CurrentIterations { get; set; }
        protected int CurrentDepth { get; set; }
        protected int FrameCurrentIterations { get; set; }
        protected CurrentStateWorldModel InitialState { get; set; }
        protected MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }
        
        

        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.InitialState = currentStateWorldModel; //previously CurrentStateWorldModel
            this.MaxIterations = 500;
            this.MaxIterationsPerFrame = 5;
            this.RandomGenerator = new System.Random();
        }


        public void InitializeMCTSearch()
        {
            this.InitialState.Initialize();
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.FrameCurrentIterations = 0; //previously CurrentIterationsInFrame
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

            while (CurrentIterations <= MaxIterations)
            {
                // TODO: where do we see max depth reached
                if (FrameCurrentIterations > MaxIterationsPerFrame)
                {
                    this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
                    return null;
                }
                selectedNode = Selection(InitialNode);
                reward = Playout(new WorldModel(selectedNode.State));
                //Debug.Log(reward);
                Backpropagate(selectedNode, reward);

                CurrentIterations++;
                FrameCurrentIterations++;
                //Debug.Log(CurrentIterations);
            }

            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            this.InProgress = false;
            return BestAction(InitialNode);

            //while within computational budget

                //Selection + Expansion

                //Playout

                //Backpropagation

            // return best initial child
        }

        // Selection and Expantion
        protected MCTSNode Selection(MCTSNode initialNode)
        {
            // select a node, if not all expanded expand, otherwise choose best
            Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild;

            CurrentDepth = 0;

            while (!currentNode.State.IsTerminal()) { // also is next action null bc state :|
                nextAction = currentNode.State.GetNextAction();
                if (nextAction != null)
                {
                    //Debug.Log("expanding");
                    return this.Expand(currentNode, nextAction);
                }
                else if (nextAction == null) {
                    bestChild = BestUCTChild(currentNode); // this is UCT I believe
                    currentNode = bestChild;
                    CurrentDepth++;
                    //currentNode = bestChild != null ? bestChild : currentNode;
                }

            }

            //   while(!currentNode.State.IsTerminal())

                // nextAction = currentNode.State.GetNextAction();

                //Expansion

            if (CurrentDepth > MaxSelectionDepthReached)
            {
                MaxSelectionDepthReached = CurrentDepth;
            }

            return currentNode;
        }

        protected virtual float Playout(WorldModel initialStateForPlayout)
        {
            // is terminal is always for player, getScore is always from player perspective
            // playout and return a result for initialStateForPlayout
            //Debug.Log("playout");

            CurrentDepth = 0;
            var currentState = initialStateForPlayout;
            Action[] executableActions = currentState.GetExecutableActions();

            //player?

            while (!currentState.IsTerminal())
            {
                Debug.Log("hererererer");
                //var index = (int) RandomHelper.RandomBinomial(executableActions.Length-1);
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
            //ToDo, do not forget to later consider two advesary moves...
            var currentNode = node;

            while (currentNode != null) {
                if (currentNode.PlayerID == 0)
                {
                    currentNode.Q += reward;
                }
                else if (currentNode.PlayerID == 1)
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
            WorldModel newState = parent.State.GenerateChildWorldModel();
            //Debug.Log(action.Name);
            action.ApplyActionEffects(newState);
            newState.CalculateNextPlayer();
            // insert newState in the MCTS tree = newNode
            MCTSNode newNode = new MCTSNode(newState) {
                Parent = parent,
                Action = action,
                PlayerID = parent.State.GetNextPlayer(),
                N = 0,
                Q = 0
                // player?
            };

            parent.ChildNodes.Add(newNode);

            //var result = this.Playout(newState);
            // make the newState actions be this action
            //ToDo
            //Backpropagate(newNode, result);

            return newNode;
        }

        protected virtual MCTSNode BestUCTChild(MCTSNode node)
        {
            // go for each child and check which value is best
            //ToDo
            List<MCTSNode> childNodes = node.ChildNodes;
            MCTSNode bestChild = null;
            double bestChildUCT = double.MinValue;
            double childUCT;

            foreach (MCTSNode child in childNodes)
            {
                childUCT = child.N != 0 ? child.Q/child.N + C * Math.Sqrt(Math.Log(node.N)/child.N) : double.PositiveInfinity;

                if (childUCT > bestChildUCT)
                {
                    bestChild = child;
                    bestChildUCT = childUCT;
                }
            }

            return bestChild;
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        protected MCTSNode BestChild(MCTSNode node)
        {
            //ToDo
            List<MCTSNode> childNodes = node.ChildNodes;
            MCTSNode bestChild = null;
            float bestChildWinRate = float.MinValue;
            float childWinRate;
            //Debug.Log("childs");
            //Debug.Log(childNodes.Count);

            foreach (MCTSNode child in childNodes)
            {
                childWinRate = child.Q/child.N;
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

            //this is done for debugging proposes only
            this.BestActionSequence = new List<Action>();
            this.BestActionSequence.Add(bestChild.Action);
            node = bestChild;

            while(!node.State.IsTerminal()) //not doing this??
            {
                Debug.Log("In here");
                bestChild = this.BestChild(node);
                if (bestChild == null) break;
                this.BestActionSequence.Add(bestChild.Action);
                node = bestChild;
                this.BestActionSequenceEndState = node.State; // previously BestActionSequenceWorldState
            }

            return this.BestFirstChild.Action;
        }

    }
}
