using UnityEngine;
using System.Collections;

public class DragPiece : MonoBehaviour
{

    private Vector3 screenPoint;
    private Vector3 offset;

    [SerializeField]
    private Piece p;

    void Start()
    {
        p = GetComponent<Piece>();
    }

        void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(mousePos);
    }

    void OnMouseDrag()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 Position = Camera.main.ScreenToWorldPoint(mousePos) + offset;
        p.MovePieceTo(Position);
    }

}