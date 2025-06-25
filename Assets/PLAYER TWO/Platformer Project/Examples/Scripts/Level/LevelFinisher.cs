using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelFinisher : Singleton<LevelFinisher>
{
    public float loadingDelay = 1f;
    protected LevelPauser m_pauser => LevelPauser.instance;
    protected Level m_level => Level.instance;
    protected GameLoader m_loader => GameLoader.instance;
    public string exitScene;

    public UnityEvent OnExit;

    protected virtual IEnumerator ExitRoutine()
    {
        m_pauser.Pause(false);
        m_pauser.canPause = false;
        m_level.player.inputs.enabled = false;

        yield return new WaitForSeconds(loadingDelay);

        Game.LockCursor(false);
        m_loader.Load(exitScene);
        OnExit?.Invoke();
    }

    public virtual void Exit()
    {
        StopAllCoroutines();
        StartCoroutine(ExitRoutine());
    }
}