using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ChangeTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] TMP_Text text = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = Color.grey;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = Color.white;
    }
}