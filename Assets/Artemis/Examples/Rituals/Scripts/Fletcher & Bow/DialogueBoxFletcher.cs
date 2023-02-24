using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis.Example.Rituals
{
    [System.Serializable]
    public struct DialogueData
    {
        public enum Speaker
        {
            SORCERER,
            MAGE,
            DEMON_SPIRIT,
            DEMON_BOSS,
            SMITH,
            SERVANT,
            NARRATION
        }

        [System.Serializable]
        public struct LineData
        {
            public Speaker speaker;
            public string text;
        }

        public LineData[] lines;
    }


    [CreateAssetMenu(fileName = "New Artemis Dialogue Delivery System", menuName = "Artemis Examples/Dialogue Delivery System")]
    public class DialogueBoxFletcher : Fletcher<DialogueData>
    {
        protected override bool SetUpDataFromCells(string[] dataToInterpret, out DialogueData valueDetermined)
        {
            bool success = true;

            string[] stringLines = dataToInterpret[0].Split('\n',System.StringSplitOptions.RemoveEmptyEntries);

            valueDetermined = new DialogueData();
            valueDetermined.lines = new DialogueData.LineData[stringLines.Length];

            if (stringLines.Length == 0)
            {
                Debug.LogError("Not Enough Lines");
                success = false;
            }
            else
            {
                DialogueData.Speaker speakerEnum;
                string speakerString;
                int indexOfDivider;
                for (int i = 0; i < stringLines.Length; i++)
                {
                    indexOfDivider = stringLines[i].IndexOf(':');
                    if(indexOfDivider < 0)
                    {
                Debug.LogError("No : found");
                        success = false;
                        break;
                    }

                    speakerString = stringLines[i].Substring(0, indexOfDivider);
                    if(System.Enum.TryParse<DialogueData.Speaker>(speakerString, true, out speakerEnum))
                    {
                        valueDetermined.lines[i].speaker = speakerEnum;
                        valueDetermined.lines[i].text = stringLines[i].Substring(indexOfDivider + 1);
                    }
                    else
                    {
                        Debug.LogError("No enum found for "+ speakerString);
                        success = false;
                        break;
                    }
                }
            }

            return success;
        }
    }
}
