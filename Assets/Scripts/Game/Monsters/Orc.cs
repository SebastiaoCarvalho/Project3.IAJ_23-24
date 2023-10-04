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
        public bool HeardShout;
        public Vector3 ShoutPosition;
        public bool IsAttacking;

        public GameObject AlertSprite;

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
            this.HeardShout = false;
            this.IsAttacking = false;
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
            var gameObjs = GameObject.FindGameObjectsWithTag("Orc");
            List<Orc> orcs = new List<Orc>();
            foreach (var orc in gameObjs) {
                if (orc.GetComponent<Orc>() != this)
                    orcs.Add(orc.GetComponent<Orc>());
            }

            this.BehaviourTree = new PatrolAndReactTree(this, Target, position1, position2, orcs);
         }

         public void Shout(List<Orc> targets) {
            var shout = this.GetComponent<AudioSource>();
            shout.Play();

            AlertSprite.transform.position = this.transform.position;
            var animation = Instantiate(AlertSprite);
            Destroy(animation, 2);

            foreach(Orc target in targets)
                target.hearShout(this);
         }

         public void hearShout(Orc source) {
            if (!this.IsAttacking) {
                HeardShout = true;
                ShoutPosition = source.transform.position;
            }
         }

    }
}
