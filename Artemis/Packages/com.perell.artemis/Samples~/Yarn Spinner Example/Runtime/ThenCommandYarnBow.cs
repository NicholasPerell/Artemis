using Perell.Artemis.Generated;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Perell.Artemis.Example.YarnSpinnerIntegration
{
    public class ThenCommandYarnBow : YarnBow
    {
        [Space]
        [SerializeField]
        private Archer thenArcher;
        [SerializeField]
        private FlagBundle[] thenFlagBundles;
        
        protected override void Awake()
        {
            base.Awake();

            FlagID[] partitionerIDs = thenArcher.GetPartitioningFlags();

            FlagBundle thenPartitioner = FlagBundle.CreateInstance();
            thenPartitioner.name = gameObject.name + " Then Partitioning Flags";

            Flag tempFlag;
            for (int i = 0; i < partitionerIDs.Length; i++)
            {
                tempFlag = ScriptableObject.CreateInstance<Flag>();
                tempFlag.name = partitionerIDs[i].ToString();
                tempFlag.InitFlag(partitionerIDs[i]);
                thenPartitioner.Add(tempFlag);
            }

            FlagBundle[] importedStates = new FlagBundle[thenFlagBundles.Length + 1];
            importedStates[0] = thenPartitioner;
            Array.Copy(thenFlagBundles, 0, importedStates, 1, thenFlagBundles.Length);
            thenFlagBundles = importedStates;

            switch (partitionerIDs.Length)
            {
                case 0:
                    dialogueRunner.AddCommandHandler("then", () => ThenTrigger());
                    break;
                case 1:
                    dialogueRunner.AddCommandHandler<string>("then", (a) => ThenTrigger(a));
                    break;
                case 2:
                    dialogueRunner.AddCommandHandler<string, string>("then", (a,b) => ThenTrigger(a,b));
                    break;
                case 3:
                    dialogueRunner.AddCommandHandler<string, string, string>("then", (a, b, c) => ThenTrigger(a, b, c));
                    break;
                case 4:
                    dialogueRunner.AddCommandHandler<string, string, string, string>("then", (a, b, c, d) => ThenTrigger(a, b, c, d));
                    break;
                case 5:
                    dialogueRunner.AddCommandHandler<string, string, string, string, string>("then", (a, b, c, d, e) => ThenTrigger(a, b, c, d, e));
                    break;
                case 6:
                    dialogueRunner.AddCommandHandler<string, string, string, string, string, string>("then", (a, b, c, d, e, f) => ThenTrigger(a, b, c, d, e, f));
                    break;
                case 7:
                    dialogueRunner.AddCommandHandler<string, string, string, string, string, string, string>("then", (a, b, c, d, e, f, g) => ThenTrigger(a, b, c, d, e, f, g));
                    break;
                case 8:
                    dialogueRunner.AddCommandHandler<string, string, string, string, string, string, string, string>("then", (a, b, c, d, e, f, g, h) => ThenTrigger(a, b, c, d, e, f, g, h));
                    break;
                case 9:
                    dialogueRunner.AddCommandHandler<string, string, string, string, string, string, string, string, string>("then", (a, b, c, d, e, f, g, h, i) => ThenTrigger(a, b, c, d, e, f, g, h, i));
                    break;
                case 10:
                    dialogueRunner.AddCommandHandler<string, string, string, string, string, string, string, string, string, string>("then", (a, b, c, d, e, f, g, h, i, j) => ThenTrigger(a, b, c, d, e, f, g, h, i, j));
                    break;
                default:
                    Debug.LogError(gameObject.name + ": Could not add \"then\" command handler to yarn spinner as the number of paritioning symbols for this archer exceeds 10");
                    break;
            }
        }

        public override void Send(YarnArtemisData data)
        {
            base.Send(data);
        }

        private async void ThenTrigger(params string[] keys)
        {
            Debugging.ArtemisDebug.Instance.OpenReportLine("ThenTrigger");
            Debugging.ArtemisDebug.Instance.Report("Keys: ").ReportLine(keys);
            Debugging.ArtemisDebug.Instance.CloseReport();

            FlagBundle thenPartitioner = thenFlagBundles[0];
            List<FlagID> alls = new List<FlagID>();
            SortedStrictDictionary<FlagID, Flag>.Tuple pair;
            for(int i = 0; i < keys.Length; i++)
            {
                pair = thenPartitioner.flagsUsed.GetTupleAtIndex(i);
                pair.Value.SetValue(keys[i]);
                if(keys[i] == "any")
                {
                    alls.Add(pair.Key);
                }
            }

            await Task.Delay(1);

            thenArcher.AttemptDelivery(thenFlagBundles, alls.ToArray());
        }
    }
}