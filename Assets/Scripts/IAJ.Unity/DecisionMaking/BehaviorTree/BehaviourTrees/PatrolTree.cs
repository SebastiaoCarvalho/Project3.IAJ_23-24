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
   class PatrolTree : Sequence
    {
        public PatrolTree(Monster character, Vector3 position1, Vector3 position2) {
            // To create a new tree you need to create each branck which is done using the constructors of different tasks
            // Additionally it is possible to create more complex behaviour by combining different tasks and composite tasks...
            this.children = new List<Task>()
            {
                new MoveTo(character, position1, 1.0f),
                new MoveTo(character, position2, 1.0f)
            };

        }

    }
}
