using System;

namespace BehaviorTree
{
    public delegate BehaviorReturnCode BehaviourActionDelegate();

    public class BehaviorAction : BehaviorComponent
    {
        private readonly BehaviourActionDelegate _Action;

        public BehaviorAction(BehaviourActionDelegate action)
        {
            _Action = action;
        }

        public override void AddChild(BehaviorComponent behaviorComponent)
        {
            throw new NotImplementedException();
        }

        public override BehaviorReturnCode Behave()
        {
            try
            {
                switch (_Action())
                {
                    case BehaviorReturnCode.Success:
                        ReturnCode = BehaviorReturnCode.Success;
                        return ReturnCode;
                    case BehaviorReturnCode.Failure:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                    case BehaviorReturnCode.Running:
                        ReturnCode = BehaviorReturnCode.Running;
                        return ReturnCode;
                    default:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                }
            }
            catch (Exception)
            {
                ReturnCode = BehaviorReturnCode.Failure;
                return ReturnCode;
            }
        }
    }
}

