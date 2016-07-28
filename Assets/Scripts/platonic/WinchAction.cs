using UnityEngine;
using System.Collections;

public class WinchAction : IAction
{

    [SerializeField]
    UseableItem item;
    [SerializeField]
    ElementSolid element;

    bool raised;

    const float lerpTime = 0.6f;
    float lerpTimer = 2.0f;

    Vector3 lowest;
    Vector3 highest;

    public void Update()
    {
        if (lerpTimer < lerpTime)
        {
            Debug.Log("lerping");
            lerpTimer += Time.deltaTime;
            element.transform.position = Vector3.Lerp(lowest, highest, (lerpTimer / lerpTime));
            if(element.transform.position == highest)
            {
                Debug.Log("raised");
                raised = true;
                
            }
        }
    }

    public override void Perform()
    {
        if (!raised)
            Raise();
        else
            Release();
    }

    private void Raise()
    {
        if (lerpTimer > lerpTime)
        {
            item.used = false;
            Debug.Log("raise");
            element.LockIn(); // unnecessary if we lock all in once all correct
            lerpTimer = 0.0f;
            lowest = element.transform.position;
            highest = lowest + new Vector3(0, 2, 0);
        }
    }

    private void Release()
    {
        element.Drop();
        Debug.Log("release");
        item.used = true;
    }
}
