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
            var uiLevelCard = Instantiate(card, container);
            print(uiLevelCard);
            m_cardList.Add(uiLevelCard);
            m_cardList[i].Fill(levels[i]);
        }

        if (focusFirstCard)
        {
            EventSystem.current.SetSelectedGameObject(m_cardList[0].play.gameObject);
        }
    }
}