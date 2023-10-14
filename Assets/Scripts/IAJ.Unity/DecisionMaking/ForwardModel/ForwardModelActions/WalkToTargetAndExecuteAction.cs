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

        public override float GetDuration(WorldModelImproved WorldModelImproved)
        {
            var position = (Vector3)WorldModelImproved.GetProperty(Properties.POSITION);
            return this.GetDuration(position);
        }

        public override float GetDuration(WorldModel WorldModelImproved)
        {
            var position = (Vector3)WorldModelImproved.GetProperty(Properties.POSITION);
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

        public override bool CanExecute(WorldModelImproved WorldModelImproved)
        {
            if (this.Target == null) return false;
            var targetEnabled = (bool)WorldModelImproved.GetProperty(this.Target.name);
            return targetEnabled;
        }

        public override bool CanExecute(WorldModel WorldModelImproved)
        {
            if (this.Target == null) return false;
            var targetEnabled = (bool)WorldModelImproved.GetProperty(this.Target.name);
            return targetEnabled;
        }

        public override void Execute()
        {
            Vector3 delta = this.Target.transform.position - this.Character.transform.position;
            
            if (delta.sqrMagnitude > 5 )
               this.Character.StartPathfinding(this.Target.transform.position);
        }


        public override void ApplyActionEffects(WorldModelImproved WorldModelImproved)
        {
            var duration = this.GetDuration(WorldModelImproved);

            var quicknessValue = WorldModelImproved.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, quicknessValue + duration);
            var time = (float)WorldModelImproved.GetProperty(Properties.TIME);
            WorldModelImproved.SetProperty(Properties.TIME, time + duration);
            WorldModelImproved.SetProperty(Properties.POSITION, Target.transform.position);
        }

        public override void ApplyActionEffects(WorldModel WorldModelImproved)
        {
            var duration = this.GetDuration(WorldModelImproved);

            var quicknessValue = WorldModelImproved.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL);
            WorldModelImproved.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, quicknessValue + duration);
            var time = (float)WorldModelImproved.GetProperty(Properties.TIME);
            WorldModelImproved.SetProperty(Properties.TIME, time + duration);
            WorldModelImproved.SetProperty(Properties.POSITION, Target.transform.position);
        }

        private float getDistance(Vector3 currentPosition, Vector3 targetPosition)
        {        
            var distance = this.Character.GetDistanceToTarget(currentPosition, targetPosition);
            return distance;
        }

        public override float GetHValue(WorldModelImproved WorldModelImproved)
        {
            var position = (Vector3)WorldModelImproved.GetProperty(Properties.POSITION);
            var duration = (position - this.Target.transform.position).magnitude / this.Character.Speed; // avoid navmesh bug
            var time = (float) WorldModelImproved.GetProperty(Properties.TIME);
            return (float) (Math.Log(duration + 1)/Math.Log(150 - time + 1));
        }

        public override float GetHValue(WorldModel WorldModelImproved)
        {
            var position = (Vector3)WorldModelImproved.GetProperty(Properties.POSITION);
            var duration = (position - this.Target.transform.position).magnitude / this.Character.Speed; // avoid navmesh bug
            var time = (float) WorldModelImproved.GetProperty(Properties.TIME);
            return (float) (Math.Log(duration + 1)/Math.Log(150 - time + 1));
        }
    }
}