using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] GameObject _notes;     

    void Start()
    {
        _notes.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("gameplay"); 
    }

    public void OpenInfoPanel()
    {
        _notes.SetActive(true); 
    }

    public void CloseInfoPanel()
    {
        _notes.SetActive(false);  
    }
}
