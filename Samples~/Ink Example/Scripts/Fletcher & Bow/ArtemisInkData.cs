using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.UnityIntegration;
using System;

namespace Perell.Artemis.Example.InkIntegration
{
    [System.Serializable]
    public class ArtemisInkData : ISerializationCallbackReceiver
    {
        public string jsonString;
        [SerializeField]
        private InkFile inkFile;

        public ArtemisInkData(InkFile _inkFile)
        {
            inkFile = _inkFile;
            jsonString = inkFile.jsonAsset.text;

            InkCompiler.OnCompileInk += OnCompileInk;
        }

        ~ArtemisInkData()
        {
            InkCompiler.OnCompileInk -= OnCompileInk;
        }

        public void OnAfterDeserialize()
        {
            
        }

        public void OnBeforeSerialize()
        {
            OnCompileInk(null);
        }

        private void OnCompileInk(InkFile[] inkFiles)
        {
            jsonString = inkFile.jsonAsset.text;
        }
    }
}