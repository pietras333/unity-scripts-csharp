using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockAbsorbtion : MonoBehaviour
{
    [Header("Shock Absorbtion")]
    [Space]
    [Header("Necessary")]
    [Space]
    [SerializeField] Transform FollowOb;
    [SerializeField] Rigidbody rb;
    [SerializeField] Move move;
    [Space]
    [Header("Customizable")]
    [Space]
    [SerializeField] float ThisLerpSpeed = 6f;
    [SerializeField] float FollowLerpSpeed = 7f;
    [SerializeField] float YPos = .5f;
    [SerializeField] float Timer = 0.15f;
    [HideInInspector] float timer;

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, FollowOb.localPosition, ThisLerpSpeed * Time.deltaTime);
        timer -= Time.deltaTime;
        RaycastHit hit;
        float y = (YPos * rb.velocity.y * (24 / 5)) / (96 / 5);
        if(move.sliding){
            y = Mathf.Clamp(y, -.25f, 2f);
        }else{
            y = Mathf.Clamp(y, -2f, 2f);
        }

        if (Physics.Raycast(transform.parent.transform.position, Vector3.down, out hit, 4f) && !Physics.Raycast(transform.parent.transform.position, Vector3.down, out hit, Time.deltaTime) && rb.velocity.y < -0.2f)
            timer = Timer;
        if (timer > 0)
            FollowOb.transform.localPosition = Vector3.Lerp(FollowOb.transform.localPosition, new Vector3(0, y ,0), FollowLerpSpeed * Time.deltaTime);
        if (timer < 0)
            FollowOb.transform.localPosition = Vector3.Lerp(FollowOb.transform.localPosition, new Vector3(0, 0.4f, 0), FollowLerpSpeed * Time.deltaTime);
    }
}
