using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentBasketGame : MonoBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] Transform ball;
    [SerializeField] Transform arms;
    [SerializeField] Transform posOverhead;
    [SerializeField] Transform posDribble;
    [SerializeField] Transform target;
    [SerializeField] float missRange = 0.5f;
    [SerializeField] Vector3 minBounds;
    [SerializeField] Vector3 maxBounds;
    [SerializeField] float throwHeight = 5.0f;
    [SerializeField] float stealRange = 0.5f; 
    private BallController ballController;
    public bool isBallInHands = false; 
    private bool isBallFlying = false;
    private float t = 0;
    private Vector3 finalTargetPosition;

    void Start()
    {
        ballController = ball.GetComponent<BallController>();
    }

    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("P2Vertical"), 0, -Input.GetAxisRaw("P2Horizontal"));
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        newPosition = ClampPosition(newPosition);
        transform.position = newPosition;

        transform.LookAt(transform.position + direction);

        if (isBallInHands)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ball.position = posOverhead.position;
                arms.localEulerAngles = Vector3.right * 180;
                transform.LookAt(target.position);
            }
            else
            {
                ball.position = posDribble.position + Vector3.up / 2 * Mathf.Abs(Mathf.Sin(Time.time * 5));
                arms.localEulerAngles = Vector3.right * 0;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isBallInHands = false;
                isBallFlying = true;
                t = 0;
                finalTargetPosition = GetFinalTargetPosition();
                ballController.ChangeHolder(null);
            }
        }

        if (isBallFlying)
        {
            t += Time.deltaTime;
            float duration = 1.0f;
            float t01 = t / duration;

            Vector3 start = posOverhead.position;
            Vector3 end = finalTargetPosition;
            Vector3 mid = (start + end) / 2 + Vector3.up * throwHeight;

            Vector3 pos = (1 - t01) * (1 - t01) * start + 2 * (1 - t01) * t01 * mid + t01 * t01 * end;

            ball.position = ClampPosition(pos);

            if (t01 >= 1)
            {
                isBallFlying = false;
                ball.GetComponent<Rigidbody>().isKinematic = false;
                ball.GetComponent<Rigidbody>().useGravity = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            TryStealBall();
        }
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        position.z = Mathf.Clamp(position.z, minBounds.z, maxBounds.z);
        return position;
    }

    private Vector3 GetFinalTargetPosition()
    {
        int outcome = Random.Range(0, 3);
        Vector3 offset = Vector3.zero;
        switch (outcome)
        {
            case 0:
                offset = Vector3.zero;
                break;
            case 1:
                offset = new Vector3(-missRange, 0, 0);
                break;
            case 2:
                offset = new Vector3(missRange, 0, 0);
                break;
        }
        return target.position + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBallInHands && !isBallFlying && other.CompareTag("Ball") && ballController.currentHolder == null)
        {
            ballController.ChangeHolder(gameObject);
            isBallInHands = true;
        }
    }

    private void TryStealBall()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= stealRange && ballController.currentHolder == player)
            {
                ballController.ChangeHolder(gameObject);
                isBallInHands = true;
                BasketGame playerScript = player.GetComponent<BasketGame>();
                playerScript.isBallInHands = false;
            }
        }
    }
}
