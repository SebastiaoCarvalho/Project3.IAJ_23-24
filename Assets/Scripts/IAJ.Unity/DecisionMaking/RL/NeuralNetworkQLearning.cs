using Random = System.Random;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL {

    public class NeuralNetworkQLearning {

        public bool InProgress { get; private set; }
         public int MaxIterations { get; set; }
        public int MaxIterationsPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        protected int CurrentIterations { get; set; }
        protected int FrameCurrentIterations { get; set; }
        private NeuralNetwork _neuralNetwork;
        private Random _randomGenerator;
        protected Action ExecutedAction;
        public OutputNode OutputNode;
        public RLState State;
        private List<Action> _actions;
        private float _epsilon;
        private float _alpha { get; set; }
        private float _gamma { get; set; }
        protected bool NewAction { get; set; }

        public NeuralNetworkQLearning(RLState state, List<Action> actions) {
            this.InProgress = false;
            NewAction = false;
            _actions = actions;
            _neuralNetwork = new NeuralNetwork(3, new List<int>{ 4, 4, actions.Count }, state.ToArray(), actions);
            _epsilon = 0.1f;
            _gamma = 0.1f;
            _randomGenerator = new Random();
            State = state;
        }

        public void InitializeQLearning()
        {
            this.CurrentIterations = 0;
            this.FrameCurrentIterations = 0;
            this.TotalProcessingTime = 0.0f;
            this.State.Reset();
        }

        public Action ChooseAction() {
            Debug.Log("ChooseAction");
            InProgress = false;
            NewAction = true;
            double randomActionChance = _randomGenerator.NextDouble();
            if (randomActionChance < _epsilon) {
                ExecutedAction = State.GetRandomAction();
                return ExecutedAction;
            }
            else {
                OutputNode = _neuralNetwork.Predict(State);
                ExecutedAction = OutputNode.Action;
                return ExecutedAction;
            }

        }   

        public void UpdateNeuralNetwork() {
            this.State.Initialize();
            this.InProgress = true;
            if (!NewAction || State.PreviousState == null) {
                return;
            }

            NewAction = false;

            float reward = State.GetReward();
            float maxNext = _neuralNetwork.Predict(State).Value;
            float[] target = new float[_actions.Count];
            for (int i = 0; i < target.Length; i++) {
                target[i] = _neuralNetwork.Outputs[i];
            }
            target[OutputNode.Index] = reward + _gamma * maxNext;
            _neuralNetwork.Backpropagate(target);
        }

        public void SaveNeuralNetwork() {
            Debug.LogWarning("Saving NN");
            NeuralNetworkSerializer serializer = new NeuralNetworkSerializer {
                neuralNetwork = _neuralNetwork
            };
            RunSerializer runSerializer = new RunSerializer();
            runSerializer.Save();
            serializer.Save();
        }

    }

}
