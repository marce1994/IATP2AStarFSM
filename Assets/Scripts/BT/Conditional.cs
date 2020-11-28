using System;

namespace BehaviorTree
{
    public delegate bool ConditionalDelegate();

    public class Conditional : BehaviorComponent
    {
        private readonly ConditionalDelegate _Bool;

        public Conditional(ConditionalDelegate test)
        {
            _Bool = test;
        }

        public override void AddChild(BehaviorComponent behaviorComponent)
        {
            throw new NotImplementedException();
        }

        public override BehaviorReturnCode Behave()
        {
            try
            {
                switch (_Bool())
                {
                    case true:
                        ReturnCode = BehaviorReturnCode.Success;
                        return ReturnCode;
                    case false:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                    default:
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

