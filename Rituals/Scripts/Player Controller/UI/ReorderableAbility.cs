using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Perell.Artemis.Example.Rituals
{
    public class ReorderableAbility : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public PlayerAbilityData data;
        [SerializeField]
        [Min(1)]
        float scaleWhenPressed = 1;
        [SerializeField] Image icon;
        public RectTransform rectTransform;

        public event UnityAction<ReorderableAbility> OnPress;
        public event UnityAction<ReorderableAbility> OnRelease;

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.localScale = Vector3.one * scaleWhenPressed;
            OnPress?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
            OnRelease?.Invoke(this);
        }

        public void Initialize(PlayerAbilityData _data)
        {
            data = _data;
            icon.sprite = data.WheelIcon;
        }
    }
}