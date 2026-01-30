using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class FadeDestroy : MonoBehaviour
{


    //Seconds
    public float FadeTime = 1.0f;
    private float FadeRate = 0.1f;

    private SpriteRenderer spr;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        StartCoroutine(Fade());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Fade()
    {
        Color tmpCol = spr.color;
        while (tmpCol.a >= 0)
        {
            tmpCol.a -= FadeRate;
            spr.color = tmpCol;
            yield return new WaitForSeconds(FadeTime * FadeRate);
        }
        Destroy(gameObject);
        yield return null;
    }
}
