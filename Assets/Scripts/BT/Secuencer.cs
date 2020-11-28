using System;
using System.Linq;

namespace BehaviorTree
{
	public class Sequence : BehaviorComponent
	{
		private BehaviorComponent[] _behaviors;

        public override void AddChild(BehaviorComponent behaviorComponent)
        {
			if (_behaviors == null)
				_behaviors = new BehaviorComponent[] { };
			_behaviors = _behaviors.Append(behaviorComponent).ToArray();
		}

        public override BehaviorReturnCode Behave()
		{
			//add watch for any running behaviors
			bool anyRunning = false;

			for(int i = 0; i < _behaviors.Length;i++)
			{
				try
				{
					switch (_behaviors[i].Behave())
					{
					case BehaviorReturnCode.Failure:
						ReturnCode = BehaviorReturnCode.Failure;
						return ReturnCode;
					case BehaviorReturnCode.Success:
						continue;
					case BehaviorReturnCode.Running:
						anyRunning = true;
						continue;
					default:
						ReturnCode = BehaviorReturnCode.Success;
						return ReturnCode;
					}
				}
				catch (Exception)
                {
					ReturnCode = BehaviorReturnCode.Failure;
					return ReturnCode;
				}
			}

			//if none running, return success, otherwise return running
			ReturnCode = !anyRunning ? BehaviorReturnCode.Success : BehaviorReturnCode.Running;
			return ReturnCode;
		}
	}
}

