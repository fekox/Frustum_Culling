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
        //*Plano cercano*
        Vector3 nearPlanePos = camera.transform.position; //Guardo la posicion de la camara
        nearPlanePos += camera.transform.forward * camera.nearClipPlane; //Actualizo el plano cercano de la camara.
        plane[0].SetNormalAndPosition(camera.transform.forward, nearPlanePos); //Seteo el plano cercano del frustrum.

        //*Plano cercano*
        Vector3 farPlanePos = camera.transform.position; //Guardo la posicion de la camara
        farPlanePos += camera.transform.forward * camera.farClipPlane; //Actualizo el plano lejano de la camara.
        plane[1].SetNormalAndPosition(camera.transform.forward * -1, farPlanePos); //Seteo el plano lejano del frustrum.

        //*Plano Izquierdo*
        plane[2].Set3Points(camera.transform.position, farBottomLeft, farTopLeft); //Seteo el plano Izquierdo del frustrum.

        //*Plano Derecho*
        plane[3].Set3Points(camera.transform.position, farTopRight, farBottomRight); //Seteo el Derecho lejano del frustrum.

        //*Plano de arriba*
        plane[4].Set3Points(camera.transform.position, farTopLeft, farTopRight); //Seteo el plano de arriba del frustrum.

        //*Plano de abajo*
        plane[5].Set3Points(camera.transform.position, farBottomRight, farBottomLeft); //Seteo el plano de abajo del frustrum.

        for (int i = 2; i < maxPlanes; i++)
        {
            plane[i].Flip();//Hace que el plano mire en la direccion opuesta.
        }
    }
}
