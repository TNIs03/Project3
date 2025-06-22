using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject explodePrefab;
    [SerializeField] Sprite[] tileSprites;
    [SerializeField] Sprite rocketSprite;
    [SerializeField] Sprite bombSprite;

    private Queue<GameObject> objectQueue;
    private GameObject[,] tiles;
    private List<int> tileTypeCount;
    private Difficulty difficulty;
    private int specialTileCount;
    private int moveLeft;
    private int points;
    private int pointMultiplier;

    void Start()
    {
        GameManager.SetGameState(GameState.PLaying);
        tiles = new GameObject[width, height];
        objectQueue = new Queue<GameObject>();
        tileTypeCount = Enumerable.Repeat(0, tileSprites.Length).ToList();
        difficulty = GameManager.Instance.selectedDifficulty;
        specialTileCount = 0;
        moveLeft = GameConstants.MoveCount;
        points = 0;
        pointMultiplier = GameConstants.PointMultiplier;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x, y);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.transform.parent = transform;

                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                
                int type = GetStartTileAt(x, y);
                tileTypeCount[type]++;

                sr.sprite = tileSprites[type];

                Tile tileScript = tile.AddComponent<Tile>();
                tileScript.setGridManager(this);
                tileScript.setX(x);
                tileScript.setY(y);
                tileScript.setType((TileType)type);

                tiles[x, y] = tile;
            }
        }
        GameCanvasController.Instance.UpdateMove(moveLeft);
        GameCanvasController.Instance.UpdatePoint(points);
    }
    public void TrySwapAndAnimate(int x1, int y1, int x2, int y2)
    {
        if (GameManager.GetGameState() == GameState.PLaying)
            StartCoroutine(SwapTiles(x1, y1, x2, y2));
    }
    IEnumerator SwapTiles(int x1, int y1, int x2, int y2)
    {
        GameManager.SetGameState(GameState.Animating);
        var tileA = tiles[x1, y1];
        var tileB = tiles[x2, y2];

        tiles[x1, y1] = tileB;
        tiles[x2, y2] = tileA;

        tileA.GetComponent<Tile>().setXY(x2, y2);
        tileB.GetComponent<Tile>().setXY(x1, y1);

        StartCoroutine(Animation.SwapLerp(tileA, new Vector2(x2, y2)));
        yield return StartCoroutine(Animation.SwapLerp(tileB, new Vector2(x1, y1)));

        if (tileA.GetComponent<Tile>().isSpecial() || tileB.GetComponent<Tile>().isSpecial())
        {
            activeSpecialTile(x2, y2);
            activeSpecialTile(x1, y1);

            moveLeft--;
            GameCanvasController.Instance.UpdateMove(moveLeft);

            yield return new WaitForSeconds(GameConstants.AnimationTime.Explode);
            yield return StartCoroutine(DropTiles());
            GameManager.SetGameState(GameState.PLaying);
            yield break;
        }

        // Check for matches
        var matchesA = GetMatchesAt(x2, y2);
        var matchesB = GetMatchesAt(x1, y1);

        if (matchesA.Count >= 3 || matchesB.Count >= 3)
        {
            ClearMatches(matchesA, x2, y2);
            ClearMatches(matchesB, x1, y1);

            moveLeft--;
            GameCanvasController.Instance.UpdateMove(moveLeft);

            yield return new WaitForSeconds(GameConstants.AnimationTime.Explode);
            yield return StartCoroutine(DropTiles());
        }
        else
        {
            // Swap back if no match
            tiles[x1, y1] = tileA;
            tiles[x2, y2] = tileB;

            tileA.GetComponent<Tile>().setXY(x1, y1);
            tileB.GetComponent<Tile>().setXY(x2, y2);

            StartCoroutine(Animation.SwapLerp(tileA, new Vector2(x1, y1)));
            yield return StartCoroutine(Animation.SwapLerp(tileB, new Vector2(x2, y2)));
        }
        GameManager.SetGameState(GameState.PLaying);
    }

    void activeSpecialTile(int x, int y)
    {
        Tile tileScript = tiles[x, y]?.GetComponent<Tile>();
        if (tileScript == null) return;
        List<GameObject> destroyedList = new List<GameObject>();
        if (tileScript.getType() == TileType.Dynamite)
        {
            for (int i = -1;  i <= 1; i++)
            {
                if (IsInsideGrid(x + i, y) && tiles[x + i, y] != null) destroyedList.Add(tiles[x + i, y]);
                if (IsInsideGrid(x, y + i) && tiles[x, y + i] != null) destroyedList.Add(tiles[x, y + i]);
            }
        }
        else if (tileScript.getType() == TileType.Bomb)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (IsInsideGrid(x + i, y + j) && tiles[x + i, y + j] != null) destroyedList.Add(tiles[x + i, y + j]);
                }
            }
        }
        specialTileCount--;
        if (destroyedList.Count > 0)
        {
            // do this to prevent stack overflow when 2 nearby special tiles are activated
            tileScript.setType(TileType.None);
            ClearDestroyed(destroyedList);
        }
    }

    List<GameObject> GetMatchesAt(int x, int y)
    {
        List<GameObject> matchingTiles = new List<GameObject>();

        TileType centerSpriteType = tiles[x, y].GetComponent<Tile>().getType();
        // Horizontal
        List<GameObject> horizontalMatches = new List<GameObject> { tiles[x, y] };
        int i = x - 1;
        while (i >= 0 && tiles[i, y]?.GetComponent<Tile>().getType() == centerSpriteType)
        {
            horizontalMatches.Add(tiles[i, y]);
            i--;
        }
        i = x + 1;
        while (i < width && tiles[i, y]?.GetComponent<Tile>().getType() == centerSpriteType)
        {
            horizontalMatches.Add(tiles[i, y]);
            i++;
        }
        int verticalMatchAddition = 0;
        if (horizontalMatches.Count >= 3)
        {
            matchingTiles.AddRange(horizontalMatches);
            verticalMatchAddition = 1;
        }

        // Vertical
        List<GameObject> verticalMatches = new List<GameObject> {};
        if (verticalMatchAddition == 0) { verticalMatches.Add(tiles[x, y]); } 
        int j = y - 1;
        while (j >= 0 && tiles[x, j]?.GetComponent<Tile>().getType() == centerSpriteType)
        {
            verticalMatches.Add(tiles[x, j]);
            j--;
        }
        j = y + 1;
        while (j < height && tiles[x, j]?.GetComponent<Tile>().getType() == centerSpriteType)
        {
            verticalMatches.Add(tiles[x, j]);
            j++;
        }

        if (verticalMatchAddition + verticalMatches.Count >= 3)
            matchingTiles.AddRange(verticalMatches);

        return matchingTiles;

    }

    void ClearMatches(List<GameObject> matches, int matchX, int matchY)
    {
        foreach (GameObject match in matches)
        {
            Tile tileScript = match.GetComponent<Tile>();
            int x = tileScript.getX();
            int y = tileScript.getY();
            tileTypeCount[(int)tileScript.getType()]--;

            if (matches.Count > 3 && matchX == x && matchY == y)
            {
                if (matches.Count == 4)
                {
                    tileScript.setType(TileType.Dynamite);
                    match.GetComponent<SpriteRenderer>().sprite = rocketSprite;
                }
                else
                {
                    tileScript.setType(TileType.Bomb);
                    match.GetComponent<SpriteRenderer>().sprite = bombSprite;
                }
                specialTileCount++;
                points += pointMultiplier;
            } 
            else
            {
                StartCoroutine(AnimateExplode(new Vector2(x, y)));
                match.SetActive(false);
                if (tiles[x, y] != null)
                {
                    objectQueue.Enqueue(match);
                    tiles[x, y] = null;
                    points += pointMultiplier;
                }

                tileScript.setType(TileType.None);
            }
            
        }
        GameCanvasController.Instance.UpdatePoint(points);
    }

    void ClearDestroyed(List<GameObject> destroyedList)
    {
        foreach (GameObject tile in destroyedList)
        {
            Tile tileScript = tile.GetComponent<Tile>();
            int x = tileScript.getX();
            int y = tileScript.getY();

            if (tileScript.getType() == TileType.Dynamite || tileScript.getType() == TileType.Bomb) activeSpecialTile(x, y);
            else
            {
                if (tileScript.getType() != TileType.None) tileTypeCount[(int)tileScript.getType()]--;
            }
            tile.SetActive(false);
            if (tiles[x, y] != null)
            {
                objectQueue.Enqueue(tile);
                tiles[x, y] = null;
                points += pointMultiplier;
            }
            StartCoroutine(AnimateExplode(new Vector2(x, y)));
            tileScript.setType(TileType.None);
        }
        GameCanvasController.Instance.UpdatePoint(points);
    }

    IEnumerator DropTiles()
    {
        int maxEmptyTileCnt = 0;
        for (int x = 0; x < width; x++)
        {
            int emptyTileCnt = 0;
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null) emptyTileCnt++;
                else
                {
                    if (emptyTileCnt > 0)
                    {
                        int newY = y - emptyTileCnt;
                        StartCoroutine(Animation.AnimateDrop(tiles[x, y], new Vector2(x, newY), emptyTileCnt));
                        tiles[x, newY] = tiles[x, y];
                        tiles[x, y] = null;
                        tiles[x, newY].GetComponent<Tile>().setY(newY);
                    }
                }
            }
            if (emptyTileCnt > maxEmptyTileCnt) maxEmptyTileCnt = emptyTileCnt;
            for (int y = height - emptyTileCnt; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    GameObject tile = objectQueue.Dequeue();
                    tile.SetActive(true);
                    tile.transform.position = new Vector2(x, y + emptyTileCnt);

                    SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();

                    int type = GetSmartTileAt(x, y);
                    tileTypeCount[type]++;
                    sr.sprite = tileSprites[type];

                    Tile tileScript = tile.GetComponent<Tile>();
                    tileScript.setGridManager(this);
                    tileScript.setX(x);
                    tileScript.setY(y);
                    tileScript.setType((TileType)type);

                    tiles[x, y] = tile;

                    // Animate drop
                    StartCoroutine(Animation.AnimateDrop(tile, new Vector2(x, y), emptyTileCnt));
                }
            }
        }
        yield return new WaitForSeconds(GameConstants.AnimationTime.Drop * maxEmptyTileCnt + GameConstants.AnimationTime.DropWait);
        //CheckAllMatches();

        if (CheckAllMatches())
        {
            yield return new WaitForSeconds(GameConstants.AnimationTime.Explode);
            yield return StartCoroutine(DropTiles());
        }
        else
        {
            Debug.Log("Turn ended.\nSpecial count: " + specialTileCount);
            if (moveLeft == 0)
            {
                GameManager.SetGameState(GameState.Ended);
                DialogController.Instance.OnGameOver("OUT OF MOVE\n\nPOINTS: " + points);
            }
            else if (!HasPossibleMove())
            {
                GameManager.SetGameState(GameState.Ended);
                DialogController.Instance.OnGameOver("UNMOVABLE\n\nPOINTS: " + points);
            }
        }
    }

    bool CheckAllMatches()
    {
        bool haveMatches = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] != null)
                {
                    var matches = GetMatchesAt(x, y);
                    if (matches.Count > 0)
                    {
                        ClearMatches(matches, x, y);
                        matches.Clear();
                        haveMatches = true;
                    }
                }
            }
        }
        return haveMatches;
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    bool WouldCauseMatch(int x, int y, TileType type)
    {
        // Horizontal check
        int left = MatchCount(x - 1, y, -1, 0, type);
        int right = MatchCount(x + 1, y, 1, 0, type);
        if (left + right >= 2) return true;

        // Vertical check
        int down = MatchCount(x, y - 1, 0, -1, type);
        int up = MatchCount(x, y + 1, 0, 1, type);
        if (down + up >= 2) return true;

        return false;
    }

    int MatchCount(int startX, int startY, int dx, int dy, TileType type)
    {
        int count = 0;
        for (int i = 0; i < 2; i++)
        {
            int nx = startX + i * dx;
            int ny = startY + i * dy;
            if (!IsInsideGrid(nx, ny)) break;
            var neighbor = tiles[nx, ny];
            if (neighbor == null) break;

            TileType neighborType = neighbor.GetComponent<Tile>().getType();
            if (neighborType == type) count++;
            else break;
        }
        return count;
    }

    public int GetSmartTileAt(int x, int y)
    {
        int tileTypeCnt = tileSprites.Length;
        List<int> valid = new List<int>();
        List<float> cntList = new List<float>();

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        switch (difficulty)
        {
            case Difficulty.Easy:
                for (int i = 0; i < tileTypeCnt; i++)
                {
                    valid.Add(i);
                    cntList.Add(1);
                }
                break;
            case Difficulty.Medium:
                for (int i = 0; i <  tileTypeCnt; i++)
                {
                    if (!WouldCauseMatch(x, y, (TileType) i))
                    {
                        valid.Add(i);
                        cntList.Add(1);
                    }
                }
                break;
            case Difficulty.Hard:
                float downRate = 0.5f;
                for (int i = 0; i < tileTypeCnt; i++)
                {
                    if (!WouldCauseMatch(x, y, (TileType)i))
                    {
                        valid.Add(i);
                    }
                    else continue;
                    float cnt = tileTypeCount[i];
                    for (int j = 0; j < 4; j++)
                    {
                        if (WouldCauseMatch(x + dx[j], y + dy[j], (TileType)i))
                        {
                            cnt /= downRate;
                        }
                    }
                    cntList.Add(cnt);
                }
                break;
            case Difficulty.Extreme:
                for (int i = 0; i < tileTypeCnt; i++)
                {
                    if (WouldCauseMatch(x, y, (TileType)i))
                    {
                        continue;
                    }
                    bool validFlag = true;
                    for (int j = 0; j < 4; j++)
                    {
                        if (WouldCauseMatch(x + dx[j], y + dy[j], (TileType)i))
                        {
                            validFlag = false;
                            break;
                        }
                    }
                    if (!validFlag) continue;
                    valid.Add(i);
                    float cnt = tileTypeCount[i];
                    cntList.Add(cnt);
                }
                break;
            default: break;
        }

        int index = Utility.GetRandomIndex(cntList);

        return valid[index];
    }

    public int GetStartTileAt(int x, int y)
    {
        int tileTypeCnt = tileSprites.Length;
        List<int> valid = new List<int>();

        for (int i = 0; i < tileTypeCnt; i++)
        {
            if (!WouldCauseMatch(x, y, (TileType)i))
            {
                valid.Add(i);
            }
        }

        int index = Random.Range(0, valid.Count);

        return valid[index];
    }

    // Check posible move
    public bool HasPossibleMove()
    {
        if (specialTileCount > 0) return true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < width - 1)
                {
                    if (TryMove(x, y, x + 1, y)) return true;
                }

                if (y < height - 1)
                {
                    if (TryMove(x, y, x, y + 1)) return true;
                }
            }
        }
        return false; // No moves found
    }

    bool TryMove(int x1, int y1, int x2, int y2)
    {
        Tile tileA = tiles[x1, y1].GetComponent<Tile>();
        Tile tileB = tiles[x2, y2].GetComponent<Tile>();
        TileType typeA = tileA.getType();
        TileType typeB = tileB.getType();

        if (tileA == null || tileB == null) return false;

        // Swap them temporarily
        tileA.setType(typeB);
        tileB.setType(typeA);

        bool matchFound = WouldCauseMatch(x1, y1, typeB) || WouldCauseMatch(x2, y2, typeA);

        // Swap back
        tileA.setType(typeA);
        tileB.setType(typeB);

        return matchFound;
    }

    IEnumerator AnimateExplode(Vector2 targetPos)
    {
        GameObject obj = Instantiate(explodePrefab, targetPos, Quaternion.identity);
        yield return new WaitForSeconds(GameConstants.AnimationTime.Explode);
        Destroy(obj);
        yield return null;
    }
}
