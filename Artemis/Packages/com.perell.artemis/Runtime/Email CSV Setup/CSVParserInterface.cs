using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// massive credit to https://github.com/frozax/fgCSVReader
namespace Perell.Artemis
{
    public class CSVParserInterface : MonoBehaviour
    {
        [ContextMenu("Parse EmailDataSet")]
        void Parse()
        {
            string path = "Assets/Narrative Content/Email Sheet/Project Nautilus Written Content - EmailDataSet.csv";

            if (!System.IO.File.Exists(path))
                Debug.LogError("CSV FILE FOR TESTDIALOGUE DOESN'T EXIST");

            //fgCSVReader.LoadFromFile(path, DebugTheLines);   
        }

        void DebugTheLines(Line currentLine)
        {
            if (currentLine.cell[0] != null)
                Debug.Log("id: " + currentLine.cell[0].value + ", ");
            if (currentLine.cell[1] != null)
                Debug.Log("sender: " + currentLine.cell[1].value + ", ");
            if (currentLine.cell[2] != null)
                Debug.Log("subject: " + currentLine.cell[2].value + ", ");
            if (currentLine.cell[3] != null)
                Debug.Log("message: " + currentLine.cell[3].value + ", ");
        }
    }
}