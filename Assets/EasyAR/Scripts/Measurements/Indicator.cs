using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    private bool inrange = false;
    public GameObject indicator;
    private Collider colliderObject;

    // Start is called before the first frame update
    void Start()
    {
        indicator.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inrange)
        {
            indicator.transform.position = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Point")
        {
            inrange = true;
            StartCoroutine(moveToPoint(other));
            Handheld.Vibrate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Point")
        {
            StartCoroutine(moveToIndicator(other));
            inrange = false;
        }
    }

    IEnumerator moveToPoint(Collider other)
    {
        float totalMovementTime = 0.5f; //the amount of time you want the movement to take
        float currentMovementTime = 0f;//The amount of time that has passed
        while (Vector3.Distance(indicator.transform.localPosition, other.transform.position) > 0 && inrange)
        {
            currentMovementTime += Time.deltaTime;
            indicator.transform.position = Vector3.Lerp(transform.position, other.transform.position, currentMovementTime / totalMovementTime);
            yield return null;
        }
    }


    IEnumerator moveToIndicator(Collider other)
    {
        float totalMovementTime = 0.5f; //the amount of time you want the movement to take
        float currentMovementTime = 0f;//The amount of time that has passed
        while (Vector3.Distance(indicator.transform.localPosition, transform.position) > 0.05)
        {
            currentMovementTime += Time.deltaTime;
            indicator.transform.position = Vector3.Lerp(indicator.transform.position, transform.position, currentMovementTime / totalMovementTime);
            yield return null;
        }
    }
}
