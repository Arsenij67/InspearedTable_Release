using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class ScenesLoader : DashboardAnimator
{
    // переменные 
    [SerializeField] private string sceneName;
    public async void LoadScene()
    {
        AsyncOperation AsyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (AsyncOperation.isDone == false)
        {
            await Task.Yield();
        }

    }

}
