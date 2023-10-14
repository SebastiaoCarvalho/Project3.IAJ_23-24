using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class Rest : Action
    {
        public AutonomousCharacter Character;
        private float expectedHPChange;

        public Rest(AutonomousCharacter character) : base("Rest")
        {
            this.Character = character;
            this.Duration = AutonomousCharacter.RESTING_INTERVAL;
            this.expectedHPChange = AutonomousCharacter.REST_HP_RECOVERY;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return Character.baseStats.HP < Character.baseStats.MaxHP;
        }

        public override bool CanExecute(WorldModelImproved WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;
            int hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            int maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            return hp < maxHP;
        }

        public override bool CanExecute(WorldModel WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;
            int hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            int maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            return hp < maxHP;
        }


        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) {
                change += - this.expectedHPChange;
            }
            else if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                change += this.Duration;
            }
            return change;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.Rest();
        }

        public override void ApplyActionEffects(WorldModelImproved WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            int hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            int maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            float time = (float)WorldModelImproved.GetProperty(Properties.TIME);

            //there was an hit, enemy is destroyed, gain xp, spend mana
            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(Properties.HP, (int) Math.Max(hp + this.expectedHPChange, maxHP));
            WorldModelImproved.SetProperty(Properties.TIME, time + Duration);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, Math.Max(0, (int)WorldModelImproved.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) - this.expectedHPChange));
            WorldModelImproved.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, (int)WorldModelImproved.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) + Duration);
        }

        public override void ApplyActionEffects(WorldModel WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            int hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            int maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            float time = (float)WorldModelImproved.GetProperty(Properties.TIME);

            //there was an hit, enemy is destroyed, gain xp, spend mana
            //disables the target object so that it can't be reused again
            WorldModelImproved.SetProperty(Properties.HP, (int) Math.Max(hp + this.expectedHPChange, maxHP));
            WorldModelImproved.SetProperty(Properties.TIME, time + Duration);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, Math.Max(0, (int)WorldModelImproved.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) - this.expectedHPChange));
            WorldModelImproved.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, (int)WorldModelImproved.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) + Duration);
        }

        public override float GetHValue(WorldModelImproved WorldModelImproved)
        {
            var hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            
            return 0.3f + hp / (float) maxHP * 0.7f;
        }

        public override float GetHValue(WorldModel WorldModelImproved)
        {
            var hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            
            return 0.3f + hp / (float) maxHP * 0.7f;
        }
    }
}
