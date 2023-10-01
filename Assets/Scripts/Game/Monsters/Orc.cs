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

    public class Orc : Monster
    {
        public Orc()
        {
            this.enemyStats.Type = "Orc";
            this.enemyStats.XPvalue = 8;
            this.enemyStats.AC = 14;
            this.baseStats.HP = 15;
            this.DmgRoll = () => RandomHelper.RollD10() + 2;
            this.enemyStats.SimpleDamage = 6;
            this.enemyStats.AwakeDistance = 15;
            this.enemyStats.WeaponRange = 3;
        }

        public override void InitializeBehaviourTree()
        {
            var patrols = GameObject.FindGameObjectsWithTag("Patrol");

            float pos = float.MaxValue;
            GameObject closest = null;
            foreach (var p in patrols)
            {
                if (Vector3.Distance(this.agent.transform.position, p.transform.position) < pos)
                {
                    pos = Vector3.Distance(this.agent.transform.position, p.transform.position);
                    closest = p;
                }

            }

            var position1 = closest.transform.GetChild(0).position;
            var position2 = closest.transform.GetChild(1).position;

            //Create a Behavior tree that combines Patrol with other behaviors...
            this.BehaviourTree = new PatrolAndReactTree(this, Target, position1, position2);
         }

    }
}
