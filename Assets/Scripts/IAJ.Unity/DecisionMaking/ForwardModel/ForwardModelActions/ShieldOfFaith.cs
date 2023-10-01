using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class ShieldOfFaith : Action
    {
        private float expectedShieldChange;
        private int manaChange;
        public AutonomousCharacter Character;

        public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith")
        {
            this.manaChange = 5;
            this.Character = character;
            this.expectedShieldChange = 5; 
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return Character.baseStats.ShieldHP < Character.baseStats.MaxShieldHp;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            int shieldHP = (int)worldModel.GetProperty(Properties.ShieldHP);
            int maxShieldHP = (int)worldModel.GetProperty(Properties.MaxShieldHP);
            return shieldHP < maxShieldHP; 
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) {
                change += - this.expectedShieldChange;
            }
            return change;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.ShieldOfFaith();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int mana = (int)worldModel.GetProperty(Properties.MANA);
            int maxShieldHP = (int)worldModel.GetProperty(Properties.MaxShieldHP);

            //there was an hit, enemy is destroyed, gain xp, spend mana
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(Properties.ShieldHP, maxShieldHP);
            worldModel.SetProperty(Properties.MANA, mana - this.manaChange);
        }

        public override float GetHValue(WorldModel worldModel) // TODO : MCTS
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            
            if (hp > this.expectedShieldChange)
            {
                return base.GetHValue(worldModel)/1.5f;
            }
            return 10.0f;
        }
    }
}
