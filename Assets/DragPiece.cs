using UnityEngine;
using System.Collections;

public class DragPiece : MonoBehaviour
{

    private Vector3 screenPoint;
    private Vector3 offset;

    [SerializeField]
    private Piece p;
    private Camera cam;

    void Start()
    {
        p = GetComponent<Piece>();
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        if (Vector3.Distance(cam.transform.position, p.transform.position) < 4)
        {
            p.SelectPiece();
            if (p.selected)
            {
                Vector3 Position = (cam.gameObject.transform.position + (cam.gameObject.transform.forward * 2));


                //p.MovePieceTo(Position);
                p.transform.SetParent(cam.transform);
            }
        }
        /*screenPoint = cam.WorldToScreenPoint(gameObject.transform.position);
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        offset = gameObject.transform.position - cam.ScreenToWorldPoint(mousePos);
        p.SelectPiece();*/
    }

    void OnMouseDrag()
    {
        if (p.selected)
        {
            //Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            //Vector3 Position = cam.ScreenToWorldPoint(mousePos) + offset;
            //Vector3 Position = (cam.gameObject.transform.position + (cam.gameObject.transform.forward*2));

            //p.transform.position = Position;
            //p.MovePieceTo(Position);
        }
        /*Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 Position = cam.ScreenToWorldPoint(mousePos) + offset;
        p.MovePieceTo(Position);*/
    }

    void OnMouseUp()
    {
        if (p.selected)
        {
            p.SnapToCut();
            p.transform.parent = null;
        }
    }
}