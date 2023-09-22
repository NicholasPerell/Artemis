using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Example.Rituals.Controls;
using UnityEngine.U2D;

namespace Perell.Artemis.Example.Rituals
{
    public class AbilityReorderer : MonoBehaviour
    {
        bool open = false;

        [SerializeField]
        GameObject reorderablePrefab;
        [SerializeField]
        float spacing;

        float rectWidth;
        float rectHeight;
        List<ReorderableAbility> elements;

        ReorderableAbility pressedAbility;
        int pressedIndex = -1;
        Vector2 pressedOffset;

        SorcererInputs inputs;
        PixelPerfectCamera pixelCamera;
        RectTransform rect;

        private void Awake()
        {
            inputs = new SorcererInputs();
            pixelCamera = Camera.main.GetComponent<PixelPerfectCamera>();
            rect = GetComponent<RectTransform>();
            rectWidth = reorderablePrefab.GetComponent<RectTransform>().sizeDelta.x;
            rectHeight = reorderablePrefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        private void OnEnable()
        {
            inputs.General.Facing.Enable();
        }

        private void OnDisable()
        {
            inputs.General.Facing.Disable();
            DeleteChildren();
        }

        private void Update()
        {
            if (elements != null)
            {
                HandlePressedIcon();
                PositionIcons();
            }
        }

        private void HandlePressedIcon()
        {
            if (pressedAbility)
            {
                Vector2 mousePos, anchorPos;

#if ENABLE_INPUT_SYSTEM
                mousePos = inputs.General.Facing.ReadValue<Vector2>();
#else
                mousePos = Input.mousePosition;
#endif

                //Calculate
                anchorPos = Camera.main.ScreenToViewportPoint(mousePos);
                anchorPos -= Vector2.one * 0.5f;
                anchorPos = new Vector2(anchorPos.x * rect.rect.width, anchorPos.y * rect.rect.height);

                //Clamp
                float fullWidth = ((elements.Count - 1) * rectWidth + (elements.Count - 1) * spacing);
                float centeringOffset = fullWidth * -0.5f;
                anchorPos.y = Mathf.Clamp(anchorPos.y, -rectHeight, rectHeight);
                anchorPos.x = Mathf.Clamp(anchorPos.x, centeringOffset, -centeringOffset);

                //Assign
                pressedAbility.rectTransform.anchoredPosition = anchorPos;

                float sliderLocation = anchorPos.x - centeringOffset;
                float diff = rectWidth + spacing;
                int indexPlaced = 0;
                for (; indexPlaced < elements.Count - 1; indexPlaced++)
                {
                    if(sliderLocation < diff * (indexPlaced + .5f))
                    {
                        break;
                    }
                }
                
                if(indexPlaced != pressedIndex)
                {
                    elements.RemoveAt(pressedIndex);
                    elements.Insert(indexPlaced, pressedAbility);

                    pressedIndex = indexPlaced;
                }
            }
        }

        private void PositionIcons()
        {
            Vector2 anchorPos = Vector2.zero;
            float centeringOffset = ((elements.Count - 1) * rectWidth + (elements.Count - 1) * spacing) * -0.5f;
            for (int i = 0; i < pressedIndex; i++)
            {
                anchorPos.x = i * (rectWidth + spacing) + centeringOffset;
                elements[i].rectTransform.anchoredPosition = anchorPos;
            }
            for (int i = pressedIndex + 1; i < elements.Count; i++)
            {
                anchorPos.x = i * (rectWidth + spacing) + centeringOffset;
                elements[i].rectTransform.anchoredPosition = anchorPos;
            }
        }

        private void CreateChildren(List<PlayerAbilityData> playerAbilities, int index)
        {
            elements = new List<ReorderableAbility>();
            for (int i = index; i < playerAbilities.Count; i++)
            {
                CreateChild(playerAbilities[i]);
            }
            for (int i = 0; i < index; i++)
            {
                CreateChild(playerAbilities[i]);
            }
        }

        private void CreateChild(PlayerAbilityData playerAbility)
        {
            ReorderableAbility reorderable = Instantiate(reorderablePrefab, transform).GetComponent<ReorderableAbility>();
            reorderable.Initialize(playerAbility);
            reorderable.OnPress += RespondToPressed;
            elements.Add(reorderable);
        }

        private void RespondToPressed(ReorderableAbility _pressedAbility)
        {
            pressedAbility = _pressedAbility;

            for(int i = 0; i < elements.Count; i++)
            {
                if(pressedAbility == elements[i])
                {
                    pressedIndex = i;
                    break;
                }
            }

            pressedOffset = Vector2.zero;

            pressedAbility.transform.SetAsLastSibling();

            pressedAbility.OnRelease += RespondToReleased;
        }

        private void RespondToReleased(ReorderableAbility arg0)
        {
            pressedAbility.OnRelease -= RespondToReleased;
            pressedAbility = null;
            pressedIndex = -1;
        }

        private void DeleteChildren()
        {
            if (elements != null)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].OnPress -= RespondToPressed;
                    elements[i].OnRelease -= RespondToReleased;
                    Destroy(elements[i].gameObject);
                }
                elements = null;
            }
        }

        public void Open(List<PlayerAbilityData> playerAbilities, int index)
        {
            open = true;
            CreateChildren(playerAbilities, index);
        }

        public List<PlayerAbilityData> Close()
        {
            List<PlayerAbilityData> order = new List<PlayerAbilityData>();
            foreach(ReorderableAbility ability in elements)
            {
                order.Add(ability.data);
            }
            DeleteChildren();
            open = false;
            return order;
        }

        public bool IsOpen()
        {
            return open;
        }
    }
}