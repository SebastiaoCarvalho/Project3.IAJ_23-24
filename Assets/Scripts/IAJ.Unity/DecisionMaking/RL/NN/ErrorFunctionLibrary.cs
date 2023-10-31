
using System;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN {
    public class ErrorFunctionLibrary  {
        
        public static Func<float, float, float> MeanSquaredError = (y, t) => {
            return (float) Math.Pow(y - t, 2);
        };
    }
}