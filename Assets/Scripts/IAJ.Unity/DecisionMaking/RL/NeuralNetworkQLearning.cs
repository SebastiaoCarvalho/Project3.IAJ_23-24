using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL {

    public class NeuralNetworkQLearning {
        private NeuralNetwork _neuralNetwork;
        private Random _randomGenerator;
        protected Action ExecutedAction;
        protected OutputNode OutputNode;
        public RLState State;
        private List<Action> _actions;
        private float _epsilon;
        private float _alpha { get; set; }
        private float _gamma { get; set; }
        protected bool NewAction { get; set; }

        public NeuralNetworkQLearning(RLState state, List<Action> actions) {
            _actions = actions;
            _neuralNetwork = new NeuralNetwork(3, new List<int>{ 4, 4, 27 }, state.ToArray());
            _epsilon = 0.1f;
            _randomGenerator = new Random();
            State = state;
        }

        public Action ChooseAction() {
            double randomActionChance = _randomGenerator.NextDouble();
            if (randomActionChance < _epsilon) {
                ExecutedAction = State.GetRandomAction();
                return ExecutedAction;
            }
            else {
                OutputNode = _neuralNetwork.Predict();
                ExecutedAction = OutputNode.Action;
                return ExecutedAction;
            }
        }   

        public void UpdateNeuralNetwork() {
            if (!NewAction || State.PreviousState == null) {
                return;
            }

            NewAction = false;

            float reward = State.GetReward();
            float Q = OutputNode.Value;
            float maxNext = _neuralNetwork.Predict(State).Value;
            float[] target = new float[27];
            for (int i = 0; i < 27; i++) {
                target[i] = _neuralNetwork.Outputs[i];
            }
            target[OutputNode.Index] = reward + _gamma * maxNext;
            // FIXME : move this to right place

        }


    }

}