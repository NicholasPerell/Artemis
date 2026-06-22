using System;
using UnityEngine;

namespace Perell.Artemis
{
    [Serializable]
    public struct Rule
    {
        [SerializeField]
        Criterion[] _criteria;

        public Rule(params Criterion[] criteria)
        {
            _criteria = new Criterion[criteria.Length];
            Array.Copy(criteria, _criteria, _criteria.Length);
            Array.Sort(_criteria);
        }
    }
}
