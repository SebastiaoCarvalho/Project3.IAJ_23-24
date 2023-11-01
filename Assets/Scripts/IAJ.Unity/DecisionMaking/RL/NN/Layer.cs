using Random = System.Random;
using System;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN {

    public class Layer {


        private Func<float, float> _activationFunction;
        private Func<float, float> _activationFunctionDerivative;
        private int _numberOfInputs;
        private int _numberOfOutputs;
        private float[] _inputs;
        private float[] _outputs;
        private float[] _preActivatedOutputs;
        public float[] Outputs {
            get { return _outputs; }
        }
        private float[,] _weights;
        public float [,] Weights {
            get { return _weights; }
        }
        private float[] _bias;
        public float[] Bias {
            get { return _bias; }
        }
        private float[] _deltas;


        public Layer(int numberOfInputs, int numberOfOutputs, Func<float,float> activationFunction = null, Func<float,float> activationFunctionDerivative = null) {
            _activationFunction = activationFunction;
            _activationFunctionDerivative = activationFunctionDerivative;
            _numberOfInputs = numberOfInputs;
            _numberOfOutputs = numberOfOutputs;
            _inputs = new float[numberOfInputs];
            _preActivatedOutputs = new float[numberOfOutputs];
            _outputs = new float[numberOfOutputs];
            _deltas = new float[numberOfOutputs];
            _weights = new float[numberOfInputs, numberOfOutputs];
            _bias = new float[numberOfOutputs];
        }

        public void Initialize() {
            float mean = 0;
            double stdDev = Math.Sqrt(2d / _numberOfInputs);
            var random = new Random();
            for (var i = 0; i < _numberOfInputs; i++) {
                for (var j = 0; j < _numberOfOutputs; j++) { // generate weights using He Weight Initialization
                    double u1 = 1.0-random.NextDouble(); //uniform(0,1] random doubles
                    double u2 = 1.0-random.NextDouble();
                    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                    double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
                    Debug.Log(stdDev + " " + randNormal);
                    _weights[i, j] = (float) randNormal;
                }
            }
            for (var i = 0; i < _numberOfOutputs; i++) {
                _bias[i] = 0.01f; // Assure firing of RELU function
            }
        }

        public void Propagate(Layer previous) {
            for (var i = 0; i < _numberOfInputs; i++) {
                this._preActivatedOutputs[i] = 0;
                this._inputs[i] = previous._outputs[i];
                for (var j = 0; j < previous._numberOfOutputs; j++) {
                    this._preActivatedOutputs[i] += previous._outputs[j] * this._weights[i, j];
                }
                this._preActivatedOutputs[i] += this._bias[i];
                this._outputs[i] = this._activationFunction(_preActivatedOutputs[i]);
            }
        }

        public void Propagate(float[] inputs) { // used for the input layer
            _inputs = inputs;
            _outputs = inputs;
        }

        public void Backpropagate(Layer next) {
            var weightsTranspose = MathHelper.Transpose(_weights); 
            for (var i = 0; i < _numberOfOutputs; i++) {
                _deltas[i] = 0;
                for (var j = 0; j < next._numberOfInputs; j++) {
                    _deltas[i] += next._deltas[j] * weightsTranspose[i, j];
                }
                _deltas[i] *= _activationFunctionDerivative(_preActivatedOutputs[i]);
            }
        }

        public void Backpropagate(float[] target) { // used for output layer
            for (var i = 0; i < _numberOfOutputs; i++) {
                _deltas[i] = (_outputs[i] - target[i]) * _activationFunctionDerivative(_preActivatedOutputs[i]);
            }
        }

        public void UpdateWeights(float alpha) {
            for (var i = 0; i < _numberOfInputs; i++) {
                for (var j = 0; j < _numberOfOutputs; j++) {
                    _weights[i, j] -= alpha * _deltas[j] * _outputs[i];
                }
            }
            for (var i = 0; i < _numberOfOutputs; i++) {
                _bias[i] -= alpha * _deltas[i];
            }
        }
    }

}