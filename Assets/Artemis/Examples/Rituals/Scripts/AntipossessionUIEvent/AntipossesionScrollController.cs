using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Artemis.Example.Rituals
{
    [RequireComponent(typeof(CircularSorterUI))]
    public class AntipossesionScrollController : MonoBehaviour
    {
        CircularSorterUI sorterUI;
        [SerializeField]
        LineDrawnBetween[] bars;
        [SerializeField]
        Color basicColor;
        [SerializeField]
        Color clickMeColor;
        [SerializeField]
        Color clickedColor;
        [SerializeField]
        float timeToWaitOnceComplete;

        int jumpBy;
        int nextToClick;
        int lastClick;
        int clicksMade;

        [HideInInspector]
        public UnityEvent scrollComplete;

        // Start is called before the first frame update
        void Start()
        {
            sorterUI = GetComponent<CircularSorterUI>();
        }

        private void OnEnable()
        {
            if(sorterUI == null)
            {
                sorterUI = GetComponent<CircularSorterUI>();
            }
            DecideDrawingPattern();
        }

        private void OnDisable()
        {
            foreach(RectTransform item in sorterUI.buttons)
            {
                item.GetComponent<Button>().onClick.RemoveListener(HandleButtonClicked);
                item.gameObject.SetActive(false);
            }
            foreach(LineDrawnBetween item in bars)
            {
                item.gameObject.SetActive(false);
            }
        }

        private void DecideDrawingPattern()
        {
            clicksMade = 0;

            //Clicking Direction
            if(Random.value > 0.5f)
            {
                jumpBy = 2;
            }
            else
            {
                jumpBy = -2;
            }

            //Orientation
            if (Random.value > 0.5f)
            {
                sorterUI.startingRadian = Mathf.PI * 0.5f;
            }
            else
            {
                sorterUI.startingRadian = Mathf.PI + Mathf.PI * 0.5f;
            }

            //Numer Of Points
            sorterUI.numberOfItemsToUse = Random.Range(0, 3) * 2 + 5;

            //Set Active the items being used
            for (int i = 0; i < sorterUI.numberOfItemsToUse; i++)
            {
                sorterUI.buttons[i].gameObject.SetActive(true);
                sorterUI.buttons[i].GetComponent<Image>().color = basicColor;
            }

            lastClick = -1;

            nextToClick = Random.Range(0,sorterUI.numberOfItemsToUse);
            sorterUI.buttons[nextToClick].GetComponent<Image>().color = clickMeColor;
            sorterUI.buttons[nextToClick].GetComponent<Button>().onClick.AddListener(HandleButtonClicked);
        }

        private void HandleButtonClicked()
        {
            clicksMade++;

            sorterUI.buttons[nextToClick].GetComponent<Image>().color = clickedColor;
            sorterUI.buttons[nextToClick].GetComponent<Button>().onClick.RemoveListener(HandleButtonClicked);

            if (lastClick > -1)
            {
                bars[clicksMade].SetDots(sorterUI.buttons[lastClick], sorterUI.buttons[nextToClick]);
                bars[clicksMade].gameObject.SetActive(true);
            }

            lastClick = nextToClick;
            nextToClick += jumpBy;
            if (nextToClick < 0)
            {
                nextToClick += sorterUI.numberOfItemsToUse;
            }
            else if (nextToClick >= sorterUI.numberOfItemsToUse)
            {
                nextToClick -= sorterUI.numberOfItemsToUse;
            }

            if (clicksMade == sorterUI.numberOfItemsToUse + 1)
            {
                StartCoroutine("HoldDisplayBeforeClosing");
            }
            else
            {
                sorterUI.buttons[nextToClick].GetComponent<Image>().color = clickMeColor;
                sorterUI.buttons[nextToClick].GetComponent<Button>().onClick.AddListener(HandleButtonClicked);
            }
        }

        IEnumerator HoldDisplayBeforeClosing()
        {
            yield return new WaitForSeconds(timeToWaitOnceComplete);
            scrollComplete.Invoke();
            gameObject.SetActive(false);
        }
    }
}