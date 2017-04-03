using System.Collections.Generic;
using Gamelogic;
using UnityCallbacks;
using UnityEngine;

namespace Memoria
{
    public class PlaneController : VisualizationController, IOnValidate, IOnDrawGizmos
    {
        public float rowDistanceDifference = 0.15f;
        public Vector3 scaleFactor = Vector3.one;
        public float distance = 1.0f;

        public bool autoAngleDistance = true;
        public float angleDistance = 10.0f;


        public static float delta = 1f;

        public override void OnValidate()
        {
            visualizationRow = Mathf.Clamp(visualizationRow, 1, 3);
            elementsToDisplay = Mathf.Max(0, elementsToDisplay);
            scaleFactor = new Vector3(
                Mathf.Max(0, scaleFactor.x),
                Mathf.Max(0, scaleFactor.y),
                Mathf.Max(0, scaleFactor.z)
                );
            distance = Mathf.Max(0.0f, distance);
            alpha = Mathf.Clamp(alpha, 0.0f, 1.0f);
            angleDistance = Mathf.Clamp(angleDistance, 0.0f, 180.0f);
        }

        public override void OnDrawGizmos()
        {
        }

        public override void CreateVisualization(bool createNewObjects, Vector3 planeCenter)
        {
            var center = planeCenter;
            var planeDistance = distance;

            for (var j = 0; j < elementsPerRow.Length; j++)
            {
                if (autoAngleDistance)
                    angleDistance = 180.0f / elementsPerRow[j];

                if (elementsPerRow.Length > 1)         //si la cantidad de elementos por columnas es mayor a 1
                {
                    if (j != 1)             //determinar si corresponde a la fila de arriba o abajo (j == 1, es la fila central)
                    {
                        var heightDiff = j == 0 ? Vector3.up : Vector3.down;        //vector3.up =  Vector3(0, 1, 0)        Vector3.down = Vector3(0, -1, 0)
                        heightDiff *= rowHightDistance;     //distancia de separación entre las filas
                        center = planeCenter + heightDiff;
                        planeDistance = distance;
                    }
                    else
                    {
                        center = planeCenter;              //centro de la esfera
                        planeDistance = distance;              //radio aumenta en proporcion al número de la esfera
                    }
                }
                delta = -0.5f;
                for (var i = 0; i < elementsPerRow[j]; i++)            //por cada elemento de la fila
                {
                    if (createNewObjects)
                    {
                        var grabableObject = Instantiate(dioManager.informationPlanePrefab, gameObject);     /* se instancia un DIOController en grabableObject */
                        grabableObject.transform.RotateAroundY(180);

                        grabableObject.pitchGrabObject.transform.localScale = scaleFactor;              /* pitchGrabObject es una clase que posee las funcionalidades para manejar los objetos con Leap y OpenGlove */

                        SetGrabableObjectPosition(grabableObject, center, planeDistance, i);           /* DIOController grabableObject      Vector3 Center     float radius    int index */
                        SetGrabableObjectConfiguration(grabableObject, i);

                        var grabableObjectMeshRender = grabableObject.pitchGrabObject.GetComponent<MeshRenderer>();        /* obtiene el componente mesh render */
                        var grabableObjectColor = grabableObjectMeshRender.material.color;
                        grabableObjectColor.a = alpha;            /* color de los bordes */
                        grabableObjectMeshRender.material.color = grabableObjectColor;

                        dioControllerList.Add(grabableObject);
                    }
                    else
                    {
                        SetGrabableObjectPosition(dioControllerList[i], center, distance, i);
                    }
                    delta += 4f;
                }
            }
        }


        /* esta funcion se utiliza para navegar entre las esferas */
        public override void ChangeVisualizationConfiguration(Vector3 planeCenter, float newDistance, float newAlpha)
        {
            distance = newDistance;
            alpha = newAlpha;

            var center = planeCenter;
            var planeDistance = distance;
            var globalElementIndex = 0;

            for (var j = 0; j < elementsPerRow.Length; j++)
            {
                if (elementsPerRow.Length > 1)
                {
                    if (j != 1)
                    {
                        var heightDiff = j == 0 ? Vector3.up : Vector3.down;
                        heightDiff *= rowHightDistance;
                        center = planeCenter + heightDiff;

                        planeDistance -= rowDistanceDifference;
                    }
                    else
                    {
                        center = planeCenter;
                        planeDistance = distance;
                    }
                }

                if (planeDistance <= 0.0f)
                    planeDistance = 0.0f;
                for (var i = 0; i < elementsPerRow[j]; i++)
                {
                    var grabableObject = dioControllerList[globalElementIndex];
                    var grabableObjectMeshRender = grabableObject.pitchGrabObject.GetComponent<MeshRenderer>();
                    var grabableObjectColor = grabableObjectMeshRender.material.color;
                    grabableObjectColor.a = alpha;
                    grabableObjectMeshRender.material.color = grabableObjectColor;

                    SetGrabableObjectPosition(grabableObject, center, planeDistance, i);
                    globalElementIndex++;
                }
            }
        }

        public override void SetGrabableObjectPosition(DIOController grabableObject, Vector3 planeCenter, float distance, int index)
        {
            var angle = index * angleDistance;          /* i * distancia entre los angulos calculada en CreateVisualization */

            var position = RandomPlane(planeCenter, distance, index);

            grabableObject.transform.position = position;       /* le asigna la posición calculada al objeto */
        }

        public override void SetGrabableObjectConfiguration(DIOController grabableObject, int id)
        {

            if (dioManager.usePitchGrab)
            {
                grabableObject.pitchGrabObject.pinchDetectorLeft = dioManager.pinchDetectorLeft;
                grabableObject.pitchGrabObject.pinchDetectorRight = dioManager.pinchDetectorRight;
            }

            grabableObject.Initialize(this, id);

        }

        /*funcion que calcula la posición en donde se posicionará la imagen */
        private Vector3 RandomPlane(Vector3 center, float distance, float index)
        {
            return new Vector3
            {
                x = center.x + distance * index - 0.685f,
                y = center.y,
                z = center.z + distance
            };

        }

    }
}

