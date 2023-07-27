using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals.Controls
{
    public delegate bool ReturnBool();

    public struct InputCheckDownUp
    {
        ReturnBool isDown;
        ReturnBool isUp;

        public InputCheckDownUp(ReturnBool _isDown, ReturnBool _isUp)
        {
            isDown = _isDown;
            isUp = _isUp;
        }

        public InputCheckDownUp(KeyCode keyCode)
        {
            isDown = () => Input.GetKeyDown(keyCode);
            isUp = () => Input.GetKeyUp(keyCode);
        }

        public InputCheckDownUp(int mouseButton)
        {
            isDown = () => Input.GetMouseButtonDown(mouseButton);
            isUp = () => Input.GetMouseButtonUp(mouseButton);
        }

        public bool InputDown => isDown();
        public bool InputUp => isUp();
    }

    public struct InputsCheckDownUp
    {
        struct InputCheckStatus
        {
            private InputCheckDownUp input;
            public InputCheckDownUp Check { get => input; }
            private bool held;
            public bool Held { get { Update(); return held; } }
            private bool released;
            public bool Released { get { Update(); return released; } }

            public InputCheckStatus(InputCheckDownUp _input)
            {
                input = _input;
                held = false;
                released = true;
            }

            public InputCheckStatus(ReturnBool _isDown, ReturnBool _isUp)
            {
                input = new InputCheckDownUp(_isDown, _isUp);
                held = false;
                released = true;
            }

            public InputCheckStatus(KeyCode keyCode)
            {
                input = new InputCheckDownUp(keyCode);
                held = false;
                released = true;
            }

            public InputCheckStatus(int mouseButton)
            {
                input = new InputCheckDownUp(mouseButton);
                held = false;
                released = true;
            }

            private void Update()
            {
                released = false;

                if (input.InputUp)
                {
                    held = false;
                    released = true;
                }

                if (input.InputDown)
                {
                    held = true;
                }

                released = released || !held;
            }
        }

        InputCheckStatus[] checkStatuses;

        public InputsCheckDownUp(ReturnBool _isDown, ReturnBool _isUp, params ReturnBool[] _isDownIsUpAlternating)
        {
            Debug.Assert(_isDownIsUpAlternating.Length % 2 == 0, "InputsCheckDownUp requires the number of ReturnsBool parameters MUST be even.");
            checkStatuses = new InputCheckStatus[2 + _isDownIsUpAlternating.Length / 2];

            checkStatuses[0] = new InputCheckStatus(_isDown,_isUp);
            for (int i = 1; i < checkStatuses.Length; i++)
            {
                checkStatuses[i] = new InputCheckStatus(_isDownIsUpAlternating[(i - 1)* 2], _isDownIsUpAlternating[((i - 1) * 2) + 1]);
            }
        }

        public InputsCheckDownUp(InputCheckDownUp input, params InputCheckDownUp[] additionalInputs)
        {
            checkStatuses = new InputCheckStatus[1 + additionalInputs.Length];
            checkStatuses[0] = new InputCheckStatus(input);
            for (int i = 1; i < checkStatuses.Length; i++)
            {
                checkStatuses[i] = new InputCheckStatus(additionalInputs[i - 1]);
            }
        }

        public bool InputDown
        {
            get
            {
                bool someDown = false;
                bool someHeld = false;
                for(int i = 0; i < checkStatuses.Length; i++)
                {
                    if (checkStatuses[i].Check.InputDown)
                    {
                        someDown = true;
                    }
                    else if (checkStatuses[i].Held)
                    {
                        someHeld = true;
                    }
                }
                return someDown && !someHeld;
            }
        }

        public bool InputUp
        {
            get
            {
                bool someUp = false;
                bool allReleased = true;
                for(int i = 0; i < checkStatuses.Length; i++)
                {
                    if (checkStatuses[i].Check.InputUp)
                    {
                        someUp = true;
                    }
                    else if (!checkStatuses[i].Released)
                    {
                        allReleased = false;
                    }
                }
                return someUp && allReleased;
            }
        }
    }
}