using TMPro;
using UnityEngine;
using Zenject;

public class PointsUIUpdater : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    [Inject] readonly IPlayer player;

    private void Awake()
    {
        player.OnPointsUpdated += OnPointsUpdated;
    }

    void OnPointsUpdated(int points)
    {
        UpdatePointsUI(points);
    }

    void UpdatePointsUI(int points)
    {
        textMeshProUGUI.text = points.ToString();
    }
}
