using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Ink.UnityIntegration; 
#endif
using System;

namespace Perell.Artemis.Example.InkIntegration
{
    [System.Serializable]
    public class ArtemisInkData : ISerializationCallbackReceiver
    {
        public string jsonString;
#if UNITY_EDITOR
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
#endif

        public void OnAfterDeserialize()
        {
            
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            OnCompileInk(null);
#endif
        }

#if UNITY_EDITOR
        private void OnCompileInk(InkFile[] inkFiles)
        {
            jsonString = inkFile.jsonAsset.text;
        }
#endif
    }
}