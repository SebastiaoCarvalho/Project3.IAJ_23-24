using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class Teleport : Action
    {
        private int manaChange;
        public AutonomousCharacter Character;

        public Teleport(AutonomousCharacter character) : base("Teleport")
        {
            this.manaChange = 5;
            this.Character = character;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            int mana = Character.baseStats.Mana;
            return Character.baseStats.Level > 1 && mana >= this.manaChange ;
        }

        public override bool CanExecute(WorldModelImproved WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);
            int mana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            return level > 1 && mana >= this.manaChange; 
        }

        public override bool CanExecute(WorldModel WorldModelImproved)
        {
            if (!base.CanExecute(WorldModelImproved)) return false;
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);
            int mana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            return level > 1 && mana >= this.manaChange; 
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            // FIXME : add goals?
            return change;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.Teleport();
        }

        public override void ApplyActionEffects(WorldModelImproved WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            Vector3 position = GameManager.Instance.initialPosition;
            int mana = (int)WorldModelImproved.GetProperty(Properties.MANA);

            WorldModelImproved.SetProperty(Properties.MANA, mana - this.manaChange);
            WorldModelImproved.SetProperty(Properties.POSITION, position);
        }

        public override void ApplyActionEffects(WorldModel WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            Vector3 position = GameManager.Instance.initialPosition;
            int mana = (int)WorldModelImproved.GetProperty(Properties.MANA);

            WorldModelImproved.SetProperty(Properties.MANA, mana - this.manaChange);
            WorldModelImproved.SetProperty(Properties.POSITION, position);
        }

        public override float GetHValue(WorldModelImproved WorldModelImproved)
        {
            var hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            var mana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);
            var money = (int)WorldModelImproved.GetProperty(Properties.MONEY);

            return (hp/(float)maxHP) * 0.6f + (mana/(float)maxMana) * 0.1f + money/(float)25 * 0.3f;
        }

        public override float GetHValue(WorldModel WorldModelImproved)
        {
            var hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHP = (int)WorldModelImproved.GetProperty(Properties.MAXHP);
            var mana = (int)WorldModelImproved.GetProperty(Properties.MANA);
            var maxMana = (int)WorldModelImproved.GetProperty(Properties.MAXMANA);
            var money = (int)WorldModelImproved.GetProperty(Properties.MONEY);

            return (hp/(float)maxHP) * 0.6f + (mana/(float)maxMana) * 0.1f + money/(float)25 * 0.3f;
        }
    }
}
