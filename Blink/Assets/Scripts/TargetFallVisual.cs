using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFallVisual : MonoBehaviour
{
    [SerializeField] GameObject _targetObject = null;
    [SerializeField] Material _lineMaterial = null;

    GameObject _fallObject = null;
    RaycastHit _targetHit;
    Vector3 _fallTarget;
    bool _moveTarget = true;

    void Start()
    {
    }
    void FixedUpdate()
    {
        CastDown();
    }

    void OnDestroy()
    {
        Destroy(_fallObject);
    }

    void CastDown()
    {
        if (Physics.Raycast(transform.position, Vector3.down,out _targetHit, 200f))
        {
            _fallTarget = _targetHit.point;
        }
        StartCoroutine(UpdateTarget());
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = _lineMaterial;
        lr.startWidth = .1f;
        lr.endWidth = .1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    IEnumerator UpdateTarget()
    {
        if(_fallObject == null)
        {
            _fallObject = Instantiate(_targetObject, _fallTarget, Quaternion.identity);
        }
        else
        {
            while (_moveTarget)
            {
                _fallObject.transform.position = _fallTarget;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
