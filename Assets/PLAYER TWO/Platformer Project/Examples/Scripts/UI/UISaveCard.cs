using System;
using UnityEngine;
using UnityEngine.UI;

public class UISaveCard : MonoBehaviour
{
    public string nextScene;

    [Header("Text Formatting")]
    public string retriesFormat = "00";

    public string starsFormat = "00";
    public string coinsFormat = "000";
    public string dateFormat = "MM/dd/y hh:mm";

    [Header("Containers")]
    public GameObject dataContainer;

    public GameObject emptyContainer;

    [Header("UI Elements")]
    public Text retries;

    public Text stars;
    public Text coins;
    public Text createdAt;
    public Text updateAt;
    public Button loadButton;
    public Button deleteButton;
    public Button newGameButton;

    protected int m_index;
    protected GameData m_data;

    public bool isFilled { get; protected set; }

    protected virtual void Load()
    {
    }

    protected virtual void Delete()
    {
    }

    protected virtual void Create()
    {
    }

    public virtual void Fill(int index, GameData data)
    {
    }

    protected virtual void Start()
    {
        loadButton.onClick.AddListener(Load);
        deleteButton.onClick.AddListener(Delete);
        newGameButton.onClick.AddListener(Create);
    }
}