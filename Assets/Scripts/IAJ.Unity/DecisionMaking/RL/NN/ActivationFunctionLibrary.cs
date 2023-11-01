
using System;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN {
    public class ActivationFunctionLibrary  {
        
        public static Func<float, float> RELU = (x) => {
            if (x > 0) {
                return x;
            } else {
                return 0;
            }
        };

        public static Func<float, float> RELUDerivate = (x) => {
            if (x > 0) {
                return 1;
            } else {
                return 0;
            }
        };

        public static Func<float, float> Linear = (x) => {
            return x;
        };

        public static Func<float, float> LinearDerivate = (x) => {
            return 1;
        };

        public static Func<float, float> Sigmoid = (x) => {
            return 1 / (1 + ((float) Math.Exp(-x)));
        };

        public static Func<float, float> SigmoidDerivate = (x) => {
            return Sigmoid(x) * (1 - Sigmoid(x));
        };
    }
}