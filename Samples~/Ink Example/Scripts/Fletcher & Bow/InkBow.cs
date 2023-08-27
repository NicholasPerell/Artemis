using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using Perell.Artemis.Generated;
using System;

namespace Perell.Artemis.Example.InkIntegration
{
    public class InkBow : Bow<ArtemisInkData>
    {
        [SerializeField]
        private FlagBundle flagsMappingToInkVariables;
        [Space]
        [SerializeField]
        private TextMeshProUGUI narrativeTextBox;
        [SerializeField]
        private RectTransform buttonPanel;
        [SerializeField]
        private GameObject continueButton;
        [Space]
        [Min(float.Epsilon)]
        [SerializeField]
        private float fadeAwayTime = 1;
        [SerializeField]
        private Color defaultTextColor;


        private Image buttonPanelImage;
        private Story currentStory;
        private Flag[] currentFlags;
        private float fadeTimer;

        private void Awake()
        {
            buttonPanelImage = buttonPanel.GetComponent<Image>();
        }

        private void Update()
        {
            CheckFading();
        }

        public override void AbruptEnd()
        {
            fadeTimer = 0;
            EndStory();
        }

        public override bool IsBusy()
        {
            Debug.Log("IsBusy " +( (currentStory != null || fadeTimer > 0)? "true":"false"));

            return currentStory != null || fadeTimer > 0;
        }

        public override void Send(ArtemisInkData data)
        {
            fadeTimer = 0;
            narrativeTextBox.text = "";
            narrativeTextBox.color = defaultTextColor;
            InitializeStory(data.jsonString);
            ContinueStory();
        }

        private void InitializeStory(string jsonString)
        {
            currentStory = new Story(jsonString);
            currentFlags = flagsMappingToInkVariables.ToValueArray();

            Debug.Log("InitializeStory "+ currentStory + "\n\n" + jsonString);

            FlagID id;
            List<string> varnames = new List<string>();
            foreach (string varname in currentStory.variablesState)
            {
                varnames.Add(varname);
            }
            foreach (string varname in varnames)
            {
                if(System.Enum.TryParse(varname, true, out id))
                {
                    foreach (Flag flag in currentFlags)
                    {
                        if(flag.GetFlagID() == id)
                        {
                            switch (flag.GetValueType())
                            {
                                case Flag.ValueType.FLOAT:
                                    currentStory.variablesState[varname] = flag.GetValue();
                                    break;
                                case Flag.ValueType.BOOL:
                                    currentStory.variablesState[varname] = flag.GetValue() == 1;
                                    break;
                                case Flag.ValueType.SYMBOL:
                                    currentStory.variablesState[varname] = System.Enum.GetName(flag.GetSymbolType(), (int)flag.GetValue());
                                    break;
                            }
                            break;
                        }
                    }
                }
            }

            currentStory.variablesState.variableChangedEvent += RespondToVariableChanged;
        }

        private void EndStory()
        {
            Debug.Log("EndStory");

            currentStory.variablesState.variableChangedEvent -= RespondToVariableChanged;
            currentStory = null;
        }

        private void ContinueStory()
        {
            Debug.Log("ContinueStory");

            if (!currentStory.canContinue)
            {
                Debug.Log("(!currentStory.canContinue)");

                EndStory();
                fadeTimer = fadeAwayTime;
                return;
            }

            string additionalCopy = currentStory.Continue();

            narrativeTextBox.text += additionalCopy;

            List<Choice> currentChoices = currentStory.currentChoices;
            bool simpleContinue = currentChoices.Count == 0;
            Color baseButtonPanel = buttonPanelImage.color;
            if(simpleContinue)
            {
                continueButton.GetComponent<Button>().onClick.AddListener(RespondToContinueClick);
                baseButtonPanel.a = .5f;
            }
            else
            {
                Choice newChoice;
                GameObject newButton;
                for(int i = 0; i < currentChoices.Count; i++)
                {
                    newChoice = currentChoices[i];
                    newButton = Instantiate(continueButton, buttonPanel);
                    newButton.AddComponent<UI.ChoiceButton>().Init(newChoice, RespondToChoiceClick);
                    newButton.SetActive(true);
                }
                baseButtonPanel.a = 1f;
            }
            continueButton.SetActive(simpleContinue);
            buttonPanelImage.color = baseButtonPanel;
        }

        private void RespondToContinueClick()
        {
            Debug.Log("RespondToContinueClick");

            continueButton.GetComponent<Button>().onClick.RemoveListener(RespondToContinueClick);
            ContinueStory();
        }

        private void RespondToChoiceClick(Choice choiceMade)
        {
            Debug.Log("RespondToChoiceClick "+choiceMade.text);

            currentStory.ChooseChoiceIndex(choiceMade.index);
            for(int i = buttonPanel.childCount - 1; i >= 0; i--)
            {
                if(buttonPanel.GetChild(i).gameObject != continueButton)
                {
                    Destroy(buttonPanel.GetChild(i).gameObject);
                }
            }
            ContinueStory();
        }

        private void RespondToVariableChanged(string variableName, Ink.Runtime.Object newValue)
        {
            FlagID id;
            object symbol;

            if (System.Enum.TryParse(variableName, true, out id))
            {
                foreach (Flag flag in currentFlags)
                {
                    if (flag.GetFlagID() == id)
                    {
                        switch (flag.GetValueType())
                        {
                            case Flag.ValueType.FLOAT:
                                flag.SetValue(float.Parse(newValue.ToString()));
                                break;
                            case Flag.ValueType.SYMBOL:
                                if (System.Enum.TryParse(flag.GetSymbolType(), newValue.ToString(), true, out symbol))
                                {
                                    flag.SetValue((int)symbol);
                                }
                                else
                                {
                                    flag.SetValue(-1);
                                }
                                break;
                            case Flag.ValueType.BOOL:
                                flag.SetValue(newValue.ToString() == "true");
                                break;
                        }
                        break;
                    }
                }
            }
        }

        private void CheckFading()
        {
            if(fadeTimer > 0)
            {
                Debug.Log("Check Fading Count Down " + fadeTimer);

                fadeTimer = Mathf.Max(fadeTimer - Time.deltaTime, 0);

                Color clearTextColor = defaultTextColor;
                clearTextColor.a = 0;

                narrativeTextBox.color = Color.Lerp(defaultTextColor, clearTextColor, 1 - (fadeTimer/fadeAwayTime));
                if(fadeTimer == 0)
                {
                    Debug.Log("FadeTime went down to 0");
                    Debug.Log("Report End");

                    ReportEnd();
                }
            }
        }
    }
}