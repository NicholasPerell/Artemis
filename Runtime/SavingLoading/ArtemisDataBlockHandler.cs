using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Saving
{
    public class ArtemisDataBlockHandler : MonoBehaviour
    {
        [SerializeField]
        ArtemisDataBlock dataBlock;
        [Space]
        [SerializeField]
        string fileName;

        const string BINARY_FILE_EXTENSION = ".bin";

        [ContextMenu("Save")]
        private void Save()
        {
            string filePath = Application.persistentDataPath + '/' + fileName + BINARY_FILE_EXTENSION;
            Debug.Log(filePath);
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
        }

        [ContextMenu("Load")]
        private void Load()
        {
            string filePath = Application.persistentDataPath + '/' + fileName + BINARY_FILE_EXTENSION;
            FileStream stream;
            stream = File.OpenRead(filePath);

            BinaryReader binStream = new BinaryReader(stream);

            dataBlock.ReadFromBinary(ref binStream);
        }
    }
}