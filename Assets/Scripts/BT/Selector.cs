using System;
using System.Linq;

namespace BehaviorTree
{
    public class Selector : BehaviorComponent
    {
        protected BehaviorComponent[] _Behaviors;

        public Selector(params BehaviorComponent[] behaviors)
        {
            _Behaviors = behaviors;
        }

        public void AddBehaviour(BehaviorComponent behaviorComponent)
        {
            _Behaviors = _Behaviors.Append(behaviorComponent).ToArray();
        }

        public override void AddChild(BehaviorComponent behaviorComponent)
        {
            if (_Behaviors == null)
                _Behaviors = new BehaviorComponent[] { };
            _Behaviors = _Behaviors.Append(behaviorComponent).ToArray();
        }

        public override BehaviorReturnCode Behave()
        {
            for (int i = 0; i < _Behaviors.Length; i++)
            {
                try
                {
                    switch (_Behaviors[i].Behave())
                    {
                        case BehaviorReturnCode.Failure:
                            continue;
                        case BehaviorReturnCode.Success:
                            ReturnCode = BehaviorReturnCode.Success;
                            return ReturnCode;
                        case BehaviorReturnCode.Running:
                            ReturnCode = BehaviorReturnCode.Running;
                            return ReturnCode;
                        default:
                            continue;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            ReturnCode = BehaviorReturnCode.Failure;
            return ReturnCode;
        }
    }
}

