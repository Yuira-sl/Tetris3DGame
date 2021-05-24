using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Octamino
{
    public class GamePauseView : MonoBehaviour
    {
        public Text Title;
        public Text Subscript;
        public RectTransform ButtonsContainer;
        public GameObject ButtonPrefab;
    
        public void SetTitle(string text) => Title.text = text;
        public void SetSubscript(string text = null)
        {
            Subscript.text = text;
            Subscript.gameObject.SetActive(!String.IsNullOrEmpty(text));
        }

        public void AddButton(Sprite sprite, UnityAction onClickAction, UnityAction pointerDownAction)
        {
            var buttonGameObject = Instantiate(ButtonPrefab);
            var rectTransformComponent = buttonGameObject.GetComponent<RectTransform>();
            var buttonComponent = buttonGameObject.GetComponent<Button>();
            var pointerHandlerComponent = buttonGameObject.GetComponent<PointerHandler>();

            pointerHandlerComponent.OnPointerDown.AddListener(pointerDownAction);
            buttonComponent.onClick.AddListener(onClickAction);
            buttonComponent.onClick.AddListener(Hide);
            buttonGameObject.GetComponent<Image>().sprite = sprite;

            rectTransformComponent.SetParent(ButtonsContainer, false);
        }
        
        public void Show() => gameObject.SetActive(true);

        private void Awake() => Hide();

        private void Hide()
        {
            for (var i = ButtonsContainer.childCount - 1; i >= 0; i--)
            {
                var go = ButtonsContainer.GetChild(i).gameObject;
                if (!go.GetComponent<Text>())
                {
                    Destroy(go);
                }
            }
            gameObject.SetActive(false);
        }
    } 
}

