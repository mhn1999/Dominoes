using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dominoes
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        float gameTime = 60;
        public int numberOfDominoes = 49;
        public int numberOfFirstHandDominoes = 7;
        public float moveAndRotateSpeed=0.5f;
        public int numberOfExtraDominoes = 14;
        public int maxScore = 25;
    }
}
