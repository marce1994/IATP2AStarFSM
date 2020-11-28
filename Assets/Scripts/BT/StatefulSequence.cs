using System;
using System.Linq;

namespace BehaviorTree
{
    //ejecuta toda la secuencia, si 1 devuelve error, retorna error.
    public class StatefulSequence : BehaviorComponent
    {
        private BehaviorComponent[] _Behaviors;

        private int _LastBehavior = 0;

        public override void AddChild(BehaviorComponent behaviorComponent)
        {
            if (_Behaviors == null)
                _Behaviors = new BehaviorComponent[] { };
            _Behaviors = _Behaviors.Append(behaviorComponent).ToArray();
        }

        public override BehaviorReturnCode Behave()
        {
            //start from last remembered position
            for (; _LastBehavior < _Behaviors.Length; _LastBehavior++)
            {
                try
                {
                    switch (_Behaviors[_LastBehavior].Behave())
                    {
                        case BehaviorReturnCode.Failure:
                            _LastBehavior = 0;
                            ReturnCode = BehaviorReturnCode.Failure;
                            return ReturnCode;
                        case BehaviorReturnCode.Success:
                            continue;
                        case BehaviorReturnCode.Running:
                            ReturnCode = BehaviorReturnCode.Running;
                            return ReturnCode;
                        default:
                            _LastBehavior = 0;
                            ReturnCode = BehaviorReturnCode.Success;
                            return ReturnCode;
                    }
                }
                catch (Exception)
                {
                    _LastBehavior = 0;
                    ReturnCode = BehaviorReturnCode.Failure;
                    return ReturnCode;
                }
            }

            _LastBehavior = 0;
            ReturnCode = BehaviorReturnCode.Success;
            return ReturnCode;
        }
    }
}

