using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FrustumCulling : MonoBehaviour
{
    //Inicializa las constantes
    private const int maxPlanes = 6; //Cantidad de planos del frstrum.
    
    //Inicializo un array de planos con la cantidad de planos del frustrum.
    Plane[] plane = new Plane[maxPlanes];

    //Inicializo una camara.
    Camera camera;

    //Plano cercano de la camara.
    [SerializeField] Vector3 nearTopLeft;
    [SerializeField] Vector3 nearTopRight;
    [SerializeField] Vector3 nearBottomLeft;
    [SerializeField] Vector3 nearBottomRight;

    //Plano lejano de la camara.
    [SerializeField] Vector3 farTopLeft;
    [SerializeField] Vector3 farTopRight;
    [SerializeField] Vector3 farBottomLeft;
    [SerializeField] Vector3 farBottomRight;

    private void Awake() //Se llama automaticamente al iniciar el scrip.
    {
        //Hago que la camara sea igual a la main camara.
        camera = Camera.main;
    }

    void Start() //Se llama automaticamente despues del awake.
    {
        //Creo los 6 planos del frutrum.
        for (int i = 0; i < maxPlanes; i++)
        {
            plane[i] = new Plane();
        }
    }

    private void FixedUpdate() //Se llama un vez por frame.
    {
        UpdateFrustrumPlanes();//Llamo a la funcion que updatea los planos del frustrum.
    }

    void UpdateFrustrumPlanes()
    {
        Vector3 frontMultFar = camera.farClipPlane * camera.transform.forward;

        //*Plano cercano*
        Vector3 nearPlanePos = camera.transform.position; //Guardo la posicion de la camara
        nearPlanePos += camera.transform.forward * camera.nearClipPlane; //Actualizo el plano cercano de la camara.
        plane[0].SetNormalAndPosition(camera.transform.forward, nearPlanePos); //Seteo el plano cercano del frustrum.

        //*Plano cercano*
        Vector3 farPlanePos = camera.transform.position; //Guardo la posicion de la camara
        farPlanePos += camera.transform.forward * camera.farClipPlane; //Actualizo el plano lejano de la camara.
        plane[1].SetNormalAndPosition(camera.transform.forward * -1, farPlanePos); //Seteo el plano lejano del frustrum.
    }
}
