using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Perell.Artemis.Saving
{
    public class Constellation : ScriptableObject
    {
        const string BINARY_FILE_EXTENSION = ".bytes";

        [SerializeField]
        ConstellationData dataBlock;
#if UNITY_EDITOR
        [SerializeField]
        string m_FileName;
        [SerializeField]
        TextAsset m_AssetToLoadIn;

        public void SaveAsTextAsset(string fileName)
        {
            Save(Application.dataPath, fileName);
            AssetDatabase.Refresh();
        }
#endif

        public void LoadFromTextAsset(TextAsset asset)
        {
            Debug.Log(this.name + ": Loading from Text Asset \"" + asset.name + "\"");
            MemoryStream stream = new MemoryStream(asset.bytes);

            BinaryReader binStream = new BinaryReader(stream);

            dataBlock.ReadFromBinary(ref binStream);

            binStream.Close();
            stream.Close();
        }

        public void SaveAsPeristentData(string fileName)
        {
            Save(Application.persistentDataPath, fileName);
        }

        private void Save(string saveToPath, string fileName)
        {
            if(!fileName.EndsWith(BINARY_FILE_EXTENSION))
            {
                fileName += BINARY_FILE_EXTENSION;
            }

            string filePath =  saveToPath + '/' + fileName;
            Debug.Log(this.name + ": Saving to " + filePath);
            string parentFolder = filePath.Substring(0, filePath.LastIndexOf('/') + 1);

            if(!Directory.Exists(parentFolder))
            {
                Directory.CreateDirectory(parentFolder);
            }

            FileStream stream;
            if (!File.Exists(filePath))
            {
                stream = File.Create(filePath);
            }
            else
            {
                stream = File.OpenWrite(filePath);
            }

            BinaryWriter binStream = new BinaryWriter(stream);

            dataBlock.WriteToBinary(ref binStream);

            binStream.Close();
            stream.Close();
        }

        public void LoadAsPeristentData(string fileName)
        {
            if (!fileName.EndsWith(BINARY_FILE_EXTENSION))
            {
                fileName += BINARY_FILE_EXTENSION;
            }

            string filePath = Application.persistentDataPath + '/' + fileName;
            Debug.Log(this.name + ": Loading from " + filePath);
            FileStream stream;
            stream = File.OpenRead(filePath);

            BinaryReader binStream = new BinaryReader(stream);

            dataBlock.ReadFromBinary(ref binStream);

            binStream.Close();
            stream.Close();
        }
    }
}