using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetManaPotion : WalkToTargetAndExecuteAction
    {
        public GetManaPotion(AutonomousCharacter character, GameObject target) : base("GetManaPotion",character,target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return Character.baseStats.Mana < Character.baseStats.MaxMana;
        }

        public override bool CanExecute(WorldModelImproved WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;

            var currentMana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);
            return currentMana < maxMana;
        }

        public override bool CanExecute(WorldModel WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;

            var currentMana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);
            return currentMana < maxMana;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.GetManaPotion(this.Target);
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change -= 5.0f;
            }
 

            return change;
        }

        public override void ApplyActionEffects(WorldModelImproved WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);
            WorldModelImproved.SetProperty(Properties.MANA, maxMana);

            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(this.Target.name, false);
        }

        public override void ApplyActionEffects(WorldModel WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);
            WorldModelImproved.SetProperty(Properties.MANA, maxMana);

            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModelImproved WorldModelImproved)
        {
            var currentMana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);

            return currentMana / (float) maxMana * 0.5f  + base.GetHValue(WorldModelImproved) * 0.5f;
        }

        public override float GetHValue(WorldModel WorldModelImproved)
        {
            var currentMana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);

            return currentMana / (float) maxMana * 0.5f  + base.GetHValue(WorldModelImproved) * 0.5f;
        }
    }
}
