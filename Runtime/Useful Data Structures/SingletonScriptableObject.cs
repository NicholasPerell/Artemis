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
					m_instance = ScriptableObject.CreateInstance<T>();
					m_instance.Load();
				}
				return m_instance;
			}
		}

		protected abstract string GetFilePath();

		protected void Save()
		{
			Save(Application.persistentDataPath, GetFilePath());
#if UNITY_EDITOR
			Save(Application.dataPath, GetFilePath());
			AssetDatabase.Refresh();
#endif
		}

		private void Save(string saveToPath, string fileName)
		{
			string filePath = saveToPath + '/' + fileName;
			string parentFolder = filePath.Substring(0, filePath.LastIndexOf('/') + 1);

			if (!Directory.Exists(parentFolder))
			{
				Directory.CreateDirectory(parentFolder);
			}

			if (!File.Exists(filePath))
			{
				FileStream stream;
				stream = File.Create(filePath);
				StreamWriter textWriter = new StreamWriter(stream);

				textWriter.Write(JsonUtility.ToJson((T)this));

				textWriter.Close();
				stream.Close();
			}
			else
			{
				File.WriteAllText(filePath, JsonUtility.ToJson((T)this));
			}
		}

		protected void Load()
		{
#if UNITY_EDITOR
			Load(Application.dataPath, GetFilePath());
#else
			Load(Application.persistentDataPath, GetFilePath());
#endif
		}

		private void Load(string loadFromPath, string fileName)
		{
			string filePath = Application.persistentDataPath + '/' + fileName;
			FileStream stream;
			stream = File.OpenRead(filePath);

			StreamReader textReader = new StreamReader(stream);

			string json = textReader.ReadToEnd();
			JsonUtility.FromJsonOverwrite(json, this);

			textReader.Close();
			stream.Close();
		}
	}
}