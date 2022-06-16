using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSide : MonoBehaviour
{
    [SerializeField] float maxDistance = 1f;
    public bool canMove = false;

    RaycastHit hit;

    private void Update()
    {
        // 1. maxDistance 앞쪽에 quad 가 있는지 판단
        //Debug.DrawRay(transform.position, -transform.forward * maxDistance, Color.red);
        if (Physics.Raycast(transform.position, -transform.forward, out hit, maxDistance))
        {
            //Debug.Log(this.name + " ::: " + hit.transform.name);
            if (hit.collider.CompareTag("Quad"))
            {
                canMove = false;
            }
        }
        else
        {
            canMove = true;
        }

        // 2. 해당 면 다음 위치가 16 * 16 영역 안인지 판단
        Vector3 nextPos = transform.parent.transform.position;
        nextPos = CalculateNextPos(nextPos);
        if (!nextPos.InRange(new Vector3(0, 0, 0), new Vector3(15, 31, 15)))
        {
            canMove = false;
        }
    }

    private Vector3 CalculateNextPos(Vector3 pos)
    {
        if (this.name == "Left")
        {
            return pos -= new Vector3(1, 0, 0);
        }
        else if (this.name == "Right")
        {
            return pos += new Vector3(1, 0, 0);
        }
        else if (this.name == "Back")
        {
            return pos += new Vector3(0, 0, 1);
        }
        else if (this.name == "Front")
        {
            return pos -= new Vector3(0, 0, 1);
        }
        else if (this.name == "Bottom")
        {
            return pos -= new Vector3(0, 1, 0);
        }
        else if (this.name == "Top")
        {
            return pos += new Vector3(0, 1, 0);
        }
        return Vector3.zero;
    }
}
