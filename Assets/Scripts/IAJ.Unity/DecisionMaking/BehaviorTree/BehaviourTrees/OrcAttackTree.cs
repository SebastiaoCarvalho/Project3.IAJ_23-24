using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks.OrcTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Game;
using Assets.Scripts.Game.NPCs;


namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees
{
   class OrcAttackTree : Sequence
    {
        private Orc character;

        public OrcAttackTree(Orc character, GameObject target, List<Orc> otherOrcs) {
            this.character = character;
            this.children = new List<Task>()
            {
                new Shout(character, otherOrcs),
                new Pursue(character, target, character.enemyStats.WeaponRange), 
                new LightAttack(character)
            };
        }

        public override Result Run()
        {
            character.IsAttacking = true;
            character.HeardShout = false;

            Result result = base.Run();
            if (result == Result.Failure) {
                character.IsAttacking = false;
            }

            return result;
        } 
    }
}