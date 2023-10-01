namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree
{
    public abstract class Decorator : Task {
        
        public Task child;

        public Decorator(Task child) {
            this.child = child;
        }

        public override void Reset() {
            child.Reset();
        }
        
    }
}