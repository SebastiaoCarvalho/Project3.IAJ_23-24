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

        private float timeWeight = 20f;
        private float moneyWeight = 15;
        private float levelWeight = 3f;
        private float xpWeight = 3f;
        private float manaWeight = 0.5f;
        private float hpWeight = 2f;
        private float shieldWeight = 1f;

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
                float weightSum = timeWeight + moneyWeight + levelWeight + 
                    xpWeight + manaWeight + hpWeight + shieldWeight;

                // Weighted mean
                return (timeScore() + moneyScore() + levelScore() + xpScore() +
                         manaScore() + hpScore() + shieldScore()) / weightSum;
            }
        }

        private float timeScore() {
            return timeWeight * (1 - (float)this.GetProperty(Properties.TIME) / GameManager.GameConstants.TIME_LIMIT);
        }

        private float moneyScore() {
            return moneyWeight * ((int)this.GetProperty(Properties.MONEY) / 25f);
        }

        private float levelScore() {
            return levelWeight * (int)this.GetProperty(Properties.LEVEL) == 2 ? 1 : 0;
        }

        private float xpScore() {
            return xpWeight * (int)this.GetProperty(Properties.LEVEL) < 2 ? 
                (int)this.GetProperty(Properties.XP) / (float)(int)this.GetProperty(Properties.LEVEL) * 10 : 0;
        }

        private float manaScore() {
            return manaWeight * (int)this.GetProperty(Properties.MANA) / (float)(int)this.GetProperty(Properties.MAXMANA);
        }

        private float hpScore() {
            return hpWeight * (int)this.GetProperty(Properties.HP) / (float)(int)this.GetProperty(Properties.MAXHP);
        }

        private float shieldScore() {
            return shieldWeight * (int)this.GetProperty(Properties.ShieldHP) / (float)(int)this.GetProperty(Properties.MaxShieldHP);
        }

        public override int GetNextPlayer()
        {
            return this.NextPlayer;
        }

        public override void CalculateNextPlayer()
        {
            Vector3 position = (Vector3)this.GetProperty(Properties.POSITION);
            bool enemyEnabled;

            //basically if the character is close enough to an enemy, the next player will be the enemy.
            foreach (var enemy in this.GameManager.enemies)
            {
                enemyEnabled = (bool) this.GetProperty(enemy.name);
                if (enemyEnabled && (enemy.transform.position - position).sqrMagnitude <= 100)
                {
                    this.NextPlayer = 1;
                    this.NextEnemyAction = new SwordAttack(this.GameManager.Character, enemy);
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
