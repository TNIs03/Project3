using UnityEngine;

public class Tile : MonoBehaviour
{
    private int x;
    private int y;
    private GridManager gridManager;
    private TileType type;

    private Vector3 mouseStart;
    private bool hasSwapped = false;

    void OnMouseDown()
    {
        mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseStart.z = 0f;
        hasSwapped = false;
    }

    void OnMouseDrag()
    {
        if (hasSwapped) return;

        Vector3 currentMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMouse.z = 0f;
        Vector3 dragVector = currentMouse - mouseStart;
        
        if (Mathf.Abs(dragVector.x) > 0.5f || Mathf.Abs(dragVector.y) > 0.5f) // drag threshold
        {
            int dx = 0, dy = 0;

            if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
                dx = dragVector.x > 0 ? 1 : -1;
            else
                dy = dragVector.y > 0 ? 1 : -1;

            int targetX = x + dx;
            int targetY = y + dy;

            if (gridManager.IsInsideGrid(targetX, targetY))
            {
                gridManager.TrySwapAndAnimate(x, y, targetX, targetY);
                hasSwapped = true;
            }
        }
    }

    public int getX() { return x; }
    public int getY() { return y; }
    public void setX(int x) {  this.x = x; }
    public void setY(int y) {  this.y = y; }
    public void setXY(int x, int y) { this.x = x; this.y = y; }
    public void setType(TileType type) { this.type = type;}
    public TileType getType() { return type; }
    public bool isSpecial()
    {
        return type == TileType.Bomb || type == TileType.Dynamite;
    }
    public void setGridManager(GridManager gridManager) { this.gridManager = gridManager;}
}
