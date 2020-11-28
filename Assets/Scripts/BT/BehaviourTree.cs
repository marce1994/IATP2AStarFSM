namespace BehaviorTree
{
    public enum BehaviorReturnCode
    {
        Failure,
        Success,
        Running
    }
    
    //public class BehaviourTree: BehaviorComponent
    //{
    //    public BehaviorComponent Root { get; set; }

    //    public override BehaviorReturnCode Behave()
    //    {
    //        switch (Root.Behave())
    //        {
    //            case BehaviorReturnCode.Failure:
    //                ReturnCode = BehaviorReturnCode.Failure;
    //                return ReturnCode;
    //            case BehaviorReturnCode.Success:
    //                ReturnCode = BehaviorReturnCode.Success;
    //                return ReturnCode;
    //            case BehaviorReturnCode.Running:
    //                ReturnCode = BehaviorReturnCode.Running;
    //                return ReturnCode;
    //            default:
    //                ReturnCode = BehaviorReturnCode.Running;
    //                return ReturnCode;
    //        }
    //    }

    //    public override void AddChild(BehaviorComponent behaviorComponent)
    //    {
    //        Root = behaviorComponent;
    //    }
    //}
}