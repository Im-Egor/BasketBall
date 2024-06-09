using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketGame : MonoBehaviour
{
    [SerializeField] float _speed = 10;
    [SerializeField] Transform _ball;
    [SerializeField] Transform _arms;
    [SerializeField] Transform _posOverhead;
    [SerializeField] Transform _posDribble;
    [SerializeField] Transform _target;
    [SerializeField] float _missRange = 0.5f;
    [SerializeField] Vector3 _minBounds;
    [SerializeField] Vector3 _maxBounds;
    [SerializeField] float _throwHeight = 5.0f;
    [SerializeField] float _stealRange = 0.5f; 

    private BallController _ballController;
    public bool _isBallInHands = false;
    private bool _isBallFlying = false;
    private float _t = 0;
    private Vector3 _finalTargetPosition;

    void Start()
    {
        _ballController = _ball.GetComponent<BallController>();
    }

    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("P1Vertical"), 0, -Input.GetAxisRaw("P1Horizontal"));
        Vector3 newPosition = transform.position + direction * _speed * Time.deltaTime;

        newPosition = ClampPosition(newPosition);
        transform.position = newPosition;

        transform.LookAt(transform.position + direction);

        if (_isBallInHands)
        {
            if (Input.GetKey(KeyCode.RightShift))
            {
                _ball.position = _posOverhead.position;
                _arms.localEulerAngles = Vector3.right * 180;
                transform.LookAt(_target.position);
            }
            else
            {
                _ball.position = _posDribble.position + Vector3.up / 2 * Mathf.Abs(Mathf.Sin(Time.time * 5));
                _arms.localEulerAngles = Vector3.right * 0;
            }

            if (Input.GetKeyUp(KeyCode.RightShift))
            {
                _isBallInHands = false;
                _isBallFlying = true;
                _t = 0;
                _finalTargetPosition = GetFinalTargetPosition();
                _ballController.ChangeHolder(null);
            }
        }

        if (_isBallFlying)
        {
            _t += Time.deltaTime;
            float duration = 1.0f;
            float t01 = _t / duration;

            Vector3 start = _posOverhead.position;
            Vector3 end = _finalTargetPosition;
            Vector3 mid = (start + end) / 2 + Vector3.up * _throwHeight;

            Vector3 pos = (1 - t01) * (1 - t01) * start + 2 * (1 - t01) * t01 * mid + t01 * t01 * end;

            _ball.position = ClampPosition(pos);

            if (t01 >= 1)
            {
                _isBallFlying = false;
                _ball.GetComponent<Rigidbody>().isKinematic = false;
                _ball.GetComponent<Rigidbody>().useGravity = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            TryStealBall();
        }
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, _minBounds.x, _maxBounds.x);
        position.y = Mathf.Clamp(position.y, _minBounds.y, _maxBounds.y);
        position.z = Mathf.Clamp(position.z, _minBounds.z, _maxBounds.z);
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
                offset = new Vector3(-_missRange, 0, 0);
                break;
            case 2:
                offset = new Vector3(_missRange, 0, 0);
                break;
        }
        return _target.position + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isBallInHands && !_isBallFlying && other.CompareTag("Ball") && _ballController.currentHolder == null)
        {
            _ballController.ChangeHolder(gameObject);
            _isBallInHands = true;
        }
    }

    private void TryStealBall()
    {
        GameObject opponent = GameObject.FindWithTag("Opponent");
        if (opponent != null)
        {
            float distance = Vector3.Distance(transform.position, opponent.transform.position);
            if (distance <= _stealRange && _ballController.currentHolder == opponent)
            {
                _ballController.ChangeHolder(gameObject);
                _isBallInHands = true;
                OpponentBasketGame opponentScript = opponent.GetComponent<OpponentBasketGame>();
                opponentScript._isBallInHands = false;
            }
        }
    }
    
}
