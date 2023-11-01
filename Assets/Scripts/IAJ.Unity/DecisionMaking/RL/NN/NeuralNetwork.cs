using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Utils;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN {

    public class NeuralNetwork {

        private int _numberOfLayers;
        private float _alpha;
        private Layer[] _layers;
        public Layer[] Layers {
            get { return _layers; }
        }
        private int _numberOfInputs;
        private int _numberOfOutputs;
        private float[] _inputs;
        private float[] _outputs;
        public float[] Outputs {
            get { return _outputs; }
        }
        Func<float, float, float> _errorFunction;
        private List<Action> _actions;

        public NeuralNetwork(int numberOfLayers, List<int> neuronsPerLayer, float[] inputs, List<Action> actions, float alpha = 0.5f) {
            _numberOfLayers = numberOfLayers;
            _layers = new Layer[numberOfLayers];
            for (var i = 0; i < numberOfLayers; i++) {
                if (i == 0) {
                    _layers[i] = new Layer(neuronsPerLayer[i], neuronsPerLayer[i]); // First layer is an abstraction, since it's only the input
                } else if (i == numberOfLayers - 1) {
                    _layers[i] = new Layer(neuronsPerLayer[i - 1], neuronsPerLayer[i], ActivationFunctionLibrary.RELU, ActivationFunctionLibrary.RELUDerivate);
                } else {
                    _layers[i] = new Layer(neuronsPerLayer[i - 1], neuronsPerLayer[i], ActivationFunctionLibrary.Linear, ActivationFunctionLibrary.LinearDerivate);
                }
                _layers[i].Initialize();
            }
            _numberOfInputs = inputs.Length;
            _numberOfOutputs = neuronsPerLayer[numberOfLayers - 1];
            _inputs = inputs;
            _errorFunction = ErrorFunctionLibrary.MeanSquaredError; // FIXME : maybe remove this since it's not used
            _alpha = alpha;
            _actions = actions;
        }

        /* private OutputNode Predict() {
            Propagate();
            float max = float.MinValue;
            Action bestAction = null;
            for (int i = 0; i < _outputs.Length; i++) {
                if (_actions.)
                if (max < _outputs[i]) {
                    max = _outputs[i];
                    bestAction = _actions[i];
                }
            }
            return new OutputNode(bestAction, max, _actions.IndexOf(bestAction));
        } */

        public OutputNode Predict(RLState state) {
            _inputs = state.ToArray();
            _numberOfInputs = _inputs.Length;
             Propagate();
            float max = float.MinValue;
            Action bestAction = null;
            int[] indexes = RandomHelper.IndexesShuffled(_outputs.Length);
            foreach (int i in indexes) {
                if (! state.GetExecutableActions().ToList().Contains(_actions[i])) 
                    continue;
                if (max < _outputs[i]) {
                    max = _outputs[i];
                    bestAction = _actions[i];
                }
            }
            return new OutputNode(bestAction, max, _actions.IndexOf(bestAction));
        }

        private void Propagate() {
            for (var i = 0; i < _numberOfLayers; i++) {
                if (i == 0) {
                    _layers[i].Propagate(_inputs);
                } else {
                    _layers[i].Propagate(_layers[i - 1]);
                }
            }
            _outputs = _layers[_numberOfLayers - 1].Outputs;
        }

        public void Backpropagate(float[] target) {
            for (var i = _numberOfLayers - 1; i >= 1; i--) { // ignore input layer
                if (i == _numberOfLayers - 1) {
                    _layers[i].Backpropagate(target);
                } else {
                    _layers[i].Backpropagate(_layers[i + 1]);
                }
            }
            for (int i = 0; i < _numberOfLayers; i++) {
                if (i != 0)
                    _layers[i].UpdateWeights(_alpha);
            }
        }

    }

}