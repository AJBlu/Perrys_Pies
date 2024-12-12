using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetBoolOn(string boolName)
    {
        if (!_animator)
            return;
        _animator.SetBool(boolName, true);

    }

    public void SetBoolOff(string boolName)
    {
        if (!_animator)
            return;
        _animator.SetBool(boolName, false);
    }

}
