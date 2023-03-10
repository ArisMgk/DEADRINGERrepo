using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcess : MonoBehaviour
{
    private static PostProcess instance;

    //skybox rotation variables
    private float speed = 0.55f;
    private float _skyboxrotation;
    private int _rotationproperty;
    private Material _skymat;

    public static PostProcess GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _rotationproperty = Shader.PropertyToID("_Rotation");
        _skymat = RenderSettings.skybox;
        _skyboxrotation = _skymat.GetFloat(_rotationproperty);
    }

    private void OnDisable()
    {
        _skymat.SetFloat(_rotationproperty, _skyboxrotation);
    }

    void Update()
    {
        _skymat.SetFloat(_rotationproperty, Time.time * speed);
    }
}
