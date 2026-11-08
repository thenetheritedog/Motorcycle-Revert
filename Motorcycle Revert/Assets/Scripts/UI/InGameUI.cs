
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InGameUI : MonoBehaviour
{


    UIDocument inGameUIDocument;
    VisualElement root;
    Label distance;

    PlayerManager playerManager;
    private void OnEnable()
    {
        inGameUIDocument = GetComponent<UIDocument>();
        root = inGameUIDocument.rootVisualElement;
        distance = root.Q<Label>("Distance");
        playerManager = FindAnyObjectByType<PlayerManager>();

    }

    private void Update()
    {
        distance.text = $"Distance:{playerManager.distance}m";
    }
}
