using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion",character,target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return Character.baseStats.HP < Character.baseStats.MaxHP;
        }

        public override bool CanExecute(WorldModelImproved WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;

            var currentHP = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            return currentHP < maxHP;
        }

        public override bool CanExecute(WorldModel WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;

            var currentHP = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            return currentHP < maxHP;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.GetHealthPotion(this.Target);
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change -= goal.InsistenceValue;
            }
 
            return change;
        }

        public override void ApplyActionEffects(WorldModelImproved WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            WorldModelImproved.SetProperty(Properties.HP, maxHP);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, 0.0f);

            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(this.Target.name, false);
        }

        public override void ApplyActionEffects(WorldModel WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            WorldModelImproved.SetProperty(Properties.HP, maxHP);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, 0.0f);

            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModelImproved WorldModelImproved)
        {
            var currentHP = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);

            return currentHP / maxHP * 0.5f + base.GetHValue(WorldModelImproved) * 0.5f;
        }

        public override float GetHValue(WorldModel WorldModelImproved)
        {
            var currentHP = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);

            return currentHP / maxHP * 0.5f + base.GetHValue(WorldModelImproved) * 0.5f;
        }
    }
}
