using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dominoes
{
    public abstract class StateMachine: MonoBehaviour 
    {
        protected State State;
        // Start is called before the first frame update
        public void SetState(State state)
        {
            State = state;
            StartCoroutine(State.Start());
            StartCoroutine(State.GameTimer());
        }

        // Update is called once per frame
    }
}
