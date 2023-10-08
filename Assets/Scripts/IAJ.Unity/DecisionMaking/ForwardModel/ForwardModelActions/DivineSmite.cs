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

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            int mana = (int)worldModel.GetProperty(Properties.MANA);
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

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int xp = (int)worldModel.GetProperty(Properties.XP);
            int mana = (int)worldModel.GetProperty(Properties.MANA);

            //there was an hit, enemy is destroyed, gain xp, spend mana
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
            worldModel.SetProperty(Properties.XP, xp + this.xpChange);
            worldModel.SetProperty(Properties.MANA, mana - this.manaChange);
        }

        public override float GetHValue(WorldModel worldModel) // TODO : MCTS
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxHp = (int)worldModel.GetProperty(Properties.MAXHP);
            
            int level = (int)worldModel.GetProperty(Properties.LEVEL);

            return - (this.expectedHPChange/maxHp) * 0.6f - (this.expectedXPChange)/(level * 10) * 0.4f + base.GetHValue(worldModel);
        }
    }
}
