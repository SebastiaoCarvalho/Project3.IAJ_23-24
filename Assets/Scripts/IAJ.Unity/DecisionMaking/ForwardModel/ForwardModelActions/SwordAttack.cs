﻿using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class SwordAttack : WalkToTargetAndExecuteAction
    {
        private float expectedHPChange;
        private float expectedXPChange;
        private int xpChange;
        private int enemyAC;
        private int enemySimpleDamage;
        //how do you like lambda's in c#?
        private Func<int> dmgRoll;

        public SwordAttack(AutonomousCharacter character, GameObject target) : base("SwordAttack",character,target)
        {
            if (target.tag.Equals("Skeleton"))
            {
                this.dmgRoll = () => RandomHelper.RollD6();
                this.enemySimpleDamage = 3;
                this.expectedHPChange = 3.5f;
                this.xpChange = 3;
                this.expectedXPChange = 2.7f;
                this.enemyAC = 10;
            }
            else if (target.tag.Equals("Orc"))
            {
                this.dmgRoll = () => RandomHelper.RollD10() + 2;
                this.enemySimpleDamage = 6;
                this.expectedHPChange = 7.5f;
                this.xpChange = 10;
                this.expectedXPChange = 7.0f;
                this.enemyAC = 14;
            }
            else if (target.tag.Equals("Dragon"))
            {
                this.dmgRoll = () => RandomHelper.RollD12() + RandomHelper.RollD12();
                this.enemySimpleDamage = 12;
                this.expectedHPChange = 13.0f;
                this.xpChange = 20;
                this.expectedXPChange = 10.0f;
                this.enemyAC = 18;
            }
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += this.expectedHPChange;
            }
            else if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change += -this.expectedXPChange;
            }

            return change;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.SwordAttack(this.Target);
        }

        public override void ApplyActionEffects(WorldModelImproved WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            int hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            int shieldHp = (int)WorldModelImproved.GetProperty(Properties.ShieldHP);
            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);

            int damage = 0;
            if (GameManager.Instance.StochasticWorld)
            {
                //execute the lambda function to calculate received damage based on the creature type
                damage = this.dmgRoll.Invoke();
            }
            else
            {
                damage = this.enemySimpleDamage;
            }
            //calculate player's damage
            int remainingDamage = damage - shieldHp;
            int remainingShield = Mathf.Max(0, shieldHp - damage);
            int remainingHP;

            if(remainingDamage > 0)
            {
                remainingHP = hp - remainingDamage;
                WorldModelImproved.SetProperty(Properties.HP, remainingHP);
            }

            WorldModelImproved.SetProperty(Properties.ShieldHP, remainingShield);
            var surviveValue = WorldModelImproved.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue + remainingDamage);

            //calculate Hit
            //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
            int attackRoll = RandomHelper.RollD20() + 7;

            if (attackRoll >= enemyAC || ! GameManager.Instance.StochasticWorld)
            {
                //there was an hit, enemy is destroyed, gain xp
                //disables the target object so that it can't be reused again
                WorldModelImproved.SetProperty(this.Target.name, false);
                WorldModelImproved.SetProperty(Properties.XP, xp + this.xpChange);
                var xpValue = WorldModelImproved.GetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL);
                WorldModelImproved.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, xpValue - this.xpChange);
            }
        }

        public override void ApplyActionEffects(WorldModel WorldModelImproved)
        {
            base.ApplyActionEffects(WorldModelImproved);

            int hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            int shieldHp = (int)WorldModelImproved.GetProperty(Properties.ShieldHP);
            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);

            int damage = 0;
            if (GameManager.Instance.StochasticWorld)
            {
                //execute the lambda function to calculate received damage based on the creature type
                damage = this.dmgRoll.Invoke();
            }
            else
            {
                damage = this.enemySimpleDamage;
            }
            //calculate player's damage
            int remainingDamage = damage - shieldHp;
            int remainingShield = Mathf.Max(0, shieldHp - damage);
            int remainingHP;

            if(remainingDamage > 0)
            {
                remainingHP = hp - remainingDamage;
                WorldModelImproved.SetProperty(Properties.HP, remainingHP);
            }

            WorldModelImproved.SetProperty(Properties.ShieldHP, remainingShield);
            var surviveValue = WorldModelImproved.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue + remainingDamage);

            //calculate Hit
            //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
            int attackRoll = RandomHelper.RollD20() + 7;

            if (attackRoll >= enemyAC || ! GameManager.Instance.StochasticWorld)
            {
                //there was an hit, enemy is destroyed, gain xp
                //disables the target object so that it can't be reused again
                WorldModelImproved.SetProperty(this.Target.name, false);
                WorldModelImproved.SetProperty(Properties.XP, xp + this.xpChange);
                var xpValue = WorldModelImproved.GetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL);
                WorldModelImproved.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, xpValue - this.xpChange);
            }
        }

        public override float GetHValue(WorldModelImproved WorldModelImproved)
        {
            var hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHp = (int)WorldModelImproved.GetProperty(Properties.HP);

            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);


            if (hp > this.expectedHPChange) // you should survive
            {
                return base.GetHValue(WorldModelImproved) * 0.5f + ((float) Math.Min(this.expectedHPChange/maxHp, 1)) * 0.3f + ((float) Math.Min(level * 10/this.expectedXPChange, 1)) * 0.2f; // normalize from 0 to 1
            }
            return 10.0f;
        }

        public override float GetHValue(WorldModel WorldModelImproved)
        {
            var hp = (int)WorldModelImproved.GetProperty(Properties.HP);
            var maxHp = (int)WorldModelImproved.GetProperty(Properties.HP);

            int xp = (int)WorldModelImproved.GetProperty(Properties.XP);
            int level = (int)WorldModelImproved.GetProperty(Properties.LEVEL);


            if (hp > this.expectedHPChange) // you should survive
            {
                return base.GetHValue(WorldModelImproved) * 0.5f + ((float) Math.Min(this.expectedHPChange/maxHp, 1)) * 0.3f + ((float) Math.Min(level * 10/this.expectedXPChange, 1)) * 0.2f; // normalize from 0 to 1
            }
            return 10.0f;
        }
    }
}
