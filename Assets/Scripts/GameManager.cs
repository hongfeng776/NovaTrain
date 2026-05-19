using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GridManager gridManager;
    public Button refreshButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(OnRefreshButtonClicked);
        }
    }

    void OnRefreshButtonClicked()
    {
        if (gridManager != null)
        {
            gridManager.RefreshGrid();
        }
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }
    }
}
