
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL.NN {
    public struct OutputNode {

        private Action _action;
        private float _value;
        private int _index;
        public Action Action { get { return _action; } }
        public float Value { get { return _value; } }
        public int Index { get { return _index; } }

        public OutputNode(Action action, float value, int index) {
            _action = action;
            _index = index;
            _value = value;
        }

    }

}