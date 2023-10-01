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
using UnityEngine.UI;


namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees
{
   class PatrolAndReactTree : ParallelTask
    {
        public PatrolAndReactTree(Orc character, GameObject target, Vector3 position1, Vector3 position2, List<Orc> otherOrcs) {
            // To create a new tree you need to create each branck which is done using the constructors of different tasks
            // Additionally it is possible to create more complex behaviour by combining different tasks and composite tasks...
    
            Interrupter PatrolBehavior = new Interrupter(new PatrolTree(character, position1, position2));
            Interrupter ListenBehavior = new Interrupter(new Sequence(new List<Task>{
                new UntilSuccess(new ReactTree(
                    new HearShout(character), 
                    new List<Interrupter>(){PatrolBehavior})), 
                new MoveToShout(character, 5.0f)
            }));
            Sequence LookBehavior = new Sequence(new List<Task>{
                new UntilSuccess(new ReactTree(
                    new IsCharacterNearTarget(character, target, character.enemyStats.AwakeDistance),
                    new List<Interrupter>() {PatrolBehavior, ListenBehavior})),
                new OrcAttackTree(character, target, otherOrcs)
            });

            this.children = new List<Task>()
            {
                PatrolBehavior,
                ListenBehavior,
                LookBehavior
            };

        }

    }
}
