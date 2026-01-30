using System.Drawing;
using UnityEngine;

public class GrowFadeScript : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float growthRate = 0.1f;
    [SerializeField] private float maxSize = 2;
    private float size = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = Vector3.one;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position;
        }

        size += growthRate;

        transform.localScale = new Vector3(1, 1, 0) * size;

        if (size > maxSize)
        {
            gameObject.GetComponent<FadeDestroy>().enabled = true;
        }

    }
}
