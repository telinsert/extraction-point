using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenController : MonoBehaviour
{
    private bool canContinue = false;

    void Start()
    {
        Invoke(nameof(EnableContinue), 1.5f);
    }

    void EnableContinue()
    {
        canContinue = true;
    }

    void Update()
    {
        if (canContinue && Input.anyKeyDown)
        {
            canContinue = false;

            GameManager.Instance.ResetRun();
        }
    }
}
