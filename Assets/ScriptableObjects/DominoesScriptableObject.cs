using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dominoes
{
    [CreateAssetMenu(fileName ="Dominoes",menuName ="Dominoes")]
    public class DominoesScriptableObject : ScriptableObject
    {
        public List<Sprite> imagesVertical;
        public List<Sprite> imagesHorizantal;

    }
}
