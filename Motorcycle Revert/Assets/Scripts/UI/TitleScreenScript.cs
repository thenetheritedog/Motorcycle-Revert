
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleScreenScript : MonoBehaviour
{

    [SerializeField] string startScene;
    UIDocument titleUIDocument;
    VisualElement root;
    Button start;
    Button quit;
    private void OnEnable()
    {
        titleUIDocument = GetComponent<UIDocument>();
        root = titleUIDocument.rootVisualElement;
        start = root.Q<Button>("Start");
        start.RegisterCallback<ClickEvent>(OnStartClick);
        quit = root.Q<Button>("Quit");
        quit.RegisterCallback<ClickEvent>(OnQuitClick);
    }

    void OnStartClick(ClickEvent evt)
    {
        SceneManager.LoadScene(startScene);

    }
    
    void OnQuitClick(ClickEvent evt)
    {
        Application.Quit();
    }
}
