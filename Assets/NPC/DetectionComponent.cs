using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Image;

public class DetectionComponent : MonoBehaviour
{
    public event Action PlayerDetected;

    private Transform LOS;

    private LayerMask _layerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _layerMask = Physics.AllLayers;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (LOS != null)
        {
            RaycastHit2D hit;
            Color rayColor = Color.red;
            hit = Physics2D.Raycast(transform.position, LOS.position - transform.position, _layerMask);
            if (hit.transform != null)
            {
                rayColor = Color.green;
                Debug.Log("Player in sight");
                PlayerDetected.Invoke();
            }
            Debug.DrawRay(transform.position, LOS.position - transform.position, rayColor);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;
            Debug.Log("player collided");
        PlayerDetected.Invoke();

        LOS = collision.transform;

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        LOS = null;
    }
}
