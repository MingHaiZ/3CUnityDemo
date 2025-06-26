using System;
using UnityEngine;
using UnityEngine.UI;

public class UILevelCard : MonoBehaviour
{
    public Button play;
    public Text title;
    public Text description;
    public Text coins;
    public Text time;
    public bool[] stars;
    public Image image;
    public Image[] starsImages;

    public string scene { get; set; }
    protected bool m_locked;

    public bool locked
    {
        get { return m_locked; }
        set
        {
            m_locked = value;
            play.interactable = !m_locked;
        }
    }

    public virtual void Play()
    {
        GameLoader.instance.Load(scene);
    }

    public virtual void Fill(GameLevel level)
    {
        if (level != null)
        {
            locked = level.locked;
            scene = level.scene;
            title.text = level.name;
            description.text = level.description;
            coins.text = level.coins.ToString();
            time.text = GameLevel.FormattedTime(level.time);
            stars = (bool[])level.stars.Clone();
            image.sprite = level.image;
        }

        for (int i = 0; i < starsImages.Length; i++)
        {
            starsImages[i].gameObject.SetActive(stars[i]);
        }
    }

    private void Start()
    {
        play.onClick.AddListener(Play);
    }
}