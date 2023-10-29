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
        protected const int PROPERTIES_NUMBER = 6;
        protected object[] PropertiesArray { get; set; }
        protected string[] ObjectsNames { get; set; }
        protected bool[] ObjectsExist { get; set; }
        private List<ForwardModel.Action> Actions { get; set; }
        protected IEnumerator<ForwardModel.Action> ActionEnumerator { get; set; } 
        public RLState PreviousState { get; protected set; }

        enum MacroStateHP {
            VeryLow, Low, OK, Good
        }
        enum MacroStateTime {
            Early, Mid, Late
        }
        enum MacroStateMoney {
            VeryPoor, Poor, Mid, Rich, VeryRich
        }
        enum MacroStateLevel {
            NotLevel2, Level2
        }
        enum MacroStatePosition {
            TopLeft, TopRight, BottomRight, BottomLeft
        }
        enum MacroStateDragonAndOrc {
            EitherAlive, BothDead
        }


        public RLState(List<ForwardModel.Action> actions)
        {
            this.PreviousState = null;
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

        public void Reset() {
            this.PreviousState = null;
            this.GameManager.Character.Restart();
            InitializePropertiesArray();
             this.PropertiesArray[5] = MacroStateDragonAndOrc.EitherAlive;
        }

        public void Initialize()
        {
            PreviousState = this.Copy();
            this.ActionEnumerator.Reset();
            InitializePropertiesArray();
            InitializeDisposableObjectsArray();
        }

        public void InitializePropertiesArray()
        {
            this.PropertiesArray = new object[PROPERTIES_NUMBER];
            this.PropertiesArray[0] = CalculateMacroStateHP((int) this.GameManager.Character.baseStats.HP + (int) GameManager.Character.baseStats.ShieldHP);
            this.PropertiesArray[1] = CalculateMacroStateMoney((int) this.GameManager.Character.baseStats.Money);
            this.PropertiesArray[2] = CalculateMacroStateLevel((int) this.GameManager.Character.baseStats.Level);
            this.PropertiesArray[3] = CalculateMacroStatePosition((Vector3) this.GameManager.Character.transform.position);
            this.PropertiesArray[4] = CalculateMacroStateTime((float) this.GameManager.Character.baseStats.Time);
            this.PropertiesArray[5] = CalculateMacroStateDragon();
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

        private MacroStateDragonAndOrc CalculateMacroStateDragon() {
            if (GameObject.FindGameObjectWithTag("Dragon") == null && GameObject.Find("Orc2") == null)
                return MacroStateDragonAndOrc.BothDead;
            return MacroStateDragonAndOrc.EitherAlive;
        }

        private MacroStateHP CalculateMacroStateHP(int hp) {
            if (hp > 18)
                return MacroStateHP.Good;
            else if (hp > 12)
                return MacroStateHP.OK;
            else if (hp > 6)
                return MacroStateHP.Low;
            else
                return MacroStateHP.VeryLow;
        }

        private MacroStateMoney CalculateMacroStateMoney(int money) {
            if (money == 20)
                return MacroStateMoney.VeryRich;
            else if (money == 15)
                return MacroStateMoney.Rich;
            else if (money == 10)
                return MacroStateMoney.Mid;
            else if (money == 5)
                return MacroStateMoney.Poor;
            else
                return MacroStateMoney.VeryPoor;
        }

        private MacroStateTime CalculateMacroStateTime(float time) {
            if (time >= 100)
                return MacroStateTime.Late;
            else if (time >= 50)
                return MacroStateTime.Mid;
            else
                return MacroStateTime.Early;
        }

        private MacroStatePosition CalculateMacroStatePosition(Vector3 position) {
            if (position.x <= 66 && position.z >= 48.27)
                return MacroStatePosition.TopLeft;
            else if (position.x > 66 && position.z >= 48.27)
                return MacroStatePosition.TopRight;
            else if (position.x <= 66 && position.z < 48.27)
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

        public virtual ForwardModel.Action[] GetExecutableActions()
        {
            return this.Actions.Where(a => a.CanExecute()).ToArray();
        }

        public virtual float GetReward()
        {
            //Debug.Log("Reward " + (MoneyReward() + LevelReward() + HpReward()));
            return MoneyReward() + LevelReward() + HpReward() + TimeReward() + PositionReward() + DragonAndOrcReward();
        }

        public virtual bool IsTerminal()
        {
            return (int)GameManager.Character.baseStats.HP <= 0 ||
                   (float)GameManager.Character.baseStats.Time >= GameManager.GameConstants.TIME_LIMIT ||
                   (int)GameManager.Character.baseStats.Money == 25;
        }

        private float PositionReward() {
            if ((MacroStatePosition)PreviousState.PropertiesArray[3] != (MacroStatePosition)this.PropertiesArray[3])
                return -1f;
            return 0f;
        }

        private float DragonAndOrcReward() {
            if ((MacroStateDragonAndOrc)PreviousState.PropertiesArray[5] == MacroStateDragonAndOrc.EitherAlive &&
                (MacroStateDragonAndOrc)this.PropertiesArray[5] == MacroStateDragonAndOrc.BothDead &&
                (MacroStateTime)this.PropertiesArray[4] == MacroStateTime.Late)
                return 10f;
            return 0f;
        }

        private float MoneyReward() {
            int money = (int)GameManager.Character.baseStats.Money;
            if (money == 25) {
                GameManager.Instance.winCounter++;
                return 100f;
            }
            else if ((MacroStateMoney)PreviousState.PropertiesArray[1] != 
                     (MacroStateMoney)this.PropertiesArray[1])
                return 10f;
            else if ((MacroStateTime)this.PropertiesArray[4] == MacroStateTime.Late)
                return -5f;
            return 0f;
        }

         private float TimeReward() {
            float time = (float)GameManager.Character.baseStats.Time;
            if (time >= 150f) {
                GameManager.Instance.timeoutCounter++;
            }
            return 0;
        } 

        private float LevelReward() {
            if ((MacroStateLevel)PreviousState.PropertiesArray[2] == MacroStateLevel.NotLevel2
                  && (MacroStateLevel)this.PropertiesArray[2] == MacroStateLevel.Level2)
                return 20f;
            return 0f; 
        }

        private float HpReward() {
            int hp = (int)GameManager.Character.baseStats.HP + (int)GameManager.Character.baseStats.ShieldHP;
            if (hp <= 0) {
                GameManager.Instance.deathCounter++;
                return -100f;
            }
            else if ((MacroStateHP)PreviousState.PropertiesArray[0] == MacroStateHP.VeryLow 
                  && (MacroStateHP)this.PropertiesArray[0] == MacroStateHP.Good)
                return 10f;
            return 0f;

        }

        public ForwardModel.Action GetRandomAction() {
            var actions = GetExecutableActions();
            actions.Shuffle();
            return actions[0];
        }

        public RLState Copy() {
            return new RLState(this);
        }

        public override bool Equals(object obj)
        {
            RLState state = (RLState) obj;
            return (MacroStateHP)PropertiesArray[0] == (MacroStateHP)state.PropertiesArray[0] &&
                   (MacroStateMoney)PropertiesArray[1] == (MacroStateMoney)state.PropertiesArray[1] &&
                   (MacroStateLevel)PropertiesArray[2] == (MacroStateLevel)state.PropertiesArray[2] &&
                   (MacroStatePosition)PropertiesArray[3] == (MacroStatePosition)state.PropertiesArray[3] &&
                   (MacroStateTime)PropertiesArray[4] == (MacroStateTime)state.PropertiesArray[4] &&
                   (MacroStateDragonAndOrc)PropertiesArray[5] == (MacroStateDragonAndOrc)state.PropertiesArray[5];
        }

        public override string ToString()
        {
            return "HP: " + (MacroStateHP)PropertiesArray[0] + 
                   " Money: " + (MacroStateMoney)PropertiesArray[1] + 
                   " Level: " + (MacroStateLevel)PropertiesArray[2] +
                   " Position: " + (MacroStatePosition)PropertiesArray[3] +
                   " Time: " + (MacroStateTime)PropertiesArray[4] +
                   " DragonAndOrc: " + (MacroStateDragonAndOrc)PropertiesArray[5];
        }
    }
}
