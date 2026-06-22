using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Generated;

namespace Perell.Artemis.Example.InkIntegration
{
    public class RoomieSetToDefault : MonoBehaviour
    {
        [System.Serializable]
        public struct NameToValue
        {
            public string flagId, value;
        }

        [SerializeField]
        FlagBundle flagBundles;
        [Space]
        [SerializeField]
        NameToValue[] defaultValues;

        public void SetToDefaultValues()
        {
            Flag[] flags = flagBundles.ToValueArray();
            FlagID id;
            object symbol;
            foreach(NameToValue pair in defaultValues)
            {
                if(System.Enum.TryParse<FlagID>(pair.flagId,true,out id))
                {
                    foreach(Flag flag in flags)
                    {
                        if(flag.GetFlagID() == id)
                        {
                            switch (flag.GetValueType())
                            {
                                case Flag.ValueType.FLOAT:
                                    flag.SetValue(float.Parse(pair.value));
                                    break;
                                case Flag.ValueType.SYMBOL:
                                    if (System.Enum.TryParse(flag.GetSymbolType(), pair.value, true, out symbol))
                                    {
                                        flag.SetValue((int)symbol);
                                    }
                                    else
                                    {
                                        flag.SetValue(-1);
                                    }
                                    break;
                                case Flag.ValueType.BOOL:
                                    flag.SetValue(pair.value.ToLower() == "true");
                                    break;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}