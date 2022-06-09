//# define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] Transform instantiateBlock;
    [SerializeField] GameObject block;
    [SerializeField] GameObject previewBlock;
    [SerializeField] LayerMask blockLayer;
    [SerializeField] float maxDistance; // 최대 설치 거리
    [SerializeField] Vector3 minRange;
    [SerializeField] Vector3 maxRange;
    [SerializeField] Inventory inventory;
    [SerializeField] List<GameObject> blockList = new List<GameObject>();

    Vector3 installPos; // 설치할 위치
    RaycastHit hit;
    Vector3 rayDirection;

    private void Update()
    {
        if (!GameManager.Inst.isBuildMode)
        {
            previewBlock.SetActive(false);
            return;
        }

        rayDirection = GameManager.Inst.isTest ? Camera.main.ScreenPointToRay(Input.mousePosition).direction : Camera.main.transform.forward;

        if (Physics.Raycast(Camera.main.transform.position, rayDirection, out hit, maxDistance, blockLayer))
        {
            Vector3 size = block.transform.lossyScale;

            // 설치할 위치를 나눠서 반올림해서 다시 곱하는 방식
            installPos = Vector3.Scale(Vector3Int.RoundToInt((hit.point + hit.normal.MultiplyVector(size * 0.5f)).DivideVector(size)), size);

            if (GameManager.Inst.isTest && !EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    CraftClick();
                if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
                    BreakClick();
            }
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

    public void CraftClick()
    {
        if (!GameManager.Inst.isBuildMode)
            return;

        if (installPos.InRange(minRange, maxRange))
        {
            GameObject blockObj = Instantiate(block, installPos, Quaternion.identity, instantiateBlock);
            // material 을 새로 만듦
            blockObj.GetComponent<Renderer>().material = inventory.GetMaterial();
            GameManager.PlaySound("craft");
        }
    }

    public void BreakClick()
    {
        if (!GameManager.Inst.isBuildMode)
            return;

        if (hit.collider.CompareTag("Block"))
        {
            Destroy(hit.collider.gameObject);
            GameManager.PlaySound("break");
        }
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
