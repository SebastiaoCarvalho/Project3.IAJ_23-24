
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

        public static Func<float, float> Linear = (x) => {
            return x;
        };

        public static Func<float, float> Sigmoid = (x) => {
            return 1 / (1 + ((float) Math.Exp(-x)));
        };
    }
}