namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree
{
    public class SuccessTask : Decorator {

        public SuccessTask(Task child) : base(child) {}

        public override Result Run()
        {
            var result = this.child.Run();
            if (result != Result.Running)
            {
                return Result.Success;
            }
            else {
                return Result.Running;
            }
        }

    }
}