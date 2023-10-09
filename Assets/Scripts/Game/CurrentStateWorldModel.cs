using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Game
{
    //class that represents a world model that corresponds to the current state of the world,
    //all required properties and goals are stored inside the game manager
    public class CurrentStateWorldModel : FutureStateWorldModel
    {
        private Dictionary<string, Goal> Goals { get; set; } 

        public CurrentStateWorldModel(GameManager gameManager, List<Action> actions, List<Goal> goals) : base(gameManager, actions)
        {
            this.Parent = null;
            this.Goals = new Dictionary<string, Goal>();

            foreach (var goal in goals)
            {
                this.Goals.Add(goal.Name,goal);
            }
            InitializePropertiesArray();
            InitializeDisposableObjectsArray();
        }

        public void Initialize()
        {
            this.ActionEnumerator.Reset();
        }

        public override void InitializePropertiesArray()
        {
            this.PropertiesArray = new object[PROPERTIES_NUMBER];
            this.PropertiesArray[0] = this.GameManager.Character.baseStats.HP;
            this.PropertiesArray[1] = this.GameManager.Character.baseStats.Mana;
            this.PropertiesArray[2] = this.GameManager.Character.baseStats.Money;
            this.PropertiesArray[3] = this.GameManager.Character.baseStats.Level;
            this.PropertiesArray[4] = this.GameManager.Character.baseStats.XP;
            this.PropertiesArray[5] = this.GameManager.Character.baseStats.ShieldHP;
            this.PropertiesArray[6] = this.GameManager.Character.gameObject.transform.position;
            this.PropertiesArray[7] = this.GameManager.Character.baseStats.Time;
            this.PropertiesArray[8] = this.GameManager.Character.baseStats.MaxHP;
            this.PropertiesArray[9] = this.GameManager.Character.baseStats.MaxMana;
            this.PropertiesArray[10] = this.GameManager.Character.baseStats.MaxShieldHp;
        }

        public override void InitializeDisposableObjectsArray()
        {
            int size = GameManager.Instance.InitialDisposableObjectsCount;
            this.DisposableObjectsArray = new GameObject[size];
            this.ObjectsExist = new bool[size];
            for(int i = 0; i < size; i++) {
                this.DisposableObjectsArray[i] = this.GameManager.disposableObjects.Values.ToArray()[i][0];
                this.ObjectsExist[i] = this.DisposableObjectsArray[i].activeSelf;
            }
        }
        public override object GetProperty(string propertyName)
        {
            InitializePropertiesArray();
            return base.GetProperty(propertyName);
        }

        public override float GetGoalValue(string goalName)
        {
            return this.Goals[goalName].InsistenceValue;
        }

        public override void SetGoalValue(string goalName, float goalValue)
        {
            //this method does nothing, because you should not directly set a goal value of the CurrentStateWorldModel
        }

        public override void SetProperty(string propertyName, object value)
        {
            //this method does nothing, because you should not directly set a property of the CurrentStateWorldModel
        }

        public override int GetNextPlayer()
        {
            //in the current state, the next player is always player 0
            return 0;
        }
    }
}
