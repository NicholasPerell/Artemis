using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.Rituals
{
    public class AbilityWheelUI : MonoBehaviour
    {
        [SerializeField]
        Image primary, secondary;
        [SerializeField]
        Image before, after;
        [SerializeField]
        GameObject[] extraSlots;
        [SerializeField]
        RectTransform rect;
        [SerializeField]
        float widthWithExtra, widthWithoutExta;
        [SerializeField]
        PlayerController player;

        private void OnEnable()
        {
            player.OnChangedAbilityWheel += UpdateAbilityWheel;
        }

        private void OnDisable()
        {
            player.OnChangedAbilityWheel -= UpdateAbilityWheel;
        }

        private void UpdateAbilityWheel(PlayerAbilityData[] abilities, int primaryIndex)
        {
            int secondaryIndex = player.CalcNextIndexOfAbilityWheel(primaryIndex);
            primary.sprite = abilities[primaryIndex].WheelIcon;
            secondary.sprite = abilities[secondaryIndex].WheelIcon;

            bool extraSlotShowing = abilities.Length > 2;
            for (int i = 0; i < extraSlots.Length; i++)
            {
                extraSlots[i].SetActive(extraSlotShowing);
            }
            if (extraSlotShowing)
            {
                before.sprite = abilities[player.CalcPreviousIndexOfAbilityWheel(primaryIndex)].WheelIcon;
                after.sprite = abilities[player.CalcNextIndexOfAbilityWheel(secondaryIndex)].WheelIcon;
                SetNewWidth(widthWithExtra);
            }
            else
            {
                SetNewWidth(widthWithoutExta);
            }
        }

        private void SetNewWidth(float width)
        {
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        }
    }
}