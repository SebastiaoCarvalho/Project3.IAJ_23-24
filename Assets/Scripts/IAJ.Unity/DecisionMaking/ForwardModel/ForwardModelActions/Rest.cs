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

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            int hp = (int)worldModel.GetProperty(Properties.HP);
            int maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
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

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int hp = (int)worldModel.GetProperty(Properties.HP);
            int maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
            float time = (float)worldModel.GetProperty(Properties.TIME);

            //there was an hit, enemy is destroyed, gain xp, spend mana
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(Properties.HP, (int) Math.Max(hp + this.expectedHPChange, maxHP));
            worldModel.SetProperty(Properties.TIME, time + Duration);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, Math.Max(0, (int)worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) - this.expectedHPChange));
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, (int)worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) + Duration);
        }

        public override float GetHValue(WorldModel worldModel) // TODO : MCTS
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
            
            return 0.3f + hp / (float) maxHP * 0.7f;
        }
    }
}
