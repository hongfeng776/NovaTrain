using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public int row;
    public int col;
    public Image backgroundImage;
    public Transform iconContainer;
    public Icon currentIcon;
    
    [Header("Selection Settings")]
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color selectedColor = new Color(1f, 0.9f, 0.5f, 1f);
    public Color highlightColor = new Color(1f, 1f, 0.6f, 0.5f);
    
    private bool isSelected;
    private bool isHighlighted;

    public delegate void CellClickedHandler(Cell cell);
    public event CellClickedHandler OnCellClicked;

    void Start()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
        UpdateVisualState();
    }

    public void SetIcon(Icon icon)
    {
        if (icon == null)
        {
            Debug.LogWarning("Cell: 尝试设置空图标！");
            return;
        }

        ClearIcon();
        
        currentIcon = icon;
        
        if (iconContainer != null)
        {
            currentIcon.transform.SetParent(iconContainer);
            currentIcon.transform.localPosition = Vector3.zero;
            currentIcon.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogWarning("Cell: iconContainer为空！");
        }
        
        UpdateVisualState();
    }

    public void ClearIcon()
    {
        if (currentIcon != null)
        {
            Destroy(currentIcon.gameObject);
            currentIcon = null;
        }
        UpdateVisualState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentIcon == null)
        {
            return;
        }
        
        OnCellClicked?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }

    public void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;
        UpdateVisualState();
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public bool IsHighlighted()
    {
        return isHighlighted;
    }

    private void UpdateVisualState()
    {
        if (backgroundImage != null)
        {
            if (isSelected)
            {
                backgroundImage.color = selectedColor;
            }
            else if (isHighlighted)
            {
                backgroundImage.color = highlightColor;
            }
            else
            {
                backgroundImage.color = normalColor;
            }
        }

        if (currentIcon != null && currentIcon.transform != null)
        {
            if (isSelected)
            {
                currentIcon.transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                currentIcon.transform.localScale = Vector3.one;
            }
        }
    }
}
