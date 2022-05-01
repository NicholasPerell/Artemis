using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "New Voice Line Generator", menuName = "Narrative/Data Generators/Voice Line Generator")]
public class VoiceLineGenerator : ScriptableObject
{
    [SerializeField]
    TextAsset csvFile;

    const string backingPath = "/ScriptableObjects/Narrative/Narrative Data/Voice Lines/";
    const string subtitlePath = "/Narrative Content/Subtitling/PASSION/";

    [ContextMenu("Generate Narrative Files")]
    void Parse()
    {
        //fgCSVReader.LoadFromString(csvFile.text, AddToDatabase);
    }

    void AddToDatabase(Line currentLine)
    {
        string assetName = currentLine.cell[0].value + ".asset";
        string assetContents = "%YAML 1.1" +
            '\n' + "%TAG !u! tag:unity3d.com,2011:" +
            '\n' + "--- !u!114 &11400000" +
            '\n' + "MonoBehaviour:" +
            '\n' + "  m_ObjectHideFlags: 0" +
            '\n' + "  m_CorrespondingSourceObject: {fileID: 0}" +
            '\n' + "  m_PrefabInstance: {fileID: 0}" +
            '\n' + "  m_PrefabAsset: {fileID: 0}" +
            '\n' + "  m_GameObject: {fileID: 0}" +
            '\n' + "  m_Enabled: 1" +
            '\n' + "  m_EditorHideFlags: 0" +
            '\n' + "  m_Script: {fileID: 11500000, guid: 04ab86da48d252c46baf4c766e2090c6, type: 3}" +
            '\n' + "  m_Name: " + currentLine.cell[0].value +
            '\n' + "  m_EditorClassIdentifier:" +
            '\n' + "  id: " + currentLine.cell[0].value +
            '\n' + "  deliveryType: 1" +
            '\n' + "  priorityValue: " + currentLine.cell[3].value +
            '\n' + "  flagsToMeet:";
        if (currentLine.cell[4].value != "")
            foreach (string e in currentLine.cell[4].value.Split(','))
            {
                assetContents += '\n' + "  - " + e;
            }
        assetContents += '\n' + "  flagsToAvoid:";
        if (currentLine.cell[5] != null && currentLine.cell[5].value != "")
        {
            foreach (string e in currentLine.cell[5].value.Split(','))
            {
                assetContents += '\n' + "  - " + e;
            }
        }
        assetContents += '\n';

        string path = Application.dataPath + backingPath + assetName;

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        using (FileStream fs = File.Create(path))
        using (var writer = new StreamWriter(fs))
        {
            writer.Write(assetContents);
        }


        string subtitleAssetName = currentLine.cell[0].value + "_Subtitling.json";
        string subtitleAssetContents = "{ \"inkVersion\":20,\"root\":[[{\"#\":\"speaker Passion\"},{\"#\":\"time 20\"},\"^"+ (currentLine.cell[1].value.Contains('"')?currentLine.cell[1].value.Split('"')[0]: currentLine.cell[1].value) + "\",\"\\n\",[\"done\",{\"#f\":5,\"#n\":\"g-0\"}],null],\"done\",{\"#f\":1}],\"listDefs\":{}}";

        path = Application.dataPath + subtitlePath + subtitleAssetName;

        if (!File.Exists(path))
        {
            using (FileStream fs = File.Create(path))
            using (var writer = new StreamWriter(fs))
            {
                writer.Write(subtitleAssetContents);
            }
        }
    }

    [ContextMenu("Link Subtitles To Data Points")]
    void ParseLinker()
    {
        //fgCSVReader.LoadFromString(csvFile.text, AttemptLinkage);
    }

    void AttemptLinkage(Line currentLine)
    {
        string assetName = currentLine.cell[0].value + ".asset";
        string path = Application.dataPath + backingPath + assetName;
        string subtitleMetaName = currentLine.cell[0].value + "_Subtitling.json.meta";
        string subtitleMetaPath = Application.dataPath + subtitlePath + subtitleMetaName;

        if (File.Exists(path) && File.Exists(subtitleMetaPath))
        {
            string subtitleMetaContents = File.ReadAllText(subtitleMetaPath);
            string metaID = subtitleMetaContents.Substring(subtitleMetaContents.IndexOf("guid: ")+6);
            metaID = metaID.Substring(0,metaID.IndexOf("\n")-1);

            string oldAssetContents = File.ReadAllText(path);

            string assetContents = "";

            if(oldAssetContents.Contains("fileID: 4900000, guid: "))
            {
                assetContents = oldAssetContents.Substring(0, oldAssetContents.IndexOf("fileID: 4900000, guid: ") + 23);
            }
            else if(oldAssetContents.Contains("voiceSubtitles: {fileID: "))
            {
                assetContents = oldAssetContents.Substring(0, oldAssetContents.IndexOf("voiceSubtitles: {fileID: ") + 25)
                    + "4900000, guid: ";
            }
            else
            {
                assetContents = oldAssetContents + "  voiceSubtitles: {fileID: 4900000, guid: ";
            }

            assetContents += metaID + ", type: 3}\n";

            using (FileStream fs = File.Create(path))
            using (var writer = new StreamWriter(fs))
            {
                writer.Write(assetContents);
            }
        }
    }
}