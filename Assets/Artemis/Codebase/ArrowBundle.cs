using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    //[CreateAssetMenu(fileName = "New Artemis Narrative Bundle", menuName = "Artemis/Narrative Bundle")]
    public class ArrowBundle : ScriptableObject
    {
        [SerializeField]
        public Arrow[] arrows { get;  private set; }
    }
}