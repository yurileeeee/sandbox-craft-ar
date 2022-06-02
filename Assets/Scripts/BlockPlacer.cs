using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] GameObject block;
    [SerializeField] GameObject previewBlock;
    [SerializeField] LayerMask blockLayer;
    [SerializeField] float maxDistance; // �ִ� ��ġ �Ÿ�

    Vector3 installPos; // ��ġ�� ��ġ
    RaycastHit hit;

    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, out hit, maxDistance, blockLayer))
        {
            Vector3 size = block.transform.lossyScale;

            // ��ġ�� ��ġ�� ������ �ݿø��ؼ� �ٽ� ���ϴ� ���
            installPos = Vector3.Scale(Vector3Int.RoundToInt((hit.point + hit.normal.MultiplyVector(size * 0.5f)).DivideVector(size)), size);

            // Ŭ���� (0,0,0) ~ (15, 31, 15) ���� ���̸� ��� ����
            if (Input.GetMouseButtonDown(0) && installPos.InRange(Vector3.zero, new Vector3(15, 31, 15)))
            {
                Instantiate(block, installPos, block.transform.rotation);
            }
        }

        if (hit.transform && installPos.InRange(Vector3.zero, new Vector3(15, 31, 15)))
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
