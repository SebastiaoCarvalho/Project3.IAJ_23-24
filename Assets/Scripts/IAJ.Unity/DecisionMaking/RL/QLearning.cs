using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;
using UnityEngine;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL
{
    public class QLearning
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public float TotalProcessingTime { get; set; }
        protected int CurrentIterations { get; set; }
        protected int FrameCurrentIterations { get; set; }
        public RLState State {  get; protected set; }
        public QTable Store { get; protected set; }
        protected Action ExecutedAction { get; set; }
        //protected MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }
        protected float Nu { get; set; }
        public float Epsilon { get; set; }
        protected float Alpha { get; set; }
        protected float Gamma { get; set; }
        protected bool NewAction { get; set; }

        public QLearning(RLState initialState)
        {
            this.InProgress = false;
            this.State = initialState;
            this.RandomGenerator = new System.Random();
            Store = new QTable();
            Alpha = 0.5f;
            Gamma = 0.1f;
            Epsilon = 0.1f;
            NewAction = false;
        }


        public void InitializeQLearning()
        {
            this.CurrentIterations = 0;
            this.FrameCurrentIterations = 0;
            this.TotalProcessingTime = 0.0f;
            this.State.Reset();
        }

        public Action ChooseAction()
        {
            if (State.IsTerminal())
            {
                return null;
            }

            InProgress = false;
            NewAction = true;
            double randomActionChance = RandomGenerator.NextDouble();
            if (randomActionChance < Epsilon) //pick a random action every once in a while
            {
                ExecutedAction = State.GetRandomAction();
                return ExecutedAction;
            }
            else
            {
                ExecutedAction = Store.GetBestAction(State);
                return ExecutedAction;
            }
        }

        public void UpdateQTable()
        {
            this.State.Initialize();
            this.InProgress = true;
            if (!NewAction || State.PreviousState == null) {
                return;
            }

            NewAction = false;

            float reward = State.GetReward(ExecutedAction);

            float Q = Store.GetQValue(State.PreviousState, ExecutedAction);

            float newStateBestQ = Store.GetQValue(State, Store.GetBestAction(State));

            float newQ = (1 - Alpha) * Q + Alpha * (reward + Gamma * newStateBestQ);
 
            Store.SetQValue(State.PreviousState, ExecutedAction, newQ);
        }

        public void SaveQTable()
        {
            Debug.LogWarning("Saving QTable");
            QTableSerializer serializer = new QTableSerializer()
            {
                qTable = Store
            };
            RunSerializer runSerializer = new RunSerializer();
            runSerializer.Save();
            serializer.Save();
        }

        public void LoadQTable()
        {
            Debug.LogWarning("Loading QTable");
            QTableSerializer serializer = new QTableSerializer();
            serializer.Load();
            RunSerializer runSerializer = new RunSerializer();
            runSerializer.Load();
            Store = serializer.qTable;
        }

    }

}
