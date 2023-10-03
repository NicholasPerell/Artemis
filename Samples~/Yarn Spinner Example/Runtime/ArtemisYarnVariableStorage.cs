using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Perell.Artemis.Generated;

namespace Perell.Artemis.Example.YarnSpinnerIntegration
{
    public class ArtemisYarnVariableStorage : VariableStorageBehaviour//, IEnumerable<KeyValuePair<string, object>>
    {
        public FlagBundle variableLinkedFlags;

        private Dictionary<string, object> variables = new Dictionary<string, object>();
        private Dictionary<string, System.Type> variableTypes = new Dictionary<string, System.Type>(); // needed for serialization

        public override void Clear()
        {
            variables.Clear();
            variableTypes.Clear();
        }

        public override bool Contains(string variableName)
        {
            return variables.ContainsKey(variableName);
        }

        public override (Dictionary<string, float>, Dictionary<string, string>, Dictionary<string, bool>) GetAllVariables()
        {
            Dictionary<string, float> floats = new Dictionary<string, float>();
            Dictionary<string, string> strings = new Dictionary<string, string>();
            Dictionary<string, bool> bools = new Dictionary<string, bool>();

            Flag[] flags = variableLinkedFlags.ToValueArray();
            string name;
            foreach (Flag flag in flags)
            {
                name = '$' + flag.GetFlagID().ToString().ToLower();
                switch (flag.GetValueType())
                {
                    case Flag.ValueType.FLOAT:
                        floats.Add(name, flag.GetValue());
                        break;
                    case Flag.ValueType.BOOL:
                        bools.Add(name, flag.GetValue() == 1);
                        break;
                    case Flag.ValueType.SYMBOL:
                        strings.Add(name, System.Enum.GetName(flag.GetSymbolType(),(int)flag.GetValue()));
                        Debug.Log(System.Enum.GetName(flag.GetSymbolType(), (int)flag.GetValue()));
                        break;
                }
            }

            return (floats, strings, bools);
        }

        public override void SetAllVariables(Dictionary<string, float> floats, Dictionary<string, string> strings, Dictionary<string, bool> bools, bool clear = true)
        {
            if (clear)
            {
                Clear();
            }

            foreach (var value in floats)
            {
                SetValue(value.Key, value.Value);
            }
            foreach (var value in strings)
            {
                SetValue(value.Key, value.Value);
            }
            foreach (var value in bools)
            {
                SetValue(value.Key, value.Value);
            }
        }

        public override void SetValue(string variableName, string stringValue)
        {
            bool complete = false;

            FlagID parsedId;
            if (System.Enum.TryParse<FlagID>(variableName.Substring(1), true, out parsedId))
            {
                Flag found = variableLinkedFlags.flagsUsed[parsedId];
                if (found && found.GetValueType() == Flag.ValueType.SYMBOL)
                {
                    int value = -1;
                    object symbolObject;
                    if(System.Enum.TryParse(found.GetSymbolType(), stringValue, true, out symbolObject))
                    {
                        value = (int)symbolObject;
                    }
                    found.SetValue(value);
                    complete = true;
                }
            }

            if(!complete)
            {
                variables[variableName] = stringValue;
                variableTypes[variableName] = typeof(string);
            }
        }

        public override void SetValue(string variableName, float floatValue)
        {
            Debug.Log($"SetValue {variableName}");

            bool complete = false;

            FlagID parsedId;
            if (System.Enum.TryParse<FlagID>(variableName.Substring(1), true, out parsedId))
            {
                Flag found = variableLinkedFlags.flagsUsed[parsedId];
                if (found && found.GetValueType() == Flag.ValueType.FLOAT)
                {
                    found.SetValue(floatValue);
                    complete = true;
                }
            }

            if (!complete)
            {
                variables[variableName] = floatValue;
                variableTypes[variableName] = typeof(float);
            }
        }

        public override void SetValue(string variableName, bool boolValue)
        {
            bool complete = false;

            FlagID parsedId;
            if (System.Enum.TryParse<FlagID>(variableName.Substring(1), true, out parsedId))
            {
                Flag found = variableLinkedFlags.flagsUsed[parsedId];
                if (found && found.GetValueType() == Flag.ValueType.BOOL)
                {
                    found.SetValue(boolValue);
                    complete = true;
                }
            }

            if (!complete)
            {
                variables[variableName] = boolValue;
                variableTypes[variableName] = typeof(bool);
            }
        }

        public override bool TryGetValue<T>(string variableName, out T result)
        {
            bool success = false;
            result = default(T);
            object resultObject = null;

            FlagID parsedId;
            if (System.Enum.TryParse<FlagID>(variableName.Substring(1), true, out parsedId))
            {
                Flag found = variableLinkedFlags.flagsUsed[parsedId];
                if(found)
                {
                    switch (found.GetValueType())
                    {
                        case Flag.ValueType.FLOAT:
                            resultObject = found.GetValue();
                            break;
                        case Flag.ValueType.BOOL:
                            resultObject = found.GetValue() == 1;
                            break;
                        case Flag.ValueType.SYMBOL:
                            resultObject = System.Enum.GetName(found.GetSymbolType(), (int)found.GetValue());
                            break;
                    }
                    if (resultObject != null && typeof(T).IsAssignableFrom(resultObject.GetType()))
                    {
                        result = (T)resultObject;
                        success = true;
                    }
                }
            }

            if(!success 
                && variables.TryGetValue(variableName, out resultObject) 
                && typeof(T).IsAssignableFrom(resultObject.GetType()))
            {
                result = (T)resultObject;
                success = true;
            }

            return success;
        }

    }
}