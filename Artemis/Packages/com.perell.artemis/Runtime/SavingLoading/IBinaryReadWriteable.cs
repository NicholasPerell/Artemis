using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Perell.Artemis.Saving
{
    public interface IBinaryReadable
    {
        void ReadFromBinary(ref BinaryReader binaryReader);
    }

    public interface IBinaryWriteable
    {
        void WriteToBinary(ref BinaryWriter binaryWriter);
    }

    public interface IBinaryReadWriteable : IBinaryReadable, IBinaryWriteable
    {
    }

    public static class BinaryReadWriteExtensions
    {
        public static T Read<T>(this BinaryReader reader) where T : IBinaryReadable, new()
        {
            var readable = new T();
            readable.ReadFromBinary(ref reader);
            return readable;
        }

        public static Guid32 ReadGuid32(this BinaryReader reader) => new(reader.ReadBytes(4));

        public static List<T> ReadList<T>(this BinaryReader reader) where T : IBinaryReadable, new()
        {
            var list = new List<T>();
            var count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.Read<T>());
            }

            return list;
        }

        public static T[] ReadArray<T>(this BinaryReader reader) where T : IBinaryReadable, new()
        {
            var array = new T[reader.ReadInt32()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.Read<T>();
            }

            return array;
        }

        public static void Write(this BinaryWriter writer, IBinaryWriteable writeable) => writeable.WriteToBinary(ref writer);

        public static void Write(this BinaryWriter writer, Guid32 guid32) => writer.Write(guid32.ToByteArray());

        public static void Write<T>(this BinaryWriter writer, List<T> list) where T : IBinaryWriteable, new()
        {
            writer.Write(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void Write<T>(this BinaryWriter writer, T[] array) where T : IBinaryWriteable, new()
        {
            writer.Write(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(array[i]);
            }
        }

    }
}