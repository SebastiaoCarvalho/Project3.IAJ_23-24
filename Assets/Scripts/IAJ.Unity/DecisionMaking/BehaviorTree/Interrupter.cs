using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree
{
    public class Interrupter : Decorator {

        private Result _result = Result.Running;
        private bool _interrupt = false;

        public bool _running = false;

        public Interrupter(Task child) : base(child) {}

        public override Result Run()
        {
            _running = true;

            if (_interrupt) {
                Reset();
                return _result;
            } 
            _result = this.child.Run();
            return _result;
        }

        public void Interrupt(Result result) {
            if (_running) {
                _result = result;
                _interrupt = true;
            }
        }

        public override void Reset() {
            base.Reset();
            _interrupt = false;
            _running = false;
        }

    }

}