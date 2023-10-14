using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private float expectedHPChange;
        private float expectedXPChange;
        private int xpChange;
        private int manaChange;

        public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite",character,target)
        {
            if (target.CompareTag("Skeleton"))
            {
                this.expectedHPChange = 3.5f;
                this.xpChange = 3;
                this.manaChange = 2;
                this.expectedXPChange = 2.7f;
            }
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            int mana = Character.baseStats.Mana;
            return mana >= this.manaChange;
        }

        public override bool CanExecute(WorldModelImproved WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;
            int mana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            return mana >= this.manaChange;
        }

        public override bool CanExecute(WorldModel WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;
            int mana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            return mana >= this.manaChange;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change += -this.expectedXPChange;
            }
            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += -this.expectedHPChange;
            }
            return change;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.DivineSmite(this.Target);
        }

        public override void ApplyActionEffects(WorldModelImproved WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);
            int mana = (int)WorldModelImproved.GetProperty(Properties.MANA);

            //there was an hit, enemy is destroyed, gain xp, spend mana
            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(this.Target.name, false);
            WorldModelImproved.SetProperty(Properties.XP, xp + this.xpChange);
            WorldModelImproved.SetProperty(Properties.MANA, mana - this.manaChange);
        }

        public override void ApplyActionEffects(WorldModel WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);
            int mana = (int)WorldModelImproved.GetProperty(Properties.MANA);

            //there was an hit, enemy is destroyed, gain xp, spend mana
            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(this.Target.name, false);
            WorldModelImproved.SetProperty(Properties.XP, xp + this.xpChange);
            WorldModelImproved.SetProperty(Properties.MANA, mana - this.manaChange);
        }

        public override float GetHValue(WorldModelImproved WorldModelImproved)
        {
            var hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHp = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            var mana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);
            
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);

            return hp/(float)maxHp * 0.3f + ((float) Math.Min(level * 10/this.expectedXPChange, 1)) * 0.2f + mana/(float)maxMana * 0.1f + base.GetHValue(WorldModelImproved) * 0.4f;
        }

        public override float GetHValue(WorldModel WorldModelImproved)
        {
            var hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHp = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            var mana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);
            
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);

            return hp/(float)maxHp * 0.3f + ((float) Math.Min(level * 10/this.expectedXPChange, 1)) * 0.2f + mana/(float)maxMana * 0.1f + base.GetHValue(WorldModelImproved) * 0.4f;
        }
    }
}
