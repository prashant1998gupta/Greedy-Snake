
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceanChangeAfterSec : MonoBehaviour
{
    [SerializeField]
    private float delayBeforeLoading = 3;

    [SerializeField]
    private string sceneNameToLoad;

    private float timeElapsed;

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed > delayBeforeLoading)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
