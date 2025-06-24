using UnityEngine;

public class GameController : MonoBehaviour
{
    protected Game m_game => Game.instance;
    protected GameLoader m_loader => GameLoader.instance;

    public virtual void LoadScene(string sceneName) => m_loader.Load(sceneName);
}