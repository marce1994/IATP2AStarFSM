using System;

namespace BehaviorTree
{
    public class Inverter : BehaviorComponent
    {
        private BehaviorComponent _Behavior;
        
        public override void AddChild(BehaviorComponent behaviorComponent)
        {
            _Behavior = behaviorComponent;
        }

        public override BehaviorReturnCode Behave()
        {
            try
            {
                switch (_Behavior.Behave())
                {
                    case BehaviorReturnCode.Failure:
                        ReturnCode = BehaviorReturnCode.Success;
                        return ReturnCode;
                    case BehaviorReturnCode.Success:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                    case BehaviorReturnCode.Running:
                        ReturnCode = BehaviorReturnCode.Running;
                        return ReturnCode;
                }
            }
            catch (Exception)
            {
                ReturnCode = BehaviorReturnCode.Success;
                return ReturnCode;
            }

            ReturnCode = BehaviorReturnCode.Success;
            return ReturnCode;
        }
    }
}

