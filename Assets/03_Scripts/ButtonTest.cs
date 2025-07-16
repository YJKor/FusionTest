using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTest : MonoBehaviour
{
    

    public void Test()
    {
        Debug.Log("Button clicked!");
        Debug.Log("Scene loaded: " + SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Lobby");
    }
}
