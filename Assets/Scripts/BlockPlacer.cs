//# define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] Transform instantiateBlock;
    [SerializeField] GameObject cubeWithQuads;
    [SerializeField] GameObject previewBlock;
    [SerializeField] GameObject previewSelectBlock;
    [SerializeField] GameObject previewQuad;
    [SerializeField] LayerMask blockLayer;
    [SerializeField] float maxDistance; // 최대 설치 거리
    [SerializeField] Vector3 minRange;
    [SerializeField] Vector3 maxRange;
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject moveButtons;
    public List<GameObject> blockList = new List<GameObject>();

    Vector3 installPos; // 설치할 위치
    RaycastHit hit;
    Vector3 rayDirection;

    public Block selectedBlock;
    [SerializeField] Transform pcPreviewBlock;


    // 선택모드 롱클릭 관련 변수
    bool isArrowClicked = false;
    bool isArrowLongClicked = false;
    string clickedArrowSide;
    float elapsedTime = 0.0f;
    float longClickTime = 1.0f;

    private void Update()
    {
        // 실제 크기 모드 (isBuildMode = false)
        if (!GameManager.Inst.isBuildMode)
        {
            if (selectedBlock != null)
            {
                selectedBlock.isSelected = false;
            }

            previewBlock.SetActive(false);
            return;
        }

        rayDirection = GameManager.Inst.isPC ? Camera.main.ScreenPointToRay(Input.mousePosition).direction : Camera.main.transform.forward;

        if (Physics.Raycast(Camera.main.transform.position, rayDirection, out hit, maxDistance, blockLayer))
        {
            Vector3 size = cubeWithQuads.transform.lossyScale;

            // 설치할 위치 계산
            installPos = Vector3.Scale(Vector3Int.RoundToInt((hit.point + hit.normal.MultiplyVector(size * 0.5f)).DivideVector(size)), size);

            if (GameManager.Inst.isPC/* && !EventSystem.current.IsPointerOverGameObject()*/)
            {
                if (Input.GetKeyDown(KeyCode.F1)/* && !EventSystem.current.IsPointerOverGameObject()*/)
                    CraftClick();
                if (Input.GetKeyDown(KeyCode.F2)/* && !EventSystem.current.IsPointerOverGameObject()*/)
                    BreakClick();
                if (Input.GetKeyDown(KeyCode.Space)/* && !EventSystem.current.IsPointerOverGameObject()*/)
                    SelectClick();
            }
        }

        if (GameManager.Inst.isSelectMode)
        {
            if (GameManager.Inst.isPC)
            {
                if (hit.transform && hit.transform.CompareTag("Quad"))
                {
                    previewSelectBlock.SetActive(true);
                    previewSelectBlock.transform.position = hit.transform.parent.position;
                    previewSelectBlock.transform.rotation = hit.transform.parent.rotation;

                    pcPreviewBlock = hit.transform;
                }
            }
            else
            {
                if (hit.transform && hit.transform.CompareTag("Quad"))
                {
                    previewSelectBlock.SetActive(true);
                    previewSelectBlock.transform.position = hit.transform.parent.position;
                    previewSelectBlock.transform.rotation = hit.transform.parent.rotation;
                }
                else
                {
                    previewSelectBlock.SetActive(false);
                }
            }

            if (selectedBlock == null)
            {
                moveButtons.SetActive(false);
                for (int i = 0; i < blockList.Count; i++)
                {
                    blockList[i].GetComponent<Block>().isSelected = false;
                }
            }
            else
            {
                moveButtons.SetActive(true);

                for (int i = 0; i < selectedBlock.side.Length; i++)
                {
                    if (!selectedBlock.side[i].canMove)
                    {
                        moveButtons.GetComponentsInChildren<Button>()[i].GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
                    }
                    else
                    {
                        moveButtons.GetComponentsInChildren<Button>()[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    }
                }
            }

            if (isArrowClicked)
            {
                elapsedTime += Time.deltaTime;
                if (longClickTime < elapsedTime) isArrowLongClicked = true;
            }

            if (isArrowLongClicked)
            {
                ArrowClick(clickedArrowSide);
            }

            previewBlock.SetActive(false);
            previewQuad.SetActive(false);
        }
        else
        {
            #region PC 모드

            if (GameManager.Inst.isPC)
            {
                if (hit.transform && installPos.InRange(minRange, maxRange))
                {
                    previewBlock.SetActive(true);
                    previewBlock.transform.position = installPos;
                    previewBlock.transform.localScale = cubeWithQuads.transform.lossyScale;

                    previewQuad.SetActive(true);
                    previewQuad.transform.position = hit.transform.position;
                    previewQuad.transform.rotation = hit.transform.rotation;

                    if (hit.transform.CompareTag("Quad"))
                    {
                        previewBlock.SetActive(false);
                    }

                    //previewSelectBlock.SetActive(true);
                    //previewSelectBlock.transform.position = installPos;

                    pcPreviewBlock = hit.transform;
                }
            }
            #endregion

            #region AR 모드
            else
            {
                if (hit.transform && installPos.InRange(minRange, maxRange))
                {
                    previewBlock.SetActive(true);
                    previewBlock.transform.position = installPos;
                    previewBlock.transform.localScale = cubeWithQuads.transform.lossyScale;

                    previewQuad.SetActive(true);
                    previewQuad.transform.position = hit.transform.position;
                    previewQuad.transform.rotation = hit.transform.rotation;
                }
                else
                {
                    previewBlock.SetActive(false);
                    previewQuad.SetActive(false);
                }
            }
            #endregion

            selectedBlock = null;
            previewSelectBlock.SetActive(false);
            for (int i = 0; i < blockList.Count; i++)
            {
                blockList[i].GetComponent<Block>().isSelected = false;
            }
        }
    }

    public void CraftClick()
    {
        if (!GameManager.Inst.isBuildMode)
            return;

        if (installPos.InRange(minRange, maxRange) && !CheckExistBlock(installPos))
        {
            GameObject blockObj = Instantiate(cubeWithQuads, installPos, Quaternion.identity, instantiateBlock);

            Renderer[] quadRenderers = blockObj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in quadRenderers)
            {
                renderer.material = inventory.GetMaterial();
            }

            blockObj.name = "Block " + blockList.Count.ToString();
            blockList.Add(blockObj);

            GameManager.PlaySound("craft");
        }

        if (GameManager.Inst.isPC)
        {
            pcPreviewBlock = null;
        }
    }

    public void BreakClick()
    {
        if (!GameManager.Inst.isBuildMode)
            return;

        if (GameManager.Inst.isPC)
        {
            if (pcPreviewBlock.CompareTag("Quad"))
            {
                blockList.Remove(blockList.Find(x => x.name == pcPreviewBlock.parent.name));
                Destroy(pcPreviewBlock.parent.gameObject);

                previewBlock.SetActive(false);
                previewQuad.SetActive(false);
                //previewSelectBlock.SetActive(false);
                pcPreviewBlock = null;
            }
        }
        else
        {
            if (hit.transform.CompareTag("Quad"))
            {
                blockList.Remove(blockList.Find(x => x.name == hit.transform.parent.name));
                Destroy(hit.transform.parent.gameObject);
            }
        }

        GameManager.PlaySound("break");
    }

    public void SelectClick()
    {
        if (!GameManager.Inst.isBuildMode)
            return;

        if (GameManager.Inst.isSelectMode)
        {
            if (GameManager.Inst.isPC)
            {
                if (pcPreviewBlock.CompareTag("Quad"))
                {
                    Block hitBlock = pcPreviewBlock.parent.GetComponent<Block>();
                    SelectBlock(hitBlock);

                    previewSelectBlock.SetActive(false);
                    pcPreviewBlock = null;
                }
            }
            else
            {
                if (hit.transform.CompareTag("Quad"))
                {
                    Block hitBlock = hit.transform.parent.GetComponent<Block>();
                    SelectBlock(hitBlock);
                }
            }
        }
    }

    public void SelectBlock(Block selectedBlock)
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            blockList[i].GetComponent<Block>().isSelected = false;
        }
        selectedBlock.isSelected = true;
        this.selectedBlock = selectedBlock;
    }

    public void ArrowClick(string side)
    {
        if (!GameManager.Inst.isBuildMode)
            return;

        Vector3 selectNextPos = selectedBlock.transform.position;

        if (side == "Left")
        {
            selectNextPos -= new Vector3(1, 0, 0);
        }
        else if (side == "Right")
        {
            selectNextPos += new Vector3(1, 0, 0);
        }
        else if (side == "Back")
        {
            selectNextPos += new Vector3(0, 0, 1);
        }
        else if (side == "Front")
        {
            selectNextPos -= new Vector3(0, 0, 1);
        }
        else if (side == "Bottom")
        {
            selectNextPos -= new Vector3(0, 1, 0);
        }
        else if (side == "Top")
        {
            selectNextPos += new Vector3(0, 1, 0);
        }

        if (selectNextPos.InRange(minRange, maxRange))
        {
            if (!CheckExistBlock(selectNextPos))
            {
                GameObject blockObj = Instantiate(cubeWithQuads, selectNextPos, Quaternion.identity, instantiateBlock);

                Renderer[] quadRenderers = blockObj.GetComponentsInChildren<Renderer>();
                foreach (var renderer in quadRenderers)
                {
                    renderer.material = selectedBlock.side[0].GetComponent<Renderer>().material;
                }

                blockObj.name = "Block " + blockList.Count.ToString();
                blockList.Add(blockObj);

                SelectBlock(blockObj.GetComponent<Block>());
                GameManager.PlaySound("craft");
            }
            else
            {
                //blockList.Remove(blockList.Find(x => x.transform.position == selectNextPos));
                //Destroy(blockList.Find(x => x.transform.position == selectNextPos));

                blockList.Remove(blockList.Find(x => x.transform.position == selectedBlock.transform.position));
                Destroy(selectedBlock.gameObject);

                Block nextBlock = blockList.Find(x => x.transform.position == selectNextPos).GetComponent<Block>();
                SelectBlock(nextBlock);

                GameManager.PlaySound("break");
            }
        }
    }

    public void ArrowPointerDown(string side)
    {
        isArrowClicked = true;
        clickedArrowSide = side;
    }

    public void ArrowPointerUp()
    {
        isArrowClicked = false;
        isArrowLongClicked = false;
        elapsedTime = 0.0f;
    }

    private bool CheckExistBlock(Vector3 pos)
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            if (blockList[i].transform.position == pos)
            {
                return true;
            }
        }

        return false;
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
