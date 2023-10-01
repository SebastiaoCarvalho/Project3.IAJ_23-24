using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Game.NPCs;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks.OrcTasks
{
    class HearShout : Task
    {
        protected Orc character { get; set; }

        public HearShout(Orc character)
        {
            this.character = character;
        }
        public override Result Run()
        {
            if (character.HeardShout) {
                character.HeardShout = false;
                return Result.Success;
            }
            return Result.Failure;
        }

    }
}