using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks;
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
   class ReactTree : Sequence
    {
        public ReactTree(Monster character, GameObject target, Interrupter interrupter) {
            // To create a new tree you need to create each branck which is done using the constructors of different tasks
            // Additionally it is possible to create more complex behaviour by combining different tasks and composite tasks...
            this.children = new List<Task>()
            {
                new IsCharacterNearTarget(character, target, character.enemyStats.AwakeDistance),
                new PerformInterruption(interrupter, Result.Failure),
                new SuccessTask(new Sequence(new List<Task>()
                        {
                            new Pursue(character, target, character.enemyStats.WeaponRange),
                            new LightAttack(character)
                        })
                    )
            };

        }

    }
}
