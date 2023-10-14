using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class PickUpChest : WalkToTargetAndExecuteAction
    {

        public PickUpChest(AutonomousCharacter character, GameObject target) : base("PickUpChest",character,target)
        {
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.GET_RICH_GOAL) {change -= 5.0f;}

            return change;
        }

        public override bool CanExecute()
        {

            if (!base.CanExecute())
                return false;
            return true;
        }

        public override bool CanExecute(WorldModelImproved WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;
            return true;
        }

        public override bool CanExecute(WorldModel WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;
            return true;
        }

        public override void Execute()
        {
            
            base.Execute();
            GameManager.Instance.PickUpChest(this.Target);
        }

        public override void ApplyActionEffects(WorldModelImproved WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            var goalValue = WorldModelImproved.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL, goalValue - 5.0f);
            var money = (int)WorldModelImproved.GetProperty(Properties.MONEY);
            WorldModelImproved.SetProperty(Properties.MONEY, money + 5);

            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(this.Target.name, false);
        }

        public override void ApplyActionEffects(WorldModel WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            var goalValue = WorldModelImproved.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL, goalValue - 5.0f);
            var money = (int)WorldModelImproved.GetProperty(Properties.MONEY);
            WorldModelImproved.SetProperty(Properties.MONEY, money + 5);

            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModelImproved WorldModelImproved)
        {
            // get money property if 20 go for it
            // else return a lower value that's still decent
            // count with distance
            var money = (int)WorldModelImproved.GetProperty(Properties.MONEY);
            var baseValue = base.GetHValue(WorldModelImproved);

            if (money == 20 || baseValue < 0.1) {
                return baseValue * 0.01f;
            }
            return baseValue;
        }

        public override float GetHValue(WorldModel WorldModelImproved)
        {
            // get money property if 20 go for it
            // else return a lower value that's still decent
            // count with distance
            var money = (int)WorldModelImproved.GetProperty(Properties.MONEY);
            var baseValue = base.GetHValue(WorldModelImproved);

            if (money == 20 || baseValue < 0.1) {
                return baseValue * 0.01f;
            }
            return baseValue;
        }
    }
}
