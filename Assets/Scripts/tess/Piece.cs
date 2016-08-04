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

    //Collider[] colliders;
    List<Collider> colliders = new List<Collider>();

    List<GameObject> spheres = new List<GameObject>();

    List<Cut> cuts = new List<Cut>();

    Puzzle puzzle;
    public void SetPuzzle(Puzzle p) { puzzle = p; }
    bool core = false;
    public bool selected = false;
    bool selectedThisUpdate = false;

    //GameObject PieceObject;
    public Vector3 GetPosition() { return NonRotatedPosition + LocalCentre; }//gameObject.transform.position; }
    Vector3 RotatedDelta;
    Vector3 LocalCentre;
    Vector3 NonRotatedPosition;

    private Vector3 previousPosition;

    const float lerpTime = 0.6f;
    float lerpTimer = 2.0f;
    Quaternion start_rot;
    Quaternion end_rot;

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

        previousPosition = rb.position;
    }

    
    public void Update()
    {
        if (selected)
        {
            selectedThisUpdate = false;

            if (lerpTimer < lerpTime)
            {
                Debug.Log("slerping");
                lerpTimer += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(start_rot, end_rot, (lerpTimer / lerpTime));
            }
          
        }
    }

    public void FixedUpdate()
    {
        if (selected) 
        {
            // place in front of cam
            Camera cam = Camera.main;
            Vector3 Position = (cam.gameObject.transform.position + (cam.gameObject.transform.forward *2.0f));
            transform.position = Position;

            // check distance from floor
            //Collider[] colliders = GetComponentsInChildren<Collider>();
            float floorY = -1.55f; // prototype code magic number
            float minY = floorY;


            foreach (Collider collider in colliders)
            {
                GameObject player = GameObject.Find("FPSController");//GameObject.Find("RigidBodyFPSController");
                while (collider.bounds.Intersects(player.GetComponent<Collider>().bounds))
                {
                    Vector3 translation = (cam.gameObject.transform.forward * 0.05f);

                    transform.Translate(translation, Space.World);
                }

                if (collider.bounds.min.y < minY)
                    minY = collider.bounds.min.y;
            }
            float diff = floorY - minY;
            //Debug.Log("diff " + diff);

            // if beneath floor
            if (diff > 0)
            {
                Debug.Log("translate up");
                transform.Translate(0, diff, 0, Space.World);
                //Vector3 translation = (cam.gameObject.transform.forward * 0.1f);
                //transform.Translate(translation);
            }
        }
    }

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
        //RecordCuts();
        RecordCentre();

        for (int i = 0; i < edges.Count; i++)
        {
            //edges[i].AddCutOnUnconnected();

            GameObject EdgeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            EdgeObject.transform.position = edges[i].Get_Position();// + (edges[i].Get_Position() - LocalCentre);   //Get_point_a();  //
            EdgeObject.transform.up = edges[i].Get_point_b() - edges[i].Get_point_a();
            EdgeObject.transform.localScale = new Vector3(0.1f, Vector3.Distance(edges[i].Get_point_a(), edges[i].Get_point_b()) / 2, 0.1f);

            EdgeObject.transform.SetParent(gameObject.transform);

            EdgeObject.name = "cylinder" + edges[i].GetID();
            EdgeObject.layer = 9;

            //parent cuts to edge
            if (edges[i].Get_cut_a() != null)
            {
                edges[i].Get_cut_a().gameObject.transform.SetParent(EdgeObject.transform);
                edges[i].Get_cut_a().SetAlignment(EdgeObject.transform);
            }
            else
            {
                bool needs_awesome_sphere = true;
                for (int j = 0; j < spheres.Count; j++)
                {
                    if (spheres[j].transform.position == edges[i].Get_point_a())
                    {
                        needs_awesome_sphere = false;
                        break;
                    }
                }
                if (needs_awesome_sphere)
                {
                    GameObject SphereObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    SphereObject.transform.position = edges[i].Get_point_a();
                    SphereObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                    SphereObject.transform.SetParent(gameObject.transform);

                    spheres.Add(SphereObject);
                }
            }
            if (edges[i].Get_cut_b() != null)
            {
                edges[i].Get_cut_b().gameObject.transform.SetParent(EdgeObject.transform);
                edges[i].Get_cut_b().SetAlignment(EdgeObject.transform);
            }
            else
            {
                bool needs_awesome_sphere = true;
                for (int j = 0; j < spheres.Count; j++)
                {
                    if (spheres[j].transform.position == edges[i].Get_point_b())
                    {
                        needs_awesome_sphere = false;
                        break;
                    }
                }
                if (needs_awesome_sphere)
                {
                    GameObject SphereObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    SphereObject.transform.position = edges[i].Get_point_b();
                    SphereObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                    SphereObject.transform.SetParent(gameObject.transform);

                    spheres.Add(SphereObject);
                }
            }

            colliders.Add(EdgeObject.GetComponent<Collider>());
        }

        RecordCuts();
        return this;
    }

    // if the cut appears twice in the same piece, it should get joined.
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
                        Debug.Log("try joining now. cut: "+ c1.GetID() + " with " +c2.GetID());
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
        Debug.Log("snaptocut");

        CheckForSnap();

        

        puzzle.CheckComplete();
    }

    private void CheckForSnap()
    {
        List<Cut> all_cuts = puzzle.Get_all_cuts();

        Debug.Log("cuts.Count " + cuts.Count);
        Debug.Log("allcuts.Count " + all_cuts.Count);

        for (int i = 0; i < cuts.Count; i++)
        {
            for (int j = 0; j < all_cuts.Count; j++)
            {
                // if its not belonging to the same piece
                if (cuts[i].GetEdge().GetPiece() != all_cuts[j].GetEdge().GetPiece())
                {

                    // if the cuts are close enough to snap together
                    if (Vector3.Distance(cuts[i].Get_cut_pos(), all_cuts[j].Get_cut_pos()) < 0.3f)
                    {
                        all_cuts[j].GetEdge().GetPiece().RoundTo90();
                        Vector3 iF = -cuts[i].transform.forward;
                        Vector3 jF = all_cuts[j].transform.forward;
                        if (DoVectorsAlign(iF, jF))
                        {
                            // if other cut is already connected to any other cut, then don't snap to it
                            List<Cut> AllOtherCuts = new List<Cut>(all_cuts);
                            AllOtherCuts.Remove(cuts[i]);
                            AllOtherCuts.Remove(all_cuts[j]);

                            if (all_cuts[j].IfAnyCutsTouchThis(AllOtherCuts))
                            {
                                Debug.Log("cut " + all_cuts[j].GetID() + " already connected - don't snap");
                                //return;
                                break;
                            }
                            Debug.Log("cut " + all_cuts[j].GetID() + " of piece " + all_cuts[j].GetEdge().GetPiece().gameObject.name +
                                " is unconnected. snap with cut " + cuts[i].GetID());

                            //Debug.Log("this cut loc rot eul at " + cuts[i].transform.localEulerAngles.ToString("F4"));
                            //Debug.Log("this cut rot eul at " + cuts[i].transform.rotation.eulerAngles.ToString("F4"));
                            //Debug.Log("this cut rot quat at " + cuts[i].transform.rotation.ToString("F4"));
                            //Debug.Log("this cut FORWARD " + cuts[i].transform.forward.ToString("F4"));
                            //Debug.Log("this cut FORWARD mag " + cuts[i].transform.forward.magnitude.ToString("F4"));
                            //Debug.Log("other cut loc rot eul at " + all_cuts[j].transform.localEulerAngles.ToString("F4"));
                            //Debug.Log("other cut rot eul at " + all_cuts[j].transform.rotation.eulerAngles.ToString("F4"));
                            //Debug.Log("other cut rot quat at " + all_cuts[j].transform.rotation.ToString("F4"));
                            //Debug.Log("other cut FORWARD " + all_cuts[j].transform.forward.ToString("F4"));
                            //Debug.Log("other cut FORWARD mag " + all_cuts[j].transform.forward.magnitude.ToString("F4"));

                            //cuts[i].transform.forward = -all_cuts[j].transform.forward;


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
                            //Debug.Log("axis: " + axis);
                            float angle = Vector3.Angle(iF, jF);
                            //Debug.Log("angle: " + angle);

                            //cuts[i].gameObject.transform.Rotate(axis, angle, Space.World);
                            //Debug.Log(gameObject.transform.position.ToString("f4"));

                            gameObject.transform.Rotate(axis, angle, Space.World);

                            RoundTo90();

                            //RotatedDelta = transform.position - NonRotatedPosition;

                            // if cuts correctly align after rotation then connect them up
                            if (DoVectorsAlign(-cuts[i].transform.forward, jF))
                            {
                                Vector3 d = all_cuts[j].Get_cut_pos() - cuts[i].Get_cut_pos();
                                //Debug.Log("this cut at " + cuts[i].Get_cut_pos().ToString("F4"));
                                //Debug.Log("other cut at " + all_cuts[j].Get_cut_pos().ToString("F4"));
                                Debug.Log("move: " + d.ToString("F4"));
                                MovePieceDelta(d);

                                return;
                            }
                        }
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
            //gameObject.transform.position -= LocalCentre;

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
        transform.Rotate(r, angle, Space.World);;

        //transform.position += LocalCentre;
        //transform.position += p1;

        
    }

    public void RotateTween(Vector3 r, float angle)
    {
        Vector3 r1 = transform.rotation.eulerAngles;
        transform.Rotate(r, angle, Space.World);
        Vector3 r2 = transform.rotation.eulerAngles;
        r.Normalize();
        //Vector3 ra = (r * angle);
        //Vector3 r3 = r1 + ra;
        //Vector3 r4 = r3;
        //r3.x = r3.x % 360;
        //r3.y = r3.y % 360;
        //r3.z = r3.z % 360;
        //r3.x = Mathf.Round(r3.x / 90) * 90;
        //r3.y = Mathf.Round(r3.y / 90) * 90;
        //r3.z = Mathf.Round(r3.z / 90) * 90;
        Vector3 roundME = gameObject.transform.eulerAngles;
        transform.Rotate(r, -angle, Space.World);
        
        
        roundME.x = Mathf.Round(roundME.x / 90) * 90;
        roundME.y = Mathf.Round(roundME.y / 90) * 90;
        roundME.z = Mathf.Round(roundME.z / 90) * 90;
        //Debug.Log("r1: " + r1 + "  ra: " + ra + "  r4: " + r4 + "  r2: " + r2 + "  r3: " + r3 +  "  rm: " + roundME);


        start_rot = transform.rotation;
        end_rot = Quaternion.Euler(roundME);
        
        //Debug.Log("q_cur " + start_rot + " q_rot " + end_rot);

        lerpTimer = 0.0f;

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, q_rot, 0.1f * Time.deltaTime);//Quaternion.Slerp(q_cur, q_rot, Time.deltaTime);

        //LeanTween.rotate(gameObject, roundME, 0.6f);
        //LeanTween.rotate()
    }

    public void ResetRotation()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        //Debug.Log(gameObject.transform.eulerAngles);
        RotatedDelta = Vector3.zero;

    }

    public void RoundTo90()
    {
        //Vector3 p1 = gameObject.transform.position;

        Vector3 roundME = gameObject.transform.eulerAngles;
        roundME.x = Mathf.Round(roundME.x / 90) * 90;
        roundME.y = Mathf.Round(roundME.y / 90) * 90;
        roundME.z = Mathf.Round(roundME.z / 90) * 90;
        Debug.Log("angles should be snapped from " + gameObject.transform.eulerAngles.ToString("f4") + " to " + roundME.ToString("f4"));
        gameObject.transform.eulerAngles = roundME;

        

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

    private bool DoVectorsAlign(Vector3 a, Vector3 b)
    {
        // check if angles align (within tolerance)
        float tolerance = 0.4f;
        if ((Mathf.Abs(a.x - b.x) < tolerance) && (Mathf.Abs(a.y - b.y) < tolerance) && (Mathf.Abs(a.z - b.z) < tolerance))
        {
            return true;
        }
        return false;
    }

    private bool DoPiecesOverlap(Piece p)
    {
        if(this == p)
        {
            return true; // or false... maybe...
        }
        Collider[] p_colliders = p.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            foreach(Collider p_collider in p_colliders)
            {
                if (collider.bounds.Intersects(p_collider.bounds))
                    return true;
            }
        }

        return false;
    }

    public void Freeze()
    {
        //rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnFreeze()
    {
        if (!core)
        {
            //rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void SelectPiece()
    {
        selectedThisUpdate = true;

        if(puzzle.selected != null)
            puzzle.selected.DoDeSelection();
        DoSelection();
    }
    public void DeSelectPiece()
    {
        if (!selectedThisUpdate)
        {
            DoDeSelection();
            SnapToCut();
            puzzle.ConfigurePiecePhysics();
        }
    }

    public void DoDeSelection()
    {
        puzzle.selected = null;
        selected = false;
        MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
        foreach(var mr in MRs)
        {
            mr.material = mat_default;
        }
        transform.SetParent(puzzle.transform);
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
