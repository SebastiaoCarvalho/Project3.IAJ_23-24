using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using System;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public abstract class WalkToTargetAndExecuteAction : Action
    {
        protected AutonomousCharacter Character { get; set; }

        public GameObject Target { get; set; }

        protected WalkToTargetAndExecuteAction(string actionName, AutonomousCharacter character, GameObject target) : base(actionName + "(" + target.name + ")")
        {
            this.Character = character;
            this.Target = target;
        }

        public override float GetDuration()
        {
            return this.GetDuration(this.Character.transform.position);
        }

        public override float GetDuration(WorldModel worldModel)
        {
            var position = (Vector3)worldModel.GetProperty(Properties.POSITION);
            return this.GetDuration(position);
        }

        private float GetDuration(Vector3 currentPosition)
        {
            //rough estimation, with no pathfinding...
            var distance = getDistance(currentPosition, Target.transform.position);
            var result = distance / this.Character.Speed;
            return result;
        }

        public override float GetGoalChange(Goal goal)
        {
            if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                return this.GetDuration();
            }
            else return 0.0f;
        }

        public override bool CanExecute()
        {
            return this.Target != null;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (this.Target == null) return false;
            var targetEnabled = (bool)worldModel.GetProperty(this.Target.name);
            return targetEnabled;
        }

        public override void Execute()
        {
            Vector3 delta = this.Target.transform.position - this.Character.transform.position;
            
            if (delta.sqrMagnitude > 5 )
               this.Character.StartPathfinding(this.Target.transform.position);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var duration = this.GetDuration(worldModel);

            var quicknessValue = worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, quicknessValue + duration);
            var time = (float)worldModel.GetProperty(Properties.TIME);
            worldModel.SetProperty(Properties.TIME, time + duration);
            worldModel.SetProperty(Properties.POSITION, Target.transform.position);
        }

        private float getDistance(Vector3 currentPosition, Vector3 targetPosition)
        {        
            var distance = (currentPosition - targetPosition).magnitude * 2;
            return distance;
        }

        public override float GetHValue(WorldModel worldModel)
        {
            /* Debug.Log("base " + worldModel.GetProperty(Properties.POSITION) + " " + Target.transform.position);
            Debug.Log("distance " + getDistance((Vector3)worldModel.GetProperty(Properties.POSITION), Target.transform.position)); */
            var duration = this.GetDuration(worldModel);
            var time = (float) worldModel.GetProperty(Properties.TIME);
            /* Debug.Log("time remaining " + (150 - time) + " duration " + duration);
            Debug.Log("value " + (float) (Math.Log(duration + 1)/Math.Log(150 - time + 1))); */
            return (float) (Math.Log(duration + 1)/Math.Log(150 - time + 1));
        }
    }
}