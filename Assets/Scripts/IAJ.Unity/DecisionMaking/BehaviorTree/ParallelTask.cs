using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree
{
    public class ParallelTask : CompositeTask {

        List<Task> terminated = new List<Task>();
        Result result = Result.Running;

        public override Result Run()
        {
            var child = children[currentChild];
            currentChild = (currentChild + 1) % children.Count;
            if (! terminated.Contains(child)) {
                result = child.Run();
            }     
            if (result == Result.Success) {
                Reset();
                return result;
            }
            if (result != Result.Running) terminated.Add(child);
            if (terminated.Count == children.Count) {
                Reset();
                return Result.Failure;
            }
            return Result.Running;
        }

        public override void Reset() {
            base.Reset();
            terminated.Clear();
        }
    }
}