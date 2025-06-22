using System.Collections;
using UnityEngine;

public static class Animation 
{
    public static IEnumerator SwapLerp(GameObject tile, Vector2 targetPos)
    {
        Vector2 start = tile.transform.position;
        float t = 0f;
        float effectTime = GameConstants.AnimationTime.Swap;

        while (t < effectTime)
        {
            t += Time.deltaTime; // Speed of animation
            tile.transform.position = Vector2.Lerp(start, targetPos, t / effectTime);
            yield return null;
        }

        tile.transform.position = targetPos;
        yield return new WaitForSeconds(GameConstants.AnimationTime.SwapWait);
    }
    public static IEnumerator AnimateDrop(GameObject tile, Vector2 targetPos, int dropTileCnt = 1)
    {
        float t = 0;
        Vector3 start = tile.transform.position;
        float effectTime = GameConstants.AnimationTime.Drop;
        while (t < effectTime * dropTileCnt)
        {
            t += Time.deltaTime;
            tile.transform.position = Vector3.Lerp(start, targetPos, t / (effectTime * dropTileCnt));
            yield return null;
        }
        tile.transform.position = targetPos;
    }
}