namespace BehaviorTree
{
    public abstract class BehaviorComponent
    {
        protected BehaviorReturnCode ReturnCode;

        public BehaviorComponent() { }

        public abstract BehaviorReturnCode Behave();

        public abstract void AddChild(BehaviorComponent behaviorComponent);
    }
}
