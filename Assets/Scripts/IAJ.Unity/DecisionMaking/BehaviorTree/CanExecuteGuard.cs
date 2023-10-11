using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AI;
using Assets.Scripts.Game.NPCs;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree
{
    public class CanExecuteGuard : Decorator {

        Monster monster;

        public CanExecuteGuard(Monster character, Task child) : base(child) {
            monster = character;
        }

        public override Result Run()
        {
            if (monster.usingFormation && !monster.formationLeader)
            {
                return Result.Failure;
            }
            return this.child.Run();
        }

    }
}