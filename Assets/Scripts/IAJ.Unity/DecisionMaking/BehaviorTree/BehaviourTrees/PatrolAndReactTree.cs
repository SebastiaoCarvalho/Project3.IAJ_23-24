using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Game;
using Assets.Scripts.Game.NPCs;
using UnityEngine.UI;


namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees
{
   class PatrolAndReactTree : ParallelTask
    {
        public PatrolAndReactTree(Monster character, GameObject target, Vector3 position1, Vector3 position2) {
            // To create a new tree you need to create each branck which is done using the constructors of different tasks
            // Additionally it is possible to create more complex behaviour by combining different tasks and composite tasks...
            Interrupter PatrolBehavior = new Interrupter(new PatrolTree(character, position1, position2));
            Sequence ReactBehavior = new Sequence(new List<Task>{
                new UntilSuccess(new ReactTree(character, target, PatrolBehavior)), 
                new Sequence(new List<Task>{
                    new Pursue(character, target, character.enemyStats.WeaponRange), 
                    new LightAttack(character)
                })
            });

            this.children = new List<Task>()
            {
                ReactBehavior,
                PatrolBehavior
            };

        }

    }
}
