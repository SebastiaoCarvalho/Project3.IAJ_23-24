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

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            int level = (int)worldModel.GetProperty(Properties.LEVEL);
            int mana = (int)worldModel.GetProperty(Properties.MANA);
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

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            Vector3 position = GameManager.Instance.initialPosition;
            int mana = (int)worldModel.GetProperty(Properties.MANA);

            worldModel.SetProperty(Properties.MANA, mana - this.manaChange);
            worldModel.SetProperty(Properties.POSITION, position);
        }

        public override float GetHValue(WorldModel worldModel) // TODO : MCTS
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            
            if (hp > 5)
            {
                return base.GetHValue(worldModel)/1.5f;
            }
            return 10.0f;
        }
    }
}
