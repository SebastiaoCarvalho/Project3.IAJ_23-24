﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Utils;
using Assets.Scripts.Game;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel
{
    public class WorldModel
    {
        protected const int PROPERTIES_NUMBER = 11;
        protected object[] PropertiesArray { get; set; }
        protected GameObject[] DisposableObjectsArray { get; set; }
        private List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; } 

        private Dictionary<string, float> GoalValues { get; set; } 

        protected WorldModel Parent { get; set; }

        public WorldModel(List<Action> actions)
        {
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = new List<Action>(actions);
            this.Actions.Shuffle();
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public WorldModel(WorldModel parent)
        {
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = new List<Action>(parent.Actions);
            this.Actions.Shuffle();
            this.Parent = parent;
            InitializePropertiesArray();
            InitializeDisposableObjectsArray();
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public virtual void InitializePropertiesArray()
        {
            this.PropertiesArray = new object[PROPERTIES_NUMBER];
            for(int i = 0; i < PROPERTIES_NUMBER; i++) {
                this.PropertiesArray[i] = this.Parent.PropertiesArray[i];
            }
        }

        public virtual void InitializeDisposableObjectsArray()
        {
            int size = GameManager.Instance.InitialDisposableObjectsCount;
            this.DisposableObjectsArray = new GameObject[GameManager.Instance.InitialDisposableObjectsCount];
            for(int i = 0; i < size; i++) {
                this.DisposableObjectsArray[i] = this.Parent.DisposableObjectsArray[i];
            }
        }

        public virtual object GetProperty(string propertyName)
        {
            //recursive implementation of WorldModel
            var index = this.GetPropertyIndex(propertyName);
            if (index != -1)
            {
                return this.PropertiesArray[index];
            }
            return SearchDisposableObject(propertyName);
        }

        private int GetPropertyIndex(string propertyName)
        {
            return propertyName switch
            {
                Properties.HP => 0,
                Properties.MANA => 1,
                Properties.MONEY => 2,
                Properties.LEVEL => 3,
                Properties.XP => 4,
                Properties.ShieldHP => 5,
                Properties.POSITION => 6,
                Properties.TIME => 7,
                Properties.MAXHP => 8,
                Properties.MAXMANA => 9,
                Properties.MaxShieldHP => 10,
                _ => -1,
            };
        }

        private object SearchDisposableObject(string objectName)
        {
            for(int i = 0; i < GameManager.Instance.InitialDisposableObjectsCount; i++) {
                if(this.DisposableObjectsArray[i] != null && this.DisposableObjectsArray[i].name == objectName) {
                    return this.DisposableObjectsArray[i].activeSelf;
                }
            }
            return false;
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            var index = this.GetPropertyIndex(propertyName);
            if (index != -1)
            {
                this.PropertiesArray[index] = value;
                return;
            }
            if (! (bool) value) {
                for(int i = 0; i < GameManager.Instance.InitialDisposableObjectsCount; i++) {
                    if(this.DisposableObjectsArray[i] != null && this.DisposableObjectsArray[i].name == propertyName) {
                        this.DisposableObjectsArray[i] = null;
                    }
            }
            }
        }

        public virtual float GetGoalValue(string goalName)
        {
            //recursive implementation of WorldModel
            if (this.GoalValues.ContainsKey(goalName))
            {
                return this.GoalValues[goalName];
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetGoalValue(goalName);
            }
            else
            {
                return 0;
            }
        }

        public virtual void SetGoalValue(string goalName, float value)
        {
            var limitedValue = value;
            if (value > 10.0f)
            {
                limitedValue = 10.0f;
            }

            else if (value < 0.0f)
            {
                limitedValue = 0.0f;
            }

            this.GoalValues[goalName] = limitedValue;
        }

        public virtual WorldModel GenerateChildWorldModel()
        {
            return new WorldModel(this);
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.Name);

                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public virtual Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
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

        public virtual Action[] GetExecutableActions()
        {
            return this.Actions.Where(a => a.CanExecute(this)).ToArray();
        }

        public virtual bool IsTerminal()
        {
            return true;
        }
        

        public virtual float GetScore()
        {
            return 0.0f;
        }

        public virtual int GetNextPlayer()
        {
            return 0;
        }

        public virtual void CalculateNextPlayer()
        {
        }
    }
}
