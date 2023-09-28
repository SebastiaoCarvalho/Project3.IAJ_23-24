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
        int index = 0;
        Result result = Result.Running;

        public override Result Run()
        {
            var child = children[index];
            index = (index + 1) % children.Count;
            if (! terminated.Contains(child)){
                result = child.Run();
            }     
            if (result == Result.Success) {
                terminated.Clear();
                index = 0;
                return result;
            }
            if (result != Result.Running) terminated.Add(child);
            if (terminated.Count == children.Count) {
                terminated.Clear();
                return Result.Failure;
            }
            return Result.Running;
        }
    }
}