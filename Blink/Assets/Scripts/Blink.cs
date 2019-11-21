using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(AudioSource))]
public class Blink : MonoBehaviour
{
    InputManager _input = null;
    Rigidbody _rb = null;
    Camera _camera = null;
    AudioSource _audio = null;

    [Header("Mechanical Settings")]
    [SerializeField] float _blinkRange = 10f;
    [SerializeField] float _blinkDuration = .3f;

    [Header("Feedback Settings")]
    [SerializeField] AudioClip[] _blinkAudio = new AudioClip[4];
    [SerializeField] GameObject _blinkTarget = null;
    [SerializeField] float _blinkFOV = 80f;

    RaycastHit _blinkHit;
    GameObject _targetObject = null;
    Vector3 _targetLocation;
    float _baseCameraFOV;

    //feedback timers
    private float _fovTimer = 0;
    private float _fovTimerDuration;
    bool _fovChange = false;

    void Awake() {
        _input = GetComponent<InputManager>();
        _rb = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
        _audio = GetComponent<AudioSource>();
        _fovTimerDuration = _blinkDuration;
        _baseCameraFOV = _camera.fieldOfView;
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

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _audio.PlayOneShot(_blinkAudio[Random.Range(0, 1)]);
        }
    }

    void Target()
    {
        if (Physics.Raycast(transform.position, _camera.transform.forward, out _blinkHit, _blinkRange)){ // if a collider is hit
            Debug.Log("Hit");
            _targetLocation = _blinkHit.point;
        }
        else //if no collider is hit (blink into world space)
        {
            Debug.Log("Miss");
            //calculate miss
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, _camera.nearClipPlane);
            Vector3 screenCenterToWorld = _camera.ScreenToWorldPoint(screenCenter);
            _targetLocation = screenCenterToWorld + _camera.transform.forward * _blinkRange;
        }

        StartCoroutine(DisplayTarget());
    }
    void Release()
    {
        _audio.PlayOneShot(_blinkAudio[Random.Range(2,3)]);
        Destroy(_targetObject);
        Feedback();
        StartCoroutine(BlinkMovement());    //Blink Coroutine, so it is uninterrupted
    }

    void Feedback()
    {
        //FOV Change
        _fovChange = true;
        _fovTimer = _fovTimerDuration;
        StartCoroutine(FOVFeedback());
        Debug.Log("Started FOV Coroutine");
        //TODO Blur
        //TODO Camera Shake
    }

    IEnumerator BlinkMovement()
    {
        Vector3 startPos = _rb.position;    //find start position (player location)
        Vector3 endPos = _targetLocation;    //find target location (blink location)
        // blink player process
        for (float t = 0; t < _blinkDuration; t += Time.deltaTime)  //while blink is less than total blink duration, increasing by time since last frame
        {
            Vector3 newPosition = Vector3.Lerp(startPos, endPos, t / _blinkDuration);   //Find how much time has elapsed in the blink (0-1)
            _rb.MovePosition(newPosition);                                              //Move player position slightly towards blink
            yield return new WaitForFixedUpdate();  //leave coroutine, wait for FixedUpdate to catch up - This allows for animation and "movement"
        }
    }

    IEnumerator DisplayTarget()
    {
        if (_targetObject != null)
        {
            _targetObject.transform.position = _targetLocation;
            yield return new WaitForFixedUpdate();
        }
        else if (_targetObject == null)
        {
            _targetObject = Instantiate(_blinkTarget, _targetLocation, Quaternion.identity);
        }
    }

    IEnumerator FOVFeedback()
    {
        Debug.Log("Entered Coroutine");
        while(_camera.fieldOfView < _blinkFOV) { 
            _camera.fieldOfView += 5;
            Debug.Log("Increasing FOV...");
            yield return new WaitForFixedUpdate();
        }
        while(_camera.fieldOfView > _baseCameraFOV)
        {
            _camera.fieldOfView -= 2;
            yield return new WaitForFixedUpdate();
        }
        _camera.fieldOfView = _baseCameraFOV;
    }
}
