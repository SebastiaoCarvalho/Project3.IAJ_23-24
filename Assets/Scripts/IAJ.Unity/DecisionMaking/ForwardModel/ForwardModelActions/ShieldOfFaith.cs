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
            int mana = Character.baseStats.Mana;
            return Character.baseStats.ShieldHP < Character.baseStats.MaxShieldHp && mana >= this.manaChange ;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            int shieldHP = (int)worldModel.GetProperty(Properties.ShieldHP);
            int maxShieldHP = (int)worldModel.GetProperty(Properties.MaxShieldHP);
            int mana = (int)worldModel.GetProperty(Properties.MANA);
            return shieldHP < maxShieldHP && mana >= this.manaChange; 
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
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, Math.Max(0, (int)worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) - this.expectedShieldChange));
        }

        public override float GetHValue(WorldModel worldModel)
        {
            // if low hp and low shields

            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxHp = (int)worldModel.GetProperty(Properties.MAXHP);
            var shieldHp = (int)worldModel.GetProperty(Properties.ShieldHP);
            var maxShieldHp = (int)worldModel.GetProperty(Properties.MaxShieldHP);

            return  hp/(float)maxHp * 0.8f + shieldHp/(float)maxShieldHp * 0.2f;
        }
    }
}
