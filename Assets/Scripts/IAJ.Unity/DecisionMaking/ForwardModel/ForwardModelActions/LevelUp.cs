using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class LevelUp : Action
    {
        public AutonomousCharacter Character { get; private set; }

        public LevelUp(AutonomousCharacter character) : base("LevelUp")
        {
            this.Character = character;
            this.Duration = AutonomousCharacter.LEVELING_INTERVAL;
        }

        public override bool CanExecute()
        {
            var level = Character.baseStats.Level;
            var xp = Character.baseStats.XP;

            return xp >= level * 10;
        }
        

        public override bool CanExecute(WorldModel worldModel)
        {
            int xp = (int)worldModel.GetProperty(Properties.XP);
            int level = (int)worldModel.GetProperty(Properties.LEVEL);

            return xp >= level * 10;
        }

        public override void Execute()
        {
            GameManager.Instance.LevelUp();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            int maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
            int level = (int)worldModel.GetProperty(Properties.LEVEL);
            float time = (float)worldModel.GetProperty(Properties.TIME);

            worldModel.SetProperty(Properties.LEVEL, level + 1);
            worldModel.SetProperty(Properties.MAXHP, maxHP + 10);
            worldModel.SetProperty(Properties.XP, (int)0);
            worldModel.SetProperty(Properties.TIME, time + this.Duration);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, 0);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, (int)worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) + Duration);
        }

        public override float GetGoalChange(Goal goal)
        {
            float change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change = -goal.InsistenceValue;
            }
            else if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                change += this.Duration;
            }
            return change;
        }

        public override float GetHValue(WorldModel worldModel) // FIXME : maybe factor in time and other stuff?
        {
            // if you are close to leveling up, choose this
            int xp = (int)worldModel.GetProperty(Properties.XP);
            int level = (int)worldModel.GetProperty(Properties.LEVEL);
            return xp > level * 10 ? 0.0f : 1.0f;
            
        }
    }
}
