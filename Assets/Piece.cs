using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {

    [SerializeField]
    Material mat_default;
    [SerializeField]
    Material mat_core;
    [SerializeField]
    Material mat_selected;

    List<Edge> edges = new List<Edge>();
    public List<Edge> Get_edges() { return edges; }

    List<Cut> cuts = new List<Cut>();

    Puzzle puzzle;
    public void SetPuzzle(Puzzle p) { puzzle = p; }
    bool core = false;
    public bool selected = false;

    //GameObject PieceObject;
    public Vector3 GetPosition() { return NonRotatedPosition + LocalCentre; }//gameObject.transform.position; }
    Vector3 RotatedDelta;
    Vector3 LocalCentre;
    Vector3 NonRotatedPosition;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //Physics.IgnoreLayerCollision(8, 9, true);
    }

    private void Start()
    {
        if (puzzle == null)
            puzzle = FindObjectOfType<Puzzle>();
    }

    /*
    public void Update()
    {
        if(this == puzzle.selected)
        {

        }
    }
    */

    public void Reset()
    {
        edges.Clear();
        cuts.Clear();
    }

    public void AddEdge(Edge e)
    {
        if (!edges.Contains(e))
        {
            edges.Add(e);
            e.SetPiece(this);
        }
    }

    public Piece CreatePieceObject()
    {
        RecordCuts();
        JoinUnusedCuts();
        RecordCuts();
        RecordCentre();

        for (int i = 0; i < edges.Count; i++)
        {
            GameObject EdgeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            EdgeObject.transform.position = edges[i].Get_Position()-LocalCentre;   //Get_point_a();  //
            EdgeObject.transform.up = edges[i].Get_point_b() - edges[i].Get_point_a();
            EdgeObject.transform.localScale = new Vector3(0.1f, Vector3.Distance(edges[i].Get_point_a(), edges[i].Get_point_b()) / 2, 0.1f);

            EdgeObject.transform.SetParent(gameObject.transform);

            EdgeObject.name = "cylinder" + edges[i].GetID();

            //parent cuts to edge
            if (edges[i].Get_cut_a() != null)
            {
                edges[i].Get_cut_a().gameObject.transform.SetParent(EdgeObject.transform);
                edges[i].Get_cut_a().SetAlignment(EdgeObject.transform);
            }
            if (edges[i].Get_cut_b() != null)
            {
                edges[i].Get_cut_b().gameObject.transform.SetParent(EdgeObject.transform);
                edges[i].Get_cut_b().SetAlignment(EdgeObject.transform);
            }
        }
        return this;
    }

    private void JoinUnusedCuts()
    {
        //Debug.Log("try join unused");
        foreach (Cut c1 in cuts)
        {
            foreach (Cut c2 in cuts)
            {
                if(c1!= c2)
                {
                    if(c1.GetID() == c2.GetID())
                    {
                        //Debug.Log("try joining now");
                        c1.GetEdge().join_cuts(c2.GetEdge());
                    }
                }
            }
        }
    }

    private void RecordCuts()
    {
        cuts.Clear();
        foreach (Edge e in edges)
        {
            if (e.Get_cut_a() != null)
            {
                if (!cuts.Contains(e.Get_cut_a()))
                    cuts.Add(e.Get_cut_a());
            }
            if (e.Get_cut_b() != null)
            {
                if (!cuts.Contains(e.Get_cut_b()))
                    cuts.Add(e.Get_cut_b());
            }
        }
    }

    private void RecordCentre()
    {
        LocalCentre = Vector3.zero;
        foreach (Edge e in edges)
        {
            LocalCentre += e.Get_Position();
        }
        LocalCentre /= edges.Count;

        //LocalCentre += transform.position;

        gameObject.transform.position = LocalCentre;
    }

    public bool IsPieceConnected()
    {
        for(int i = 0; i < cuts.Count; i++)
        {
            if(cuts[i].IfAnyCutsTouchThis(puzzle.Get_all_cuts()))
            {
                return true;
            }
        }

        return false;
    }

    public void SnapToCut()
    {
        CheckForSnap();

        puzzle.ConfigurePiecePhysics();
    }

    private void CheckForSnap()
    {
        List<Cut> all_cuts = puzzle.Get_all_cuts();

        for (int i = 0; i < cuts.Count; i++)
        {
            for (int j = 0; j < all_cuts.Count; j++)
            {
                if ((cuts[i] != all_cuts[j]) && (cuts[i].GetEdge() != all_cuts[j].GetEdge()))
                {
                    if (Vector3.Distance(cuts[i].Get_cut_pos(), all_cuts[j].Get_cut_pos()) < 0.1f)
                    {
                        //Debug.Log("this cut loc rot eul at " + cuts[i].transform.localEulerAngles.ToString("F4"));
                        //Debug.Log("this cut rot eul at " + cuts[i].transform.rotation.eulerAngles.ToString("F4"));
                        //Debug.Log("this cut rot quat at " + cuts[i].transform.rotation.ToString("F4"));
                        Debug.Log("this cut FORWARD " + cuts[i].transform.forward.ToString("F4"));
                        //Debug.Log("this cut FORWARD mag " + cuts[i].transform.forward.magnitude.ToString("F4"));
                        //Debug.Log("other cut loc rot eul at " + all_cuts[j].transform.localEulerAngles.ToString("F4"));
                        // Debug.Log("other cut rot eul at " + all_cuts[j].transform.rotation.eulerAngles.ToString("F4"));
                        //Debug.Log("other cut rot quat at " + all_cuts[j].transform.rotation.ToString("F4"));
                        Debug.Log("other cut FORWARD " + all_cuts[j].transform.forward.ToString("F4"));
                        //Debug.Log("other cut FORWARD mag " + all_cuts[j].transform.forward.magnitude.ToString("F4"));

                        //cuts[i].transform.forward = -all_cuts[j].transform.forward;

                        all_cuts[j].GetEdge().GetPiece().RoundTo90();

                        Vector3 iF = -cuts[i].transform.forward;
                        Vector3 jF = all_cuts[j].transform.forward;
                        float tolerance = 1.1f;
                        if ((Mathf.Abs(iF.x - jF.x) < tolerance) && (Mathf.Abs(iF.x - jF.x) < tolerance) && (Mathf.Abs(iF.x - jF.x) < tolerance))
                        {
                            // to do angle to rotate between the 2 vectors
                            // look this up later :3
                            /*float a = Vector3.Angle(iF, jF);
                            Vector3 dif = iF - jF;
                            Debug.Log("dif "+ dif);
                            Debug.Log("angle " + a);

                            Vector3 difx = new Vector3(dif.x, 0, 0);
                            Vector3 dify = new Vector3(0, dif.y, 0);
                            Vector3 difz = new Vector3(0, 0, dif.z);

                            Vector3 p = NonRotatedPosition + LocalCentre;
                            //gameObject.transform.Rotate(dif.normalized, a);
                            gameObject.transform.RotateAround(p, difx, a);
                            gameObject.transform.RotateAround(p, dify, a);
                            gameObject.transform.RotateAround(p, difz, a);*/

                            //Vector3 iiF = new Vector3(0, 1, 0);
                            //Vector3 jjF = new Vector3(0, 1, 0);

                            Vector3 axis = Vector3.Cross(iF, jF);
                            Debug.Log("axis: " + axis);
                            float angle = Vector3.Angle(iF, jF);
                            Debug.Log("angle: " + angle);

                            //cuts[i].gameObject.transform.Rotate(axis, angle, Space.World);
                            Debug.Log(gameObject.transform.position.ToString("f4"));

                            gameObject.transform.Rotate(axis, angle, Space.World);

                            RoundTo90();

                            RotatedDelta = transform.position - NonRotatedPosition;

                        }

                        Vector3 d = all_cuts[j].Get_cut_pos() - cuts[i].Get_cut_pos();
                        //Debug.Log("this cut at " + cuts[i].Get_cut_pos().ToString("F4"));
                        //Debug.Log("other cut at " + all_cuts[j].Get_cut_pos().ToString("F4"));
                        //Debug.Log("move: " + d.ToString("F4"));
                        MovePieceDelta(d);


                        return;
                    }
                }
            }
        }
    }

    public void MovePieceTo(Vector3 v)
    {
        if (!core)
        {
            gameObject.transform.position = v;
            //gameObject.transform.position += RotatedDelta;
            gameObject.transform.position -= LocalCentre;

            //SnapToCut();
            NonRotatedPosition = v + RotatedDelta;
        }
    }

    public void MovePieceDelta(Vector3 v)
    {
        //Debug.Log("move: " + v.ToString("F4"));
        gameObject.transform.position = (gameObject.transform.position + v);
        //gameObject.transform.
        NonRotatedPosition += v;
        puzzle.CheckComplete();
    }

    public void RotatePiece(Vector3 r, float angle)
    {
        //Vector3 p = NonRotatedPosition + LocalCentre;
        ////Debug.Log("Rotated Around " + p.ToString("F4") + "with loc pos " + LocalCentre.ToString("F4") + "and non-rotated pos " + NonRotatedPosition.ToString("F4"));
        //gameObject.transform.RotateAround(p,r,angle);
        //RotatedDelta = transform.position - NonRotatedPosition;
        ////Debug.Log("RotatedDelta " + RotatedDelta.ToString("F4"));

        //Vector3 p1 = transform.position;
        //transform.position = Vector3.zero;
        //transform.position -= LocalCentre;
        transform.Rotate(r, angle, Space.World);
        //transform.position += LocalCentre;
        //transform.position += p1;

    }

    public void ResetRotation()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        Debug.Log(gameObject.transform.eulerAngles);
        RotatedDelta = Vector3.zero;
    }

    public void RoundTo90()
    {
        //Vector3 p1 = gameObject.transform.position;

        Vector3 roundME = gameObject.transform.eulerAngles;
        roundME.x = Mathf.Round(roundME.x / 90) * 90;
        roundME.y = Mathf.Round(roundME.y / 90) * 90;
        roundME.z = Mathf.Round(roundME.z / 90) * 90;
        gameObject.transform.eulerAngles = roundME;

        //Debug.Log("angles should be snapped to 90 " + gameObject.transform.eulerAngles.ToString("f4"));

        //Vector3 p2 = gameObject.transform.position;

        //Vector3 diff = p1 - p2;

        //gameObject.transform.position += diff;

    }

    public void RoundToWhole()
    {
        //Vector3 p1 = gameObject.transform.position;

        Vector3 roundME = gameObject.transform.eulerAngles;
        roundME.x = Mathf.Round(roundME.x);
        roundME.y = Mathf.Round(roundME.y );
        roundME.z = Mathf.Round(roundME.z);
        gameObject.transform.eulerAngles = roundME;

        Debug.Log("angles should be snapped to 1 " + gameObject.transform.eulerAngles.ToString("f4"));

        //Vector3 p2 = gameObject.transform.position;

        //Vector3 diff = p1 - p2;

        //gameObject.transform.position += diff;

    }

    public void Freeze()
    {
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnFreeze()
    {
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
    }

    public void SelectPiece()
    {
        if(puzzle.selected != null)
            puzzle.selected.DoDeSelection();
        DoSelection();
    }

    private void DoDeSelection()
    {
        puzzle.selected = null;
        selected = false;
        MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
        foreach(var mr in MRs)
        {
            mr.material = mat_default;
        }


    }
    private void DoSelection()
    {
        if (!core)
        {
            puzzle.selected = this;
            selected = true;
            MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in MRs)
            {
                mr.material = mat_selected;
            }

            Freeze();
        }
    }
    public void SetCore()
    {
        core = true;
        MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in MRs)
        {
            mr.material = mat_core;
        }

        Freeze();
    }
}
