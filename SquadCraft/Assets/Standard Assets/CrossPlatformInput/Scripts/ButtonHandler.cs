using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class ButtonHandler : MonoBehaviour
    {

        public string Name;
        public bool State;

        void OnEnable()
        {
            State = true;
        }

        void OnDisable()
        {
            State = false;
        }

        public void SetDownState()
        {
            if (State) CrossPlatformInputManager.SetButtonDown(Name);
        }


        public void SetUpState()
        {
            if (State) CrossPlatformInputManager.SetButtonUp(Name);
        }


        public void SetAxisPositiveState()
        {
            if (State) CrossPlatformInputManager.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
            if (State) CrossPlatformInputManager.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
            if (State) CrossPlatformInputManager.SetAxisNegative(Name);
        }

        public void Update()
        {

        }
    }
}
