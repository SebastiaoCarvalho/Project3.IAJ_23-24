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
        

        public override bool CanExecute(WorldModelImproved WorldModelImproved)
        {
            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);

            return xp >= level * 10;
        }

        public override bool CanExecute(WorldModel WorldModelImproved)
        {
            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);

            return xp >= level * 10;
        }

        public override void Execute()
        {
            GameManager.Instance.LevelUp();
        }

        public override void ApplyActionEffects(WorldModelImproved WorldModelImproved)
        {
            int maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);
            float time = (float)WorldModelImproved.GetProperty(Properties.TIME);

            WorldModelImproved.SetProperty(Properties.LEVEL, level + 1);
            WorldModelImproved.SetProperty(Properties.MAXHP, maxHP + 10);
            WorldModelImproved.SetProperty(Properties.XP, (int)0);
            WorldModelImproved.SetProperty(Properties.TIME, time + this.Duration);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, 0);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, (int)WorldModelImproved.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) + Duration);
        }

        public override void ApplyActionEffects(WorldModel WorldModelImproved)
        {
            int maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);
            float time = (float)WorldModelImproved.GetProperty(Properties.TIME);

            WorldModelImproved.SetProperty(Properties.LEVEL, level + 1);
            WorldModelImproved.SetProperty(Properties.MAXHP, maxHP + 10);
            WorldModelImproved.SetProperty(Properties.XP, (int)0);
            WorldModelImproved.SetProperty(Properties.TIME, time + this.Duration);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, 0);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, (int)WorldModelImproved.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) + Duration);
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

        public override float GetHValue(WorldModelImproved WorldModelImproved)
        {
            // if you are close to leveling up, choose this
            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);
            return xp > level * 10 - 5 ? -10 : 10;
            
        }

        public override float GetHValue(WorldModel WorldModelImproved)
        {
            // if you are close to leveling up, choose this
            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);
            return xp > level * 10 - 5 ? -10 : 10;
            
        }
    }
}
