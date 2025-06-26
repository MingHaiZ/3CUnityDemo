using UnityEngine;

public class LevelController : MonoBehaviour
{
    protected LevelScore m_score => LevelScore.instance;
    protected LevelFinisher m_finisher => LevelFinisher.instance;
    public virtual void AddCoins(int amount) => m_score.coins += amount;

    public virtual void Exit() => m_finisher.Exit();

    public virtual void Finish() => m_finisher.Finish();
}