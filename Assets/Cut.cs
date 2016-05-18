using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cut : MonoBehaviour {

    static int total_cuts;
    int ID;
    public int GetID() { return ID;}

    public float percent_across_orig;

    public Vector3 Get_cut_pos() { return transform.position; }
    public void Set_cut_pos(Vector3 p) { transform.position = p; }

    Edge edge;
    public void SetEdge(Edge e) { edge = e; }
    public Edge GetEdge() { return edge; }

    public Cut()
    {
        ID = (total_cuts++)/2;
        Debug.Log("new cut with ID: " + ID);
    }

    public void SetAlignment(Transform t)
    {
        transform.LookAt(t);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

	public bool IfCutsTouch(Cut p_cut)
    {
        if (p_cut != this)
            if (Vector3.Distance(Get_cut_pos(),p_cut.Get_cut_pos()) < 0.01f)
                return true;
        return false;
    }

    public bool IfAnyCutsTouchThis(List<Cut> cuts)
    {
        for (int i = 0; i < cuts.Count; i++) {
            if (IfCutsTouch(cuts[i]))
            {
                return true;
            }
        }
        return false;
    }


}
