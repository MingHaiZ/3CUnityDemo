using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class sign : MonoBehaviour
{
    [Header("Sign Settings")]
    [TextArea(15, 20)]
    public string text = "Hello world";

    public float viewAngle = 90f;

    public Text uiText;

    public Canvas canvas;
    public float scaleDuration = 0.25f;

    protected Vector3 m_initialScale;
    protected bool m_showing;
    protected Collider m_collider;
    protected Camera m_camera;

    [Space(15)]
    public UnityEvent onShow;

    public UnityEvent onHide;


    protected virtual void Awake()
    {
        uiText.text = text;
        m_initialScale = canvas.transform.localScale;
        canvas.transform.localScale = Vector3.zero;
        //这里不频繁active true false可以避免触发网格重构,优化性能
        canvas.gameObject.SetActive(true);
        m_collider = GetComponent<Collider>();
        m_camera = Camera.main;
    }

    public virtual void Show()
    {
        if (!m_showing)
        {
            m_showing = true;
            onShow?.Invoke();
            StopAllCoroutines();
            StartCoroutine(Scale(Vector3.zero, m_initialScale));
        }
    }

    public virtual void Hide()
    {
        if (m_showing)
        {
            m_showing = false;
            onHide?.Invoke();
            StopAllCoroutines();
            StartCoroutine(Scale(canvas.transform.localScale, Vector3.zero));
        }
    }

    protected virtual IEnumerator Scale(Vector3 from, Vector3 to)
    {
        var elapsedTime = 0f;
        var scale = canvas.transform.localScale;

        while (elapsedTime < scaleDuration)
        {
            scale = Vector3.Lerp(from, to, (elapsedTime / scaleDuration));
            canvas.transform.localScale = scale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvas.transform.localScale = to;
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameTag.Player))
        {
            Hide();
        }
    }

    protected void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTag.Player))
        {
            var direction = (other.transform.position - transform.position).normalized;
            var angle = Vector3.Angle(transform.forward, direction);
            var allowedHeight = other.transform.position.y > m_collider.bounds.min.y;
            var inCameraSight = Vector3.Dot(m_camera.transform.forward, transform.forward) < 0;
            
            if (angle < viewAngle && allowedHeight && inCameraSight)
            {
                Show();
            } else
            {
                Hide();
            }
        }
    }
}