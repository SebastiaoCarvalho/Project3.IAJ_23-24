namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree
{
    public class UntilFail : Decorator {

        public UntilFail(Task child) : base(child) {}

        public override Result Run()
        {
            var result = this.child.Run();
            if (result == Result.Failure)
            {
                return Result.Success;
            }
            else {
                return Result.Running;
            }
        }

    }
}