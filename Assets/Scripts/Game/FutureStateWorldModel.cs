using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class FutureStateWorldModel : WorldModel
    {
        protected GameManager GameManager { get; set; }
        protected int NextPlayer { get; set; }
        protected Action NextEnemyAction { get; set; }
        protected Action[] NextEnemyActions { get; set; }

        public FutureStateWorldModel(GameManager gameManager, List<Action> actions) : base(actions)
        {
            this.GameManager = gameManager;
            this.NextPlayer = 0;
        }

        public FutureStateWorldModel(FutureStateWorldModel parent) : base(parent)
        {
            this.GameManager = parent.GameManager;
        }

        public override WorldModel GenerateChildWorldModel()
        {
            return new FutureStateWorldModel(this);
        }

        public override bool IsTerminal()
        {
            int HP = (int)this.GetProperty(Properties.HP);
            float time = (float)this.GetProperty(Properties.TIME);
            int money = (int)this.GetProperty(Properties.MONEY);

            return HP <= 0 ||  time >= GameManager.GameConstants.TIME_LIMIT || (this.NextPlayer == 0 && money == 25);
        }

        public override float GetScore()
        {
            int money = (int)this.GetProperty(Properties.MONEY);
            int HP = (int)this.GetProperty(Properties.HP);
            float time = (float)this.GetProperty(Properties.TIME);

            if (HP <= 0 || time >= GameManager.GameConstants.TIME_LIMIT) //lose
                return 0.0f;
            else if (this.NextPlayer == 0 && money == 25 && HP > 0) //win
                return 1.0f;
            else { // non-terminal state
                return timeAndMoneyScore(time, money) * levelScore() * hpScore(HP) * timeScore(time);          
            }
        }

        private float timeAndMoneyScore(float time, int money) {
            float relationTimeMoney = time - 6 * money;

            if (relationTimeMoney > 30)
                return 0;
            else if (relationTimeMoney < 0)
                return 0.6f;
            else
                return 0.3f;
        }

        private float timeScore(float time) {
            return (1 - time / GameManager.GameConstants.TIME_LIMIT);
        }

        private float levelScore() {
            int level = (int)this.GetProperty(Properties.LEVEL);
            if (level == 2)
                return 1f;
            else if (level == 1)
                return 0.4f;
            else
                return 0; 
        }

        private float hpScore(int hp) {
            if (hp > 18) //survives orc and dragon
                return 1f;
            if (hp > 12) //survives dragon or two orcs
                return 0.6f;
            else if (hp > 6) //survives orc
                return 0.1f;
            else
                return 0.01f;

        }

        public override int GetNextPlayer()
        {
            return this.NextPlayer;
        }

        public override void CalculateNextPlayer()
        {
            Vector3 position = (Vector3)this.GetProperty(Properties.POSITION);
            bool enemyEnabled;
            if (GameManager.SleepingNPCs){
                this.NextPlayer = 0;
                return;
            } 
            //basically if the character is close enough to an enemy, the next player will be the enemy.
            foreach (var enemy in this.GameManager.enemies)
            {
                enemyEnabled = (bool) this.GetProperty(enemy.name);
                if (enemyEnabled && (enemy.transform.position - position).sqrMagnitude <= 100)
                {
                    this.NextPlayer = 1;
                    this.NextEnemyAction = new EnemyAttack(this.GameManager.Character, enemy);
                    this.NextEnemyActions = new Action[] { this.NextEnemyAction };
                    return; 
                }
            }
            this.NextPlayer = 0;
            //if not, then the next player will be player 0
        }

        public override Action GetNextAction()
        {
            Action action;
            if (this.NextPlayer == 1)
            {
                action = this.NextEnemyAction;
                this.NextEnemyAction = null;
                return action;
            }
            else return base.GetNextAction();
        }

        public override Action[] GetExecutableActions()
        {
            if (this.NextPlayer == 1)
            {
                return this.NextEnemyActions;
            }
            else return base.GetExecutableActions();
        }

    }
}
