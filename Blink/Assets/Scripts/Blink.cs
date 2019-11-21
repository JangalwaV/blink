using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]

public class Blink : MonoBehaviour
{
    InputManager _input = null;
    Rigidbody _rb = null;

    //[SerializeField] float _blinkStrength = 100f;
    [SerializeField] float _blinkDistance = 10f;
    [SerializeField] float _blinkSpeed = 1f;
    [SerializeField] GameObject _blinkTarget = null;

    RaycastHit _blinkHit;
    GameObject blinkTargetInstance = null;
    bool _move = false;
    Vector3 _blinkLocation;
    float _startTime;
    float _blinkLength;
    Vector3 _blinkStartPos;

    void Awake() {
        _input = GetComponent<InputManager>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _input.BlinkChannel += Target;
        _input.BlinkRelease += Release;
    }

    private void OnDisable()
    {
        _input.BlinkChannel -= Target;
        _input.BlinkRelease -= Release;
    }

    private void FixedUpdate()
    {
        /*
        if (_move)
        {
            Debug.Log("...Moving...");
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - _startTime) * _blinkSpeed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / _blinkLength;

            // Set our position as a fraction of the distance between the markers.
            Vector3 newPosition = Vector3.Lerp(_blinkStartPos, _blinkLocation, fractionOfJourney);
            _rb.MovePosition(newPosition);

            transform.position = Vector3.Lerp(_blinkStartPos, _blinkLocation, fractionOfJourney);


            if (fractionOfJourney == 1)
            {
                _move = false;
            }
        }
        */
    }

    void Target()
    {
        Debug.Log("Target");
        Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out _blinkHit, _blinkDistance);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _blinkDistance,Color.cyan);
        blinkTargetInstance = Instantiate(_blinkTarget, _blinkHit.point,Quaternion.identity);
    }
    void Release()
    {
        Debug.Log("Release");
        _blinkLocation = _blinkHit.point;
        _blinkStartPos = _rb.position;
        _startTime = Time.time;

        _blinkLength = Vector3.Distance(transform.position, _blinkLocation);
        
        _move = true;
        Destroy(blinkTargetInstance);

        StartCoroutine(BlinkMovement());
    }

    IEnumerator BlinkMovement()
    {
        Vector3 startPos = _rb.position;
        Vector3 endPos = _blinkLocation;
        float blinkDurationSeconds = .3f;
        // blink player process
        for (float t = 0; t < blinkDurationSeconds; t += Time.deltaTime)
        {
            Vector3 newPosition = Vector3.Lerp(startPos, endPos, t / blinkDurationSeconds);
            _rb.MovePosition(newPosition);
            yield return new WaitForFixedUpdate();
        }
    }
}
