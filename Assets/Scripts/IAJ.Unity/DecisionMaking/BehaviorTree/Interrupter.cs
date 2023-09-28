using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree
{
    public class Interrupter : Decorator {

        private Result _result = Result.Running;
        private bool _interrupt = false;

        public Interrupter(Task child) : base(child) {}

        public override Result Run()
        {
            if (_interrupt){
                _interrupt = false;
                return _result;
            } 
            _result = this.child.Run();
            return _result;
        }

        public void SetResult(Result result) {
            _result = result;
            _interrupt = true;
        }

    }

}