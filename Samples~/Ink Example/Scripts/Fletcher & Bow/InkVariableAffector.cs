using UnityEngine;
using Ink.Runtime;

namespace Perell.Artemis.Example.InkIntegration
{
    public abstract class InkVariableKeeper : MonoBehaviour
    {
        public virtual void SetUpVariables(VariablesState state)
        {
        }

        public virtual void HandleVariableChange(string variableName, Ink.Runtime.Object newValue)
        {
        }
    }
}