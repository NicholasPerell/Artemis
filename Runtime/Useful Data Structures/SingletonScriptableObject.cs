using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Perell.Artemis
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
		private static T m_instance;
		public static T instance
		{
			get
			{
				if (!m_instance)
				{
					T[] found = Resources.FindObjectsOfTypeAll<T>();
					if (found != null && found.Length > 0)
					{
						m_instance = found[0];
					}
					else
					{
						m_instance = ScriptableObject.CreateInstance<T>();
#if UNITY_EDITOR
						string filePath = "Assets/" + m_instance.GetFilePath();
						string parentFolder = filePath.Substring(0, filePath.LastIndexOf('/') + 1);
						if(!AssetDatabase.IsValidFolder(parentFolder))
						{
							AssetDatabase.CreateFolder(parentFolder.Substring(0, parentFolder.LastIndexOf('/') + 1), parentFolder.Substring(parentFolder.LastIndexOf('/') + 1));
						}
						AssetDatabase.CreateAsset(m_instance,filePath);
						AssetDatabase.ImportAsset(filePath);
#endif
					}
				}
				return m_instance;
			}
		}

		public abstract string GetFilePath();

	}
}