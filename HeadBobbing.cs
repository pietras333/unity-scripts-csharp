using UnityEngine;

public class HeadBobbing : MonoBehaviour 
{
    [Header("Head Bobbing")]
    [Space]
    [Header("Necessary")]
    [Space]
    [SerializeField] Move move;
    [SerializeField] Transform cam;
    [SerializeField] bool enable = true;
    [Space]
    [Header("Customizable")]
    [Space]
    [SerializeField, Range(0, 0.5f)] float amplitude = 0.015f; 
    [SerializeField, Range(0, 30)] float frequency = 10.0f;
    [SerializeField] float toggleSpeed = 3.0f;
    [HideInInspector] Vector3 startPos;

    private void Awake()
    {
        startPos = cam.localPosition;
    }

    void Update()
    {
        if (!enable) return;
        checkMotion();
        resetPosition();
    }
    void checkMotion()
    {
        float speed = new Vector3(move.velocity.x, 0, move.velocity.z).magnitude;
        if (speed < toggleSpeed) return;
        if (!move.grounded) return;

        playMotion(footStepMotion());
    }

    void playMotion(Vector3 motion)
    {
        cam.localPosition += motion;
    }

    void resetPosition()
    {
        if (cam.localPosition == startPos) return;
        cam.localPosition = Vector3.Lerp(cam.localPosition, startPos, 1 * Time.deltaTime);
    }
    Vector3 footStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos;
    }

}
