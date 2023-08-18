using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Perell.Artemis.Saving
{
    public static class ListArrayToBinaryUtil
    {

        public static List<T> ReadScriptableObjectList<T>(this BinaryReader binaryReader) where T : ScriptableObject, IBinaryReadWriteable
        {
            List<T> value = new List<T>();
            value.AddRange(binaryReader.ReadScriptableObjectArray<T>());
            return value;
        }

        public static T[] ReadScriptableObjectArray<T>(this BinaryReader binaryReader) where T : ScriptableObject, IBinaryReadWriteable
        {
            T[] value = new T[binaryReader.ReadInt32()];
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = ScriptableObject.CreateInstance<T>();
                value[i].ReadFromBinary(ref binaryReader);
            }
            return value;
        }

        /// <summary>
        /// Use binary reader to create a list of new binary-readable objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        public static List<T> ReadList<T>(this BinaryReader binaryReader) where T : IBinaryReadWriteable, new()
        {
            List<T> value = new List<T>();
            value.AddRange(binaryReader.ReadArray<T>());
            return value;
        }
        /// <summary>
        /// Use binary reader to create an array of new binary-readable objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        public static T[] ReadArray<T>(this BinaryReader binaryReader) where T : IBinaryReadWriteable, new()
        {
            T[] value = new T[binaryReader.ReadInt32()];
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = new T();
                value[i].ReadFromBinary(ref binaryReader);
            }

            return value;
        }

        /// <summary>
        /// Pass the binary reader into an existing list of binary-readable objects to update their data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="binaryReader"></param>
        public static void CopyReadFromBinary<T>(this List<T> value, ref BinaryReader binaryReader) where T : IBinaryReadWriteable, new()
        {
            T[] arr = value.ToArray();
            arr = arr.CopyReadFromBinary(ref binaryReader);

            value.Clear();
            value.AddRange(arr);
        }
        /// <summary>
        /// Pass the binary reader into an existing array of binary-readable objects to update their data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="binaryReader"></param>
        public static T[] CopyReadFromBinary<T>(this T[] value, ref BinaryReader binaryReader) where T : IBinaryReadWriteable, new()
        {
            T[] oldValues = new T[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                oldValues[i] = value[i];
            }

            value = new T[binaryReader.ReadInt32()];
            for (int i = 0; i < value.Length; i++)
            {
                if(i < oldValues.Length)
                {
                    value[i] = oldValues[i];
                }

                if (value[i] == null)
                {
                    value[i] = new T();
                }
                value[i].ReadFromBinary(ref binaryReader);
            }

            return value;
        }

        /// <summary>
        /// Write the data of a list of binary-writable objects to the binary writer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="binaryWriter"></param>
        public static void WriteToBinary<T>(this List<T> value, ref BinaryWriter binaryWriter) where T : IBinaryReadWriteable => value.ToArray().WriteToBinary(ref binaryWriter);
        public static void WriteToBinary<T>(this T[] value, ref BinaryWriter binaryWriter) where T : IBinaryReadWriteable
        {
            binaryWriter.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                value[i].WriteToBinary(ref binaryWriter);
            }
        }

        public static void Write<T>(this BinaryWriter binaryWriter, List<T> value) where T : IBinaryReadWriteable => binaryWriter.Write(value.ToArray());
        public static void Write<T>(this BinaryWriter binaryWriter, T[] value) where T : IBinaryReadWriteable
        {
            binaryWriter.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                value[i].WriteToBinary(ref binaryWriter);
            }
        }
    }
}