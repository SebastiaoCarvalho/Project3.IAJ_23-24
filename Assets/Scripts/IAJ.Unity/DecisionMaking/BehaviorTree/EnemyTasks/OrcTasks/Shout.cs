using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Game.NPCs;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks.OrcTasks
{
    class Shout : Task
    {
        protected Orc character { get; set; }

        public List<Orc> targets { get; set; }

        public Shout(Orc character, List<Orc> targets)
        {
            this.character = character;
            this.targets = targets;
        }
        public override Result Run()
        {
            character.Shout(targets);
            
            return Result.Success;
        }

    }
}