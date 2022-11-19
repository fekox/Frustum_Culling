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
    [SerializeField] Vector3 nearDownLeft;
    [SerializeField] Vector3 nearDownRight;

    //Plano lejano de la camara.
    [SerializeField] Vector3 farTopLeft;
    [SerializeField] Vector3 farTopRight;
    [SerializeField] Vector3 farDownLeft;
    [SerializeField] Vector3 farDownRight;

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

        SetFarPoints(farPlanePos);//Lamo a la funcion para setear los puntos del plano lejano del Frustrum.
        SetNearPoints(nearPlanePos);//Lamo a la funcion para setear los puntos del plano cercano del Frustrum.

        //*Plano izquierdo*
        plane[2].Set3Points(camera.transform.position, farDownLeft, farTopLeft); //Seteo el plano Izquierdo del frustrum.

        //*Plano derecho*
        plane[3].Set3Points(camera.transform.position, farTopRight, farDownRight); //Seteo el Derecho lejano del frustrum.

        //*Plano de arriba*
        plane[4].Set3Points(camera.transform.position, farTopLeft, farTopRight); //Seteo el plano de arriba del frustrum.

        //*Plano de abajo*
        plane[5].Set3Points(camera.transform.position, farDownRight, farDownLeft); //Seteo el plano de abajo del frustrum.

        for (int i = 2; i < maxPlanes; i++)
        {
            plane[i].Flip();//Hace que el plano mire en la direccion opuesta.
        }
    }

    //Funcion para setear los puntos del plano lejano
    public void SetFarPoints(Vector3 farPos)
    {
        Vector3 farPlaneDist = camera.transform.position + (camera.transform.forward * camera.farClipPlane); //Guardo la distancia del plano lejano de la camara.

        farTopLeft = farPlaneDist + (camera.transform.up) - (camera.transform.right); //Seteo el punto de arriba a la izquierda.

        farTopRight = farPlaneDist + (camera.transform.up) + (camera.transform.right); //Seteo el punto de arriba a la derecha.

        farDownLeft = farPlaneDist - (camera.transform.up) - (camera.transform.right); //Seteo el punto de abajo a la izquierda.

        farDownRight = farPlaneDist - (camera.transform.up) + (camera.transform.right); //Seteo el punto de abajo a la derecha.
    }

    //Funcion para setear los puntos del plano cercano
    public void SetNearPoints(Vector3 nearPos)
    {
        Vector3 nearPlaneDist = camera.transform.position + (camera.transform.forward * camera.nearClipPlane); //Guardo la distancia del plano cercano de la camara.

        nearTopLeft = nearPlaneDist + (camera.transform.up) - (camera.transform.right); //Seteo el punto de arriba a la izquierda.

        nearTopRight = nearPlaneDist + (camera.transform.up) + (camera.transform.right); //Seteo el punto de arriba a la derecha.

        nearDownLeft = nearPlaneDist - (camera.transform.up) - (camera.transform.right); //Seteo el punto de abajo a la izquierda.

        nearDownRight = nearPlaneDist - (camera.transform.up) + (camera.transform.right); //Seteo el punto de abajo a la derecha.
    }
}
