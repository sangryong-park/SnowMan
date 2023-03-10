using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] float _heightOffset = 1;
    Rigidbody _rb = null;
    int _currentNumber = 0;
    public int CurrentNum {get => _currentNumber;}
    bool _isRolling = false;
    DiceManager.DiceGrade _grade = default;
    public DiceManager.DiceGrade Grade{get => _grade; set => _grade = value;}
    MeshRenderer _meshRenderer;
    public MeshRenderer MeshRender {get => _meshRenderer;}
    Action<int, Dice> _doSetNumber;
    public event Action<int, Dice> DoSetNumber
    {
        add => _doSetNumber += value;
        remove => _doSetNumber -= value;
    }

    readonly float _velocityOffset = 0.55f;
    readonly int _diceSide = 6;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isRolling) return;
        //Debug.Log(_initVelY);
        if ((_rb.velocity.y  < 0 && _rb.position.y <= _velocityOffset) || _rb.IsSleeping())
        {
            var num = GetTopNumber();
            _currentNumber = num;
            Debug.Log($"{_meshRenderer.material},{num}");
            _doSetNumber?.Invoke(num, this);
            _isRolling = false;
        }
    }

    int GetTopNumber()
    {
        float minDot = float.MaxValue;
        int num = 0;
        for (int i = 0; i < _diceSide; i++)
        {
            Vector3 faceNormal = transform.TransformDirection(GetFaceNormal(i));
            float dot = Vector3.Dot(Vector3.down, faceNormal);
            if (dot < minDot) {
                minDot = dot;
                num = i + 1;
            }
        }
        return num;
    }

    Vector3 GetFaceNormal(int idx)
    {
        switch (idx) {
            case 0: return Vector3.up;
            case 1: return Vector3.forward;
            case 2: return Vector3.right;
            case 3: return Vector3.left;
            case 4: return Vector3.back;
            case 5: return Vector3.down;
            default: return Vector3.zero;
        }
    }

    public void DoRoll(DiceManager.RollInfo rollInfo)
    {
        if (_rb == null) return;
        _isRolling = true;
        if (_rb.position.y > _heightOffset) return;
        //_rb.AddForce(UnityEngine.Random.onUnitSphere * 3.0f, ForceMode.Impulse);
        _rb.AddTorque(rollInfo._randomTorque, ForceMode.Impulse);
        _rb.AddExplosionForce(rollInfo._randomForce, rollInfo._pos, rollInfo._explosionRadius, rollInfo._upForce);
    }

    public void Init(Material mat)
    {
        _isRolling = false;
        _currentNumber = 0;
        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer != null)
            _meshRenderer.material = mat;
    }
}
