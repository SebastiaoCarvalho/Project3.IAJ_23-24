using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AI;
using Assets.Scripts.Game.NPCs;
using Assets.Scripts.Game;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks
{
    class MoveToShout : MoveTo
    {
        public Orc orc;
        public MoveToShout(Orc character, float _range) : base(character, character.transform.position, _range)
        {
            this.Character = character;
            this.orc = character;
            range = _range;
        }

        public override Result Run()
        {
            Target = orc.ShoutPosition;
            return base.Run();
        }

    }
}