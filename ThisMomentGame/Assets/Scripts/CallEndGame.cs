using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CallEndGame : MonoBehaviour
{
    [SerializeField] UnityEvent trigger;

    void Start()
    {
        GameManager.onGameComplete.AddListener(CallEnd);
    }

    void CallEnd()
    {
        trigger?.Invoke();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
