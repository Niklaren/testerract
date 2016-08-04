using UnityEngine;
using System.Collections;

public class Saw : MonoBehaviour {

    PentagramPuzzle puzz;

    float sawingTimer = 2.0f;
    float sawTime = 2.0f;

    Vector3 startPos;

    // Use this for initialization
    void Start() {
        startPos = transform.position;
        puzz = FindObjectOfType<PentagramPuzzle>();
    }

    // Update is called once per frame
    void Update() {
        if(sawingTimer < sawTime)
        {
            sawingTimer += Time.deltaTime;
            DoSawX();
            if (puzz.complete)
            {
                DoSawY();
            }
        }
    }

    public void TryUse()
    {
        Debug.Log("Saw: try use");
        sawingTimer = 0.0f;
        if (puzz.complete)
        {

        }
        else
        {

        }
    }

    private void DoSawX()
    {
        float newX = Mathf.Sin((sawingTimer/sawTime) * 2 * Mathf.PI);
        Debug.Log("do saw x. " + newX);
        transform.position = startPos + new Vector3(newX, 0);
    }
    private void DoSawY()
    {
        float newY = startPos.y - (sawingTimer / 2.0f);
        Debug.Log("do saw y. " + newY);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
