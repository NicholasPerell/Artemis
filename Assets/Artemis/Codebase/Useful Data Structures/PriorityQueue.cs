using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    [System.Serializable]
    public class PriorityQueue<Value>
    {
        [SerializeField]
        private bool mRecencyBias;

        [System.Serializable]
        private struct PairingVP
        {

            [SerializeField]
            private Value mValue;

            [SerializeField]
            private float mPriority;

            public PairingVP(Value v, float p)

            {

                this.mValue = v;

                this.mPriority = p;

            }

            public Value GetValue()

            {

                return mValue;

            }

            public static bool operator <(PairingVP a, PairingVP b)

            {

                return a.mPriority < b.mPriority;

            }

            public static bool operator >(PairingVP a, PairingVP b)

            {

                return a.mPriority > b.mPriority;

            }

            public static bool operator ==(PairingVP a, PairingVP b)

            {

                return a.mPriority == b.mPriority;

            }

            public static bool operator !=(PairingVP a, PairingVP b)

            {

                return a.mPriority != b.mPriority;

            }

            public override int GetHashCode()
            {
                return mPriority.GetHashCode() * mValue.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
        }

        [SerializeField]
        private List<PairingVP> mList;

        public PriorityQueue()
        {
            mRecencyBias = false;
            mList = new List<PairingVP>();
        }

        public PriorityQueue(bool isThereRecencyBias)
        {
            mRecencyBias = isThereRecencyBias;
            mList = new List<PairingVP>();
        }

        public bool Enqueue(Value v, float p)
        {

            PairingVP pair = new PairingVP(v, p);

            if (mList.Exists(x => x.GetValue().Equals(v)))

            {

                PairingVP temp = mList.Find(x => x.GetValue().Equals(v));

                if (temp > pair)

                {

                    mList.Remove(temp);

                }

                else

                {

                    return false;

                }

            }

            int index = mList.Count;

            while (index > 0 && mList[index - 1] > pair)

            {

                index--;

            }
            while (mRecencyBias && index > 0 && mList[index - 1] == pair)

            {

                index--;

            }

            mList.Insert(index, pair);

            return true;

        }

        public Value Dequeue()
        {

            if (mList.Count == 0)

                return default(Value);

            Value rtn = mList[0].GetValue();

            mList.RemoveAt(0);

            return rtn;

        }

        public bool IsEmpty()

        {

            return mList.Count == 0;

        }
    }
}