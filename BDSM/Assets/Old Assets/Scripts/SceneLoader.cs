using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }

    public void NextScene()
    {
//        StartCoroutine(NextSceneSequence());
//        Debug.Log("trying to start new scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1;
    }
/*    IEnumerator NextSceneSequence()
    {
        yield return StartCoroutine(GameObject.Find("Fader").GetComponent<Fade>().FadeInRoutine());
        Debug.Log("trying to wait");
        yield return new WaitForSeconds(1.0f);
        Debug.Log("done waiting");
    }
 */
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void FadeToBlack()
    {
        GameObject fader = GameObject.Find("Fader");
        fader.GetComponent<Fade>().FadeIn();
    }
    public void FadeFromBlack()
    {
        GameObject fader = GameObject.Find("Fader");
        fader.GetComponent<Fade>().FadeOut();
    }
}
