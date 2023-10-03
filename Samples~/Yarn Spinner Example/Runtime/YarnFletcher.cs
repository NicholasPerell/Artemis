using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using System;
#if UNITY_EDITOR
using Yarn.Unity.Editor;
using UnityEditor;
using System.Text;
#endif

namespace Perell.Artemis.Example.YarnSpinnerIntegration
{
    [CreateAssetMenu(fileName = "New Artemis Yarn Spinner Fletcher", menuName = "Artemis Examples/Yarn Spinner Fletcher")]
    public class YarnFletcher : Fletcher<YarnArtemisData>
    {

#if UNITY_EDITOR
        [System.Serializable]
        public struct ProjectStartingNodes
        {
            [System.Serializable]
            public struct NodeString
            {
                public string str;

                public NodeString(string _str)
                {
                    str = _str;
                }

                public static implicit operator string(NodeString n) => n.str;
                public static implicit operator NodeString(string s) => new NodeString(s);
            }

            public YarnProject project;
            public NodeString[] startingNodes;
        }

        [SerializeField]
        ProjectStartingNodes[] yarnProjectsToCompile;

        public void CompileYarnProjects()
        {
            //SortedDictionary<string, object> variablesIncluded = new SortedDictionary<string, object>();
            //SortedDictionary<string, System.Type> variablesTypesIncluded = new SortedDictionary<string, System.Type>();

            StringBuilder csvGenerated = new StringBuilder("ID,Priority Value,Flags,How to handle busy,Project Asset Path,Starting Node\n");

            string projectAssetPath;
            //YarnProjectImporter importer;
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();
            string priorityValue, flags, howToHandleBusy;
            List<string> body;
            foreach(ProjectStartingNodes element in yarnProjectsToCompile)
            {
                projectAssetPath = AssetDatabase.GetAssetPath(element.project);
                //importer = (YarnProjectImporter)AssetImporter.GetAtPath(projectAssetPath);
                //foreach(YarnProjectImporter.SerializedDeclaration declaredVar in importer.ImportData.serializedDeclarations)
                //{
                //    variablesTypesIncluded.Add(declaredVar.name, declaredVar.GetType());
                //    if(declaredVar.GetType() == typeof(bool))
                //    {
                //        variablesIncluded.Add(declaredVar.name, declaredVar.defaultValueBool);
                //    }
                //    else if(declaredVar.GetType() == typeof(float))
                //    {
                //        variablesIncluded.Add(declaredVar.name, declaredVar.defaultValueNumber);
                //    }
                //    else
                //    {
                //        variablesIncluded.Add(declaredVar.name, declaredVar.defaultValueString);
                //    }
                //}

                foreach (string startingNode in element.startingNodes)
                {
                    headers = element.project.GetHeaders(startingNode);
                    priorityValue = "0";
                    flags = "";
                    howToHandleBusy = "";

                    if(headers.TryGetValue("priority_value", out body) && body.Count > 0)
                    {
                        priorityValue = body[0];
                    }

                    if (headers.TryGetValue("flags", out body))
                    {
                        for(int i = 0; i < body.Count; i++)
                        {
                            flags += body[i];
                            if(i < body.Count - 1)
                            {
                                flags += ", ";
                            }
                        }
                    }

                    if (headers.TryGetValue("how_to_handle_busy", out body) && body.Count > 0)
                    {
                        howToHandleBusy = body[0];
                    }

                    csvGenerated.Append('"').Append((element.project.name + '_' + startingNode).Replace("\"", "\"\"")).Append('"').Append(','); //ID
                    csvGenerated.Append('"').Append(priorityValue.Replace("\"", "\"\"")).Append('"').Append(','); //Priority Value
                    csvGenerated.Append('"').Append(flags.Replace("\"", "\"\"")).Append('"').Append(','); //Flags
                    csvGenerated.Append('"').Append(howToHandleBusy.Replace("\"", "\"\"")).Append('"').Append(','); //How to handle busy
                    csvGenerated.Append('"').Append(projectAssetPath.Replace("\"", "\"\"")).Append('"').Append(','); //Project Asset Path
                    csvGenerated.Append('"').Append(startingNode.Replace("\"", "\"\"")).Append('"').Append('\n'); //Start Node
                }
            }
            csvGenerated.Append("END");

            Debug.Log(csvGenerated.ToString());
            csvFile = new TextAsset(csvGenerated.ToString());
            columnsToReadFrom = 2;

            GeneratorArrowDatabase();
        }
#endif

        protected override bool SetUpDataFromCells(string[] dataToInterpret, out YarnArtemisData valueDetermined)
        {
            valueDetermined = default;
#if UNITY_EDITOR
            valueDetermined.project = (YarnProject)AssetDatabase.LoadMainAssetAtPath(dataToInterpret[0]);
            valueDetermined.node = dataToInterpret[1];
            return valueDetermined.project && Array.IndexOf(valueDetermined.project.NodeNames,valueDetermined.node) > -1;
#else
            return false;
#endif
        }
    }
}