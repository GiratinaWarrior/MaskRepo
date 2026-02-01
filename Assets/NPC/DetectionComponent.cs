using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Image;

public class DetectionComponent : MonoBehaviour
{
    public event Action<Transform> PlayerDetected;
    public event Action PlayerLost;

    private Transform LOS;
    private bool losPrev = false;
    private bool losCurr = false;

    private LayerMask _layerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _layerMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {

        losCurr = false;
        if (LOS != null)
        {
            RaycastHit2D hit;
            Color rayColor = Color.red;
            hit = Physics2D.Raycast(transform.position, LOS.position - transform.position, int.MaxValue, _layerMask);
            Debug.DrawRay(transform.position, LOS.position - transform.position, hit ? Color.green : Color.red);

            if (hit.collider != null)
            {
                losCurr = true;
            }

        }
        if (!losPrev && losCurr)
        {
            PlayerDetected?.Invoke(LOS);
            //Debug.Log("Ray Hit Player");
        }
        else if (losPrev && !losCurr)
        {
            PlayerLost?.Invoke();
        }


        losPrev = losCurr;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            return;
            
        }

        //Debug.Log("player collided");
        if (transform.parent.GetComponent<HumanMovement>().HumanAlive()) LOS = collision.transform;

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HumanMovement human = transform.parent.GetComponent<HumanMovement>();
            if (human.HumanSuspicious()) transform.parent.GetComponent<HumanMovement>().ResetHuman();
        }

        LOS = null;
    }
}
