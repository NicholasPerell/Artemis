using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Perell.Artemis.Saving
{
    [System.Serializable]
    public struct ConstellationData : IBinaryReadWriteable
    {
        [SerializeField]
        Flag[] flags;
        
        [SerializeField]
        Archer[] archers;

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            //Flags
            BitFlagList boolFlags = new BitFlagList();
            boolFlags.ReadFromBinary(ref binaryReader);

            float[] otherFlags = new float[binaryReader.ReadInt32()];
            for (int i = 0; i < otherFlags.Length; i++)
            {
                otherFlags[i] = binaryReader.ReadSingle();
            }

            int bitIndex = 0;
            int floatIndex = 0;
            Flag currentFlag;
            for (int i = 0; i < flags.Length; i++)
            {
                currentFlag = flags[i];

                switch (currentFlag.GetValueType())
                {
                    case Flag.ValueType.BOOL:
                        currentFlag.SetValue(boolFlags.GetFlag(bitIndex));
                        bitIndex++;
                        break;
                    default:
                        currentFlag.SetValue(otherFlags[floatIndex]);
                        floatIndex++;
                        break;
                }
            }

            //Archers
            archers = archers.CopyReadFromBinary(ref binaryReader);
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            //Flags
            BitFlagList boolFlags = new BitFlagList();
            List<float> otherFlags = new List<float>();

            int bitIndex = 0;
            Flag currentFlag;
            for (int i = 0; i < flags.Length; i++)
            {
                currentFlag = flags[i];

                switch (currentFlag.GetValueType())
                {
                    case Flag.ValueType.BOOL:
                        boolFlags.SetFlag(bitIndex, currentFlag.GetValue() == 1);
                        bitIndex++;
                        break;
                    default:
                        otherFlags.Add(currentFlag.GetValue());
                        break;
                }
            }
                
            boolFlags.WriteToBinary(ref binaryWriter);

            binaryWriter.Write(otherFlags.Count);
            for (int i = 0; i < otherFlags.Count; i++)
            {
                binaryWriter.Write(otherFlags[i]);
            }

            //Archers
            archers.WriteToBinary(ref binaryWriter);
        }
    }
}