using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Perell.Artemis.Debugging
{
    public class ArtemisDebug
    {
        public struct Tabs
        {
            uint indents;
            
            public Tabs(uint _indents)
            {
                indents = _indents;
            }

            public static implicit operator uint(Tabs t) => t.indents;

            public static Tabs operator ++(Tabs tabber)
            {
                tabber.indents++;
                return tabber;
            }

            public static Tabs operator --(Tabs tabber)
            {
                if (tabber.indents > 0)
                {
                    tabber.indents--;
                }
                else
                {
                    Debug.LogError("ArtemisDebug: Tabs can not go to less than 0!");
                }

                return tabber;
            }
        }

        public Tabs Indents;
        
        private StringBuilder builder;

        private static StringBuilder Builder => Instance.builder;

        private static ArtemisDebug instance = null;
        public static ArtemisDebug Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ArtemisDebug();
                }
                return instance;
            }
        }

        private ArtemisDebug()
        {
            Indents = new Tabs();
            builder = new StringBuilder();
        }

        public ArtemisDebug OpenReportLine(object headerMessage)
        {
            if (Builder.Length > 0)
            {
                Indents++;
                if (Builder.Length > 0 && Builder[Builder.Length - 1] != '\n')
                {
                    Builder.AppendLine();
                }
            }
            ReportLine(headerMessage);
            return Instance;
        }

        public void CloseReport()
        {
            if(Indents > 0)
            {
                Indents--;
            }
            else
            {
                SubmitLog();
            }
        }

        public ArtemisDebug Report<T>(T[] message)
        {
            if (message == null)
            {
                Report("null");
            }
            else
            {
                Report("{ ");
                int count = 0;
                foreach (T element in message)
                {
                    if (count > 0)
                    {
                        Report(", ");
                    }
                    Report(element);
                    count++;
                }
                Report(" }");
            }

            return Instance;
        }

        public ArtemisDebug Report<T>(List<T> message)
        {
            if (message == null)
            {
                Report("null");
            }
            else
            {
                Report("{ ");
                int count = 0;
                foreach(T element in message)
                {
                    if (count > 0)
                    {
                        Report(", ");
                    }
                    Report(element);
                    count++;
                }
                Report(" }");
            }

            return Instance;
        }

        public ArtemisDebug Report(object message)
        {
            string msg = message.ToString();

            string tabs = "";
            for (int i = 0; i < Indents; i++)
            {
                tabs += "\t";
            }

            msg = msg.Replace("\n", "\n" + tabs);

            if(Builder.Length > 0 && Builder[Builder.Length-1] == '\n')
            {
                Builder.Append(tabs);
            }

            Builder.Append(msg);
            return Instance;
        }

        public ArtemisDebug ReportLine() => ReportLine("");
        public ArtemisDebug ReportLine<T>(T[] message)
        {
            Report(message);
            Builder.AppendLine();
            return Instance;
        }
        public ArtemisDebug ReportLine<T>(List<T> message)
        {
            Report(message);
            Builder.AppendLine();
            return Instance;
        }
        public ArtemisDebug ReportLine(object message)
        {
            Report(message);
            Builder.AppendLine();
            return Instance;
        }

        private static void SubmitLog()
        {
            Debug.Log(Builder.ToString());
            Builder.Clear();
        }
    }
}