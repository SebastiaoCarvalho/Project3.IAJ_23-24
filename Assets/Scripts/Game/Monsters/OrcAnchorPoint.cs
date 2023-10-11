using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine.AI;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees;
//using Assets.Scripts.IAJ.Unity.Formations;
using System.Collections.Generic;

namespace Assets.Scripts.Game.NPCs
{

    public class OrcAnchorPoint : Monster
    {
        public OrcAnchorPoint()
        {
            this.enemyStats.Type = "AnchorPoint";
        }

        public override void InitializeBehaviourTree()
        {
            var patrol1 = GameObject.Find("Patrol1").transform.GetChild(0).position;
            var patrol2 = GameObject.Find("Patrol2").transform.GetChild(0).position;

            this.BehaviourTree = new PatrolTree(this, patrol1, patrol2);
         }
    }
}
