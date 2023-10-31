using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN {

    public class NeuralNetwork {

        private int _numberOfLayers;
        private Layer[] _layers;
        private int _numberOfInputs;
        private int _numberOfOutputs;
        private float[] _inputs;
        private float[] _outputs;
        public float[] Outputs {
            get { return _outputs; }
        }
        private List<Action> _actions;

        public NeuralNetwork(int numberOfLayers, List<int> neuronsPerLayer, float[] inputs) {
            _numberOfLayers = numberOfLayers;
            _layers = new Layer[numberOfLayers];
            for (var i = 0; i < numberOfLayers; i++) {
                if (i == 0) {
                    _layers[i] = new Layer(neuronsPerLayer[i], neuronsPerLayer[i + 1], ActivationFunctionLibrary.RELU);
                } else if (i == numberOfLayers - 1) {
                    _layers[i] = new Layer(neuronsPerLayer[i], neuronsPerLayer[i + 1], ActivationFunctionLibrary.RELU);
                } else {
                    _layers[i] = new Layer(neuronsPerLayer[i], neuronsPerLayer[i + 1], ActivationFunctionLibrary.RELU);
                }
                _layers[i].Initialize();
            }
            _numberOfInputs = inputs.Length;
            _numberOfOutputs = neuronsPerLayer[numberOfLayers - 1];
            _inputs = inputs;
        }

        public OutputNode Predict() {
            Propagate();
            float max = float.MinValue;
            Action bestAction = null;
            for (int i = 0; i < _outputs.Length; i++) {
                if (max < _outputs[i]) {
                    max = _outputs[i];
                    bestAction = _actions[i];
                }
            }
            return new OutputNode(bestAction, max, _actions.IndexOf(bestAction));
        }

        public OutputNode Predict(RLState state) {
            _inputs = state.ToArray();
            _numberOfInputs = _inputs.Length;
            return Predict();
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

    }

}