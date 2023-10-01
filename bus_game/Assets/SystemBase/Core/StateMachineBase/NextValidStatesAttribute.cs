using System;

namespace SystemBase.Core.StateMachineBase
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NextValidStatesAttribute : Attribute
    {
        public NextValidStatesAttribute(params Type[] validStateChanges)
        {
            ValidStateChanges = validStateChanges;
        }

        public Type[] ValidStateChanges { get; set; }
    }
}