namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree
{
    public class Inverter : Decorator {

        public Inverter(Task child) : base(child) {}

        public override Result Run()
        {
            var result = this.child.Run();
            if (result == Result.Running)
            {
                return Result.Running;
            }
            else if (result == Result.Success) {
                return Result.Failure;
            }
            else {
                return Result.Success;
            }
        }

    }
}