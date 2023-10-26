using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Utils;
using Assets.Scripts.Game;
using System;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine.Rendering;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.RL
{
    public class RLState
    {
        protected GameManager GameManager { get; set; }
        protected const int PROPERTIES_NUMBER = 5;
        protected object[] PropertiesArray { get; set; }
        protected string[] ObjectsNames { get; set; }
        protected bool[] ObjectsExist { get; set; }
        private List<ForwardModel.Action> Actions { get; set; }
        protected IEnumerator<ForwardModel.Action> ActionEnumerator { get; set; } 

        enum MacroStateHP {
            Lose, VeryLow, Low, OK
        }
        enum MacroStateTime {
            Early, Late, Lose
        }
        enum MacroStateMoney {
            Poor, Mid, Win
        }
        enum MacroStateLevel {
            NotLevel2, Level2
        }
        enum MacroStatePosition {
            TopLeft, TopRight, BottomRight, BottomLeft
        }


        public RLState(List<ForwardModel.Action> actions)
        {
            GameManager = GameManager.Instance;
            this.Actions = new List<ForwardModel.Action>(actions);
            this.ActionEnumerator = this.Actions.GetEnumerator();
            InitializePropertiesArray();
            InitializeDisposableObjectsArray();
        }

        public RLState(RLState parent)
        {
            GameManager = parent.GameManager;
            PropertiesArray = new object[parent.PropertiesArray.Length];
            parent.PropertiesArray.CopyTo(PropertiesArray, 0);
            ObjectsNames = new string[parent.ObjectsNames.Length];
            parent.ObjectsNames.CopyTo(ObjectsNames, 0);
            ObjectsExist = new bool[parent.ObjectsExist.Length];
            parent.ObjectsExist.CopyTo(ObjectsExist, 0);
            Actions = new List<ForwardModel.Action>(parent.Actions);
            Actions.Shuffle();
            ActionEnumerator = parent.Actions.GetEnumerator();
        }

        public void Initialize()
        {
            this.ActionEnumerator.Reset();
            InitializePropertiesArray();
            InitializeDisposableObjectsArray();
        }

        public void InitializePropertiesArray()
        {
            this.PropertiesArray = new object[PROPERTIES_NUMBER];
            this.PropertiesArray[0] = CalculateMacroStateHP((int) this.GameManager.Character.baseStats.HP);
            this.PropertiesArray[1] = CalculateMacroStateMoney((int) this.GameManager.Character.baseStats.Money);
            this.PropertiesArray[2] = CalculateMacroStateLevel((int) this.GameManager.Character.baseStats.Level);
            this.PropertiesArray[3] = CalculateMacroStatePosition((Vector3) this.GameManager.Character.gameObject.transform.position);
            this.PropertiesArray[4] = CalculateMacroStateTime((float) this.GameManager.Character.baseStats.Time);
        }

        public void InitializeDisposableObjectsArray()
        {
            int size = this.GameManager.disposableObjects.Count;
            this.ObjectsNames = new string[size];
            this.ObjectsExist = new bool[size];
            for(int i = 0; i < size; i++) {
                this.ObjectsNames[i] = this.GameManager.disposableObjects.Values.ToArray()[i][0].name;
                this.ObjectsExist[i] =  this.GameManager.disposableObjects.Values.ToArray()[i][0].activeSelf;
            }
        }

        private MacroStateHP CalculateMacroStateHP(int hp) {
            if (hp > 12)
                return MacroStateHP.OK;
            else if (hp > 6)
                return MacroStateHP.Low;
            else if (hp > 0)
                return MacroStateHP.VeryLow;
            else
                return MacroStateHP.Lose;
        }

        private MacroStateMoney CalculateMacroStateMoney(int money) {
            if (money > 20)
                return MacroStateMoney.Win;
            else if (money > 10)
                return MacroStateMoney.Mid;
            else
                return MacroStateMoney.Poor;
        }

        private MacroStateTime CalculateMacroStateTime(float time) {
            if (time >= 150)
                return MacroStateTime.Lose;
            else if (time > 75)
                return MacroStateTime.Late;
            else
                return MacroStateTime.Early;
        }

        private MacroStatePosition CalculateMacroStatePosition(Vector3 position) {
            if (position.x <= 66 && position.y <= 1) //y decreases as you go up
                return MacroStatePosition.TopLeft;
            else if (position.x > 66 && position.y <= 1)
                return MacroStatePosition.TopRight;
            else if (position.x <= 66 && position.y > 1)
                return MacroStatePosition.BottomLeft;
            else
                return MacroStatePosition.BottomRight;
        }

        private MacroStateLevel CalculateMacroStateLevel(int level) {
            if (level == 2)
                return MacroStateLevel.Level2;
            else
                return MacroStateLevel.NotLevel2;
        }

        public virtual object GetProperty(string propertyName)
        {
            //recursive implementation of WorldModelImproved
            var index = this.GetPropertyIndex(propertyName);
            if (index != -1)
            {
                return this.PropertiesArray[index];
            }
            return SearchDisposableObjectExists(propertyName);
        }

        private int GetPropertyIndex(string propertyName)
        {
            return propertyName switch
            {
                Properties.HP => 0,
                Properties.MONEY => 1,
                Properties.LEVEL => 2,
                Properties.POSITION => 3,
                Properties.TIME => 4,
                _ => -1,
            };
        }

        private object SearchDisposableObjectExists(string objectName)
        {
            for(int i = 0; i < GameManager.Instance.disposableObjects.Count; i++) {
                if(this.ObjectsNames[i].Equals(objectName)) {
                    return this.ObjectsExist[i];
                }
            }
            return false;
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            var index = this.GetPropertyIndex(propertyName);
            if (index != -1)
            {
                this.PropertiesArray[index] = CalculateMacroStateProperty(propertyName, value);
                return;
            }
            for(int i = 0; i < GameManager.Instance.disposableObjects.Count; i++) {
                if(this.ObjectsNames[i].Equals(propertyName)) {
                    this.ObjectsExist[i] = (bool) value;
                    return;
                }
            }
        }

        private object CalculateMacroStateProperty(string propertyName, object value) {
            return propertyName switch
            {
                Properties.HP => CalculateMacroStateHP((int) value),
                Properties.MONEY => CalculateMacroStateMoney((int) value),
                Properties.LEVEL => CalculateMacroStateLevel((int) value),
                Properties.POSITION => CalculateMacroStatePosition((Vector3) value),
                Properties.TIME => CalculateMacroStateTime((float) value),
                _ => -1,
            };
        }

        public virtual ForwardModel.Action GetNextAction()
        {
            ForwardModel.Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute())
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;    
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }

        public virtual ForwardModel.Action[] GetExecutableActions()
        {
            return this.Actions.Where(a => a.CanExecute()).ToArray();
        }

        public virtual float GetReward()
        {
            return MoneyReward() + LevelReward() + HpReward();
        }

        private float MoneyReward() {
            MacroStateMoney money = (MacroStateMoney)this.GetProperty(Properties.MONEY);
            if (money == MacroStateMoney.Win)
                return 100;
            else
                return 0;
        }

        private float LevelReward() {
            MacroStateLevel level = (MacroStateLevel)this.GetProperty(Properties.LEVEL);
            if (level == MacroStateLevel.Level2)
                return 0.5f;
            else
                return 0; 
        }

        private float HpReward() {
            MacroStateHP hp = (MacroStateHP)this.GetProperty(Properties.HP);
            if (hp == MacroStateHP.Lose)
                return -100;
            return 0;

        }

        public ForwardModel.Action GetRandomAction() {
            var actions = GetExecutableActions();
            actions.Shuffle();
            return actions[0];
        }

        public RLState Copy() {
            return new RLState(this);
        }
    }
}