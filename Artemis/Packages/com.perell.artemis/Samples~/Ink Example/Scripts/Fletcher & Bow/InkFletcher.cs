using Ink.Runtime;
#if UNITY_EDITOR
using Ink.UnityIntegration;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Perell.Artemis.Example.InkIntegration
{
    [CreateAssetMenu(fileName = "New Artemis Ink Fletcher", menuName = "Artemis Examples/Ink Fletcher")]
    public class InkFletcher : Fletcher<ArtemisInkData>
    {

#if UNITY_EDITOR
        [SerializeField]
        private List<DefaultAsset> inksToCompile;

        private void OnValidate()
        {
            string assetPath;
            for (int i = inksToCompile.Count - 1; i >= 0; i--)
            {
                if (inksToCompile[i] != null)
                {
                    assetPath = AssetDatabase.GetAssetPath(inksToCompile[i]);
                    if (InkLibrary.GetInkFileWithPath(assetPath) == null)
                    {
                        inksToCompile.RemoveAt(i);
                    }
                }
            }
        }

        public void CompileInks()
        {
            string assetPath;
            List<string> assetPaths = new List<string>();
            InkFile inkFile;
            List<InkFile> inkFiles = new List<InkFile>();

            for (int i = 0; i < inksToCompile.Count; i++)
            {
                if (inksToCompile[i] != null)
                {
                    assetPath = AssetDatabase.GetAssetPath(inksToCompile[i]);
                    inkFile = InkLibrary.GetInkFileWithPath(assetPath);
                    if (inkFile != null)
                    {
                        assetPaths.Add(assetPath);
                        inkFiles.Add(inkFile);
                    }
                }
            }

            StringBuilder csvGenerated = new StringBuilder("ID,Priority Value,Flags,How to handle busy,Asset Path\n");

            Story story;
            string priorityValue, flags, howToHandleBusy;
            string header, body;
            int indexOfColon;
            for(int i = 0; i < assetPaths.Count; i++)
            {
                assetPath = assetPaths[i];
                inkFile = inkFiles[i];

                priorityValue = "0";
                flags = "";
                howToHandleBusy = "";

                story = new Story(inkFile.jsonAsset.text);
                story.Continue();
                foreach (string e in story.currentTags)
                {
                    indexOfColon = e.IndexOf(":");
                    if (indexOfColon > -1 && indexOfColon < e.Length - 1)
                    {
                        header = e.Substring(0,indexOfColon).Trim().ToLower();
                        body = e.Substring(indexOfColon + 1);

                        switch (header)
                        {
                            case "priority value":
                                priorityValue = body;
                                break;
                            case "flags":
                                flags = body;
                                break;
                            case "how to handle busy":
                                howToHandleBusy = body;
                                break;
                        }
                    }
                }

                csvGenerated.Append('"').Append(inkFile.inkAsset.name.Replace("\"","\"\"")).Append('"').Append(','); //ID
                csvGenerated.Append('"').Append(priorityValue.Replace("\"","\"\"")).Append('"').Append(','); //Priority Value
                csvGenerated.Append('"').Append(flags.Replace("\"","\"\"")).Append('"').Append(','); //Flags
                csvGenerated.Append('"').Append(howToHandleBusy.Replace("\"","\"\"")).Append('"').Append(','); //How to handle busy
                csvGenerated.Append('"').Append(assetPath.Replace("\"","\"\"")).Append('"').Append('\n'); //Asset Path
            }
            csvGenerated.Append("END");

            csvFile = new TextAsset(csvGenerated.ToString());
            columnsToReadFrom = 1;

            GeneratorArrowDatabase();
        }
#endif

        protected override bool SetUpDataFromCells(string[] dataToInterpret, out ArtemisInkData valueDetermined)
        {
            valueDetermined = null;
#if UNITY_EDITOR
            InkFile inkFile = InkLibrary.GetInkFileWithPath(dataToInterpret[0]);
            bool success = inkFile != null;
            if (success)
            {
                valueDetermined = new ArtemisInkData(inkFile);
            }
            return success;
#else
            return true;
#endif
        }
    }
}