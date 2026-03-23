using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreInteractables
{
    [CreateAssetMenu(fileName = "001 - NewCoreInteractable", menuName = "Spaceship/CoreInteractable Data")]
    public class CoreInteractableSO : ScriptableObject 
    { 
        public InteractableCoreType InteractableCoreType;
        public string InteractableName;
        public float OutlineHover;
        public float OutlineBase;
    }
}