using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis
{
    //[CreateAssetMenu(fileName = "New Artemis Narrative Bundle", menuName = "Artemis/Narrative Bundle")]
    public class ArrowBundle : ScriptableObject
    {
        [SerializeField]
        private Arrow[] arrows;

        public Arrow[] GetArrows()
        {
            return arrows;
        }

        public static ArrowBundle CreateInstance(Arrow[] _arrows)
        {
            ArrowBundle created = ScriptableObject.CreateInstance<ArrowBundle>();
            created.arrows = _arrows;
            return created;
        }
    }
}