using System;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN {

    public class Layer {


        private Func<float, float> _activationFunction;
        private int _numberOfInputs;
        private int _numberOfOutputs;
        private float[] _inputs;
        private float[] _outputs;
        public float[] Outputs {
            get { return _outputs; }
        }
        private float[,] _weights;
        private float[] _bias;


        public Layer(int numberOfInputs, int numberOfOutputs, Func<float,float> activationFunction = null) {
            _activationFunction = activationFunction;
            _numberOfInputs = numberOfInputs;
            _numberOfOutputs = numberOfOutputs;
            _inputs = new float[numberOfInputs];
            _outputs = new float[numberOfOutputs];
            _weights = new float[numberOfInputs, numberOfOutputs];
            _bias = new float[numberOfOutputs];
        }

        public void Initialize() {
            float mean = 0;
            double stdDev = Math.Sqrt(2 / _numberOfInputs);
            var random = new Random();
            for (var i = 0; i < _numberOfInputs; i++) {
                for (var j = 0; j < _numberOfOutputs; j++) { // generate weights using He Weight Initialization
                    double u1 = 1.0-random.NextDouble(); //uniform(0,1] random doubles
                    double u2 = 1.0-random.NextDouble();
                    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                    double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
                    _weights[i, j] = (float) randNormal;
                }
            }
            for (var i = 0; i < _numberOfOutputs; i++) {
                _bias[i] = 0.01f; // Assure firing of RELU function
            }
        }

        public void Propagate(Layer previous) {
            for (var i = 0; i < _numberOfInputs; i++) {
                this._inputs[i] = 0;
                for (var j = 0; j < previous._numberOfOutputs; j++) {
                    this._inputs[i] += previous._outputs[j] * this._weights[i, j];
                }
                this._inputs[i] += this._bias[i];
                this._outputs[i] = this._activationFunction(_inputs[i]);
            }
        }

        public void Propagate(float[] inputs) { // used for the input layer
            _inputs = inputs;
            _outputs = inputs;
        }

        public void Backpropagate(Layer next, float[] error) { // FIXME
            for (var i = 0; i < _numberOfOutputs; i++) {
                error[i] = 0;
                for (var j = 0; j < next._numberOfInputs; j++) {
                    error[i] += next._weights[j, i] * next._outputs[j];
                }
                error[i] *= _activationFunction(_inputs[i]);
            }
        }

    }

}