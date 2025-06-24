using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UILevelList : MonoBehaviour
{
    public bool focusFirstCard = true;
    public UILevelCard card;
    public RectTransform container;
    protected List<UILevelCard> m_cardList = new List<UILevelCard>();

    private void Awake()
    {
        var levels = Game.instance.levels;
        for (int i = 0; i < levels.Count; i++)
        {
            m_cardList.Add(Instantiate(card, container));
        }

        if (focusFirstCard)
        {
            EventSystem.current.SetSelectedGameObject(m_cardList[0].play.gameObject);
        }
    }
}