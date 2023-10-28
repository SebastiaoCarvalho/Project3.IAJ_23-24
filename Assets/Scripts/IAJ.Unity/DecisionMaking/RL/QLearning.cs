using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL
{
    public class QLearning
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        protected int CurrentIterations { get; set; }
        protected int FrameCurrentIterations { get; set; }
        protected RLState PreviousState { get; set; }
        protected RLState CurrentState { get; set; }
        protected QTable Store { get; set; }
        protected Action ExecutedAction { get; set; }
        //protected MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }
        protected float Nu { get; set; }
        protected float Epsilon { get; set; }
        protected float Alpha { get; set; }
        protected float Gamma { get; set; }
        protected bool NewAction { get; set; }

        public QLearning(RLState initialState)
        {
            this.InProgress = false;
            this.CurrentState = initialState;
            this.MaxIterations = 1000;
            this.MaxIterationsPerFrame = 500;
            this.RandomGenerator = new System.Random();
            Store = new QTable();
            Alpha = 0.5f;
            Gamma = 0.1f;
            Epsilon = 0.05f;
            NewAction = false;
        }


        public void InitializeQLearning()
        {
            this.CurrentIterations = 0;
            this.FrameCurrentIterations = 0;
            this.TotalProcessingTime = 0.0f;
            this.PreviousState = null;
        }

        public void Reset() {
            this.PreviousState = null;
        }

        public Action ChooseAction()
        {
            if (CurrentState.IsTerminal())
            {
                return null;
            }

            double randomRestartChance = RandomGenerator.NextDouble();
            /*
            if (randomRestartChance < Nu) //pick a new random state every once in a while
            {
                CurrentState = problem.getRandomState();
            }
            */
            
            Action[] executableActions = CurrentState.GetExecutableActions();
            //actions = problem.getAvailableActions(state)

            InProgress = false;
            NewAction = true;
            double randomActionChance = RandomGenerator.NextDouble();
            if (randomActionChance < Epsilon) //pick a random action every once in a while
            {
                //action = actions.getRandomAction()
                PreviousState = CurrentState.Copy();
                ExecutedAction = CurrentState.GetRandomAction();
                return ExecutedAction;
            }
            else
            {
                PreviousState = CurrentState.Copy();
                ExecutedAction = Store.GetBestAction(CurrentState);
                return ExecutedAction;
            }
        }

        public void UpdateQTable()
        {
            this.CurrentState.Initialize();
            this.InProgress = true;
            if (!NewAction || PreviousState == null) {
                return;
            }

            NewAction = false;

            float reward = CurrentState.GetReward();

            //reward, newState = problem.performAction(state,action)
                
            //Q = store.getQValue(state,action)
            float Q = Store.GetQValue(PreviousState, ExecutedAction);

            float newStateBestQ = Store.GetQValue(CurrentState, Store.GetBestAction(CurrentState));
            //maxQ = store.getQValue(newState,store.getBestAction(newState))
            float newQ = (1 - Alpha) * Q + Alpha * (reward + Gamma * newStateBestQ);
            Debug.Log("Q : " + Q + " reward : " + reward + " newStateBestQ : " + newStateBestQ + " newQ : " + newQ);
            Store.SetQValue(PreviousState, ExecutedAction, newQ);
                
            //store.storeQValue(state,action,Q)
        }
        public void SaveQTable()
        {
            Debug.LogWarning("Saving QTable");
            QTableSerializer serializer = new QTableSerializer()
            {
                qTable = Store
            };
            serializer.Save();
        }

        public void LoadQTable()
        {
            Debug.LogWarning("Loading QTable");
            QTableSerializer serializer = new QTableSerializer();
            serializer.Load();
            Store = serializer.qTable;
        }

    }

}
