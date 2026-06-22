using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Perell.Artemis
{
    public class SingletonScriptableObjectInitializer<S> : MonoBehaviour where S : SingletonScriptableObject<S>
    {
        [SerializeField]
        S instance;

#if UNITY_EDITOR
        private void OnValidate()
        {
			instance = SingletonScriptableObject<S>.instance;
		}
#endif
	}
}