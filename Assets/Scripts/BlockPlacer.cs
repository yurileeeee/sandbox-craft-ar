//# define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] GameObject block;
    [SerializeField] GameObject previewBlock;
    [SerializeField] LayerMask blockLayer;
    [SerializeField] float maxDistance; // 최대 설치 거리
    [SerializeField] Vector3 minRange;
    [SerializeField] Vector3 maxRange;
    [SerializeField] Button craftButton;

    Vector3 installPos; // 설치할 위치
    RaycastHit hit;

    private void Update()
    {
#if TEST
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, out hit, maxDistance, blockLayer)) // PC용
#else
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, blockLayer)) // AR용
#endif
        {
            Vector3 size = block.transform.lossyScale;

            // 설치할 위치를 나눠서 반올림해서 다시 곱하는 방식
            installPos = Vector3.Scale(Vector3Int.RoundToInt((hit.point + hit.normal.MultiplyVector(size * 0.5f)).DivideVector(size)), size);

            //// 클릭시 (0,0,0) ~ (15, 31, 15) 범위 안이면 블록 생성
            //if (Input.GetMouseButtonDown(0))
            //{
            //    CraftClick();
            //}

            //// 우클릭시 블록 지우기
            //if (Input.GetMouseButtonDown(1))
            //{
            //    Destroy(hit.collider.gameObject);
            //}
        }

        if (hit.transform && installPos.InRange(minRange, maxRange))
        {
            previewBlock.SetActive(true);
            previewBlock.transform.position = installPos;
            previewBlock.transform.localScale = block.transform.lossyScale;
        }
        else
        {
            previewBlock.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        if (hit.transform)
        {
            Gizmos.DrawWireCube(installPos, block.transform.lossyScale);
        }
    }

    public void CraftClick()
    {
        if (installPos.InRange(minRange, maxRange))
        {
            Instantiate(block, installPos, Quaternion.identity);
        }
    }

    public void BreakClick()
    {
        Destroy(hit.collider.gameObject);
    }
}

public static class Vector3Extensions
{
    public static Vector3 DivideVector(this Vector3 vector, Vector3 divideVector)
    {
        return new Vector3(vector.x / divideVector.x, vector.y / divideVector.y, vector.z / divideVector.z);
    }

    public static Vector3 MultiplyVector(this Vector3 vector, Vector3 multiplyVector)
    {
        return new Vector3(vector.x * multiplyVector.x, vector.y * multiplyVector.y, vector.z * multiplyVector.z);
    }

    public static bool InRange(this Vector3 vector, Vector3 minRange, Vector3 maxRange)
    {
        if (vector.x < minRange.x || vector.y < minRange.y || vector.z < minRange.z ||
            vector.x > maxRange.x || vector.y > maxRange.y || vector.z > maxRange.z)
        {
            return false;
        }

        return true;
    }
}
