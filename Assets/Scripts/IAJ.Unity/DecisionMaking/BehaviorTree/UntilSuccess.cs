namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree
{
    public class UntilSuccess : Decorator {

        public UntilSuccess(Task child) : base(child) {}

        public override Result Run()
        {
            var result = this.child.Run();
            if (result == Result.Success)
            {
                return Result.Success;
            }
            else {
                return Result.Running;
            }
        }

    }
}