using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ShanghaiWindy
{
    [CreateAssetMenu(fileName = "Data", menuName = "BuildMap", order = 1)]
    public class SceneData: ScriptableObject
    {
        public GameObject[] SceneObjectReferences;



    }
}
