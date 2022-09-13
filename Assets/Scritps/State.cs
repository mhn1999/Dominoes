using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dominoes
{
    public abstract class State 
    {
        protected GameManager GameManager;
        protected List<GameObject> dominoGameObjects;


        public virtual IEnumerator Start()
        {
            yield break;
        }

        //when clicking on cards
        public virtual IEnumerator DominoMove()
        {
            yield break; 
        }
        public virtual IEnumerator GameTimer()
        {
            yield break;
        }

        // Update is called once per frame
        public virtual IEnumerator Enemy()
        {
            yield break;
        }
    }

}