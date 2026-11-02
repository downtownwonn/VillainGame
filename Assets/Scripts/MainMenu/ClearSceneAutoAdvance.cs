using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Advances a clear screen after its brief display period.</summary>
[DisallowMultipleComponent]
public sealed class ClearSceneAutoAdvance : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField, Min(0f)] private float displayDuration = 2f;

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(displayDuration);

        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError($"Clear scene cannot advance because '{nextSceneName}' is missing from Build Profiles.", this);
        }
    }
}
