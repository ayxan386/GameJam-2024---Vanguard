using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private float waitDur = 0.5f;
    [SerializeField] private GameObject loadingSpinner;

    public void Exit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        StartCoroutine(WaitThenLoad());
    }

    private IEnumerator WaitThenLoad()
    {
        loadingSpinner.SetActive(true);
        yield return new WaitForSeconds(waitDur);
        SceneManager.LoadScene("Map_01");
    }
}