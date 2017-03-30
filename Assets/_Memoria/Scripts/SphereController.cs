using System.Collections.Generic;
using Gamelogic;
using UnityCallbacks;
using UnityEngine;

namespace Memoria
{
    public class SphereController : VisualizationBehaviour, IOnValidate, IOnDrawGizmos
    {

        public int sphereRows = 1;

        public float rowRadiusDifference = 0.15f;
        public Vector3 scaleFactor = Vector3.one;
        public float sphereRadius = 1.0f;
        public float sphereAlpha = 1.0f;
        public bool autoAngleDistance = true;
        public float angleDistance = 10.0f;

        public bool notInZero;
        public int sphereId;

        public override void OnValidate()
        {
            sphereRows = Mathf.Clamp(sphereRows, 1, 3);
            elementsToDisplay = Mathf.Max(0, elementsToDisplay);
            scaleFactor = new Vector3(
                Mathf.Max(0, scaleFactor.x),
                Mathf.Max(0, scaleFactor.y),
                Mathf.Max(0, scaleFactor.z)
                );
            sphereRadius = Mathf.Max(0.0f, sphereRadius);
            sphereAlpha = Mathf.Clamp(sphereAlpha, 0.0f, 1.0f);
            angleDistance = Mathf.Clamp(angleDistance, 0.0f, 360.0f);
        }

        public override void OnDrawGizmos()
        {
            if (!showDebugGizmo)
                return;

            var previousGizmoColor = Gizmos.color;
            Gizmos.color = sphereDebugColor;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
            Gizmos.color = previousGizmoColor;
        }

        public override void InitializeDioControllers(DIOManager fatherDioManager, int assignedSphereId, Vector3 sphereCenter, int textureIndex, bool createNewObjects = false)
        {
            dioManager = fatherDioManager;
            sphereId = assignedSphereId;
            notInZero = true;

            if (createNewObjects)
                dioControllerList = new List<DIOController>();

            _elementsPerRow = new int[sphereRows];

            var extraItems = elementsToDisplay % sphereRows;    //elementos extras que sobran en las esferas   
            var rowElements = elementsToDisplay / sphereRows;   //cantidad de esferas

            for (int i = 0; i < _elementsPerRow.Length; i++)
            {
                _elementsPerRow[i] = rowElements;           //cantidad de elementos que se crearan por fila

                if (i == 1)
                    _elementsPerRow[i] += extraItems;
            }
            /* CreateNewbjects = true   sphereCenter = centro de la esfera, viene desde la función start de dio manager */
            CreateVisualization(createNewObjects, sphereCenter);        
        }

        public override void CreateVisualization(bool createNewObjects, Vector3 sphereCenter)
        {
            var center = sphereCenter;
            var radius = sphereRadius;          //viene desde DiOManager IdealSphereConfiguration()

            for (var j = 0; j < _elementsPerRow.Length; j++)
            {
                if (autoAngleDistance)
                    angleDistance = 360.0f / _elementsPerRow[j];        //distancia entre los elementos

                if (_elementsPerRow.Length > 1)         //si la cantidad de elementos por columnas es mayor a 1
                {
                    if (j != 1)             //determinar si corresponde a la fila de arriba o abajo (j == 1, es la fila central)
                    {
                        var heightDiff = j == 0 ? Vector3.up : Vector3.down;        //vector3.up =  Vector3(0, 1, 0)        Vector3.down = Vector3(0, -1, 0)
                        heightDiff *= rowHightDistance;     //distancia de separación entre las filas
                        center = sphereCenter + heightDiff;         

                        radius -= rowRadiusDifference;          //Para darle la forma esferica el radio de los bordes (up o down) se reduce
                    }
                    else
                    {
                        center = sphereCenter;              //centro de la esfera
                        radius = sphereRadius;              //radio aumenta en proporcion al número de la esfera
                    }
                }

                for (var i = 0; i < _elementsPerRow[j]; i++)            //por cada elemento de la fila
                {
                    if (createNewObjects)          
                    {
                        var grabableObject = Instantiate(dioManager.informationPrefab, gameObject);     /* se instancia un DIOController en grabableObject */
                        grabableObject.pitchGrabObject.transform.localScale = scaleFactor;              /* pitchGrabObject es una clase que posee las funcionalidades para manejar los objetos con Leap y OpenGlove */
                    
                        SetGrabableObjectPosition(grabableObject, center, radius, i);           /* DIOController grabableObject      Vector3 Center     float radius    int index */
                        SetGrabableObjectConfiguration(grabableObject, i);

                        var grabableObjectMeshRender = grabableObject.pitchGrabObject.GetComponent<MeshRenderer>();        /* obtiene el componente mesh render */
                        var grabableObjectColor = grabableObjectMeshRender.material.color;
                        grabableObjectColor.a = sphereAlpha;            /* color de los bordes */
                        grabableObjectMeshRender.material.color = grabableObjectColor;        

                        dioControllerList.Add(grabableObject);
                    }
                    else
                    {
                        SetGrabableObjectPosition(dioControllerList[i], center, radius, i);
                    }
                }
            }
        }


        /* esta funcion se utiliza para navegar entre las esferas */
        public override void ChangeVisualizationConfiguration(Vector3 sphereCenter, float newRadius, float newAlpha)
        {
            sphereRadius = newRadius;
            sphereAlpha = newAlpha;

            var center = sphereCenter;
            var radius = sphereRadius;
            var globalElementIndex = 0;

            for (var j = 0; j < _elementsPerRow.Length; j++)
            {
                if (_elementsPerRow.Length > 1)
                {
                    if (j != 1)
                    {
                        var heightDiff = j == 0 ? Vector3.up : Vector3.down;
                        heightDiff *= rowHightDistance;
                        center = sphereCenter + heightDiff;

                        radius -= rowRadiusDifference;
                    }
                    else
                    {
                        center = sphereCenter;
                        radius = sphereRadius;
                    }
                }

                if (radius <= 0.0f)
                    radius = 0.0f;

                for (var i = 0; i < _elementsPerRow[j]; i++)
                {
                    var grabableObject = dioControllerList[globalElementIndex];
                    var grabableObjectMeshRender = grabableObject.pitchGrabObject.GetComponent<MeshRenderer>();
                    var grabableObjectColor = grabableObjectMeshRender.material.color;
                    grabableObjectColor.a = sphereAlpha;
                    grabableObjectMeshRender.material.color = grabableObjectColor;

                    SetGrabableObjectPosition(grabableObject, center, radius, i);
                    globalElementIndex++;
                }
            }
        }

        public override void SetGrabableObjectPosition(DIOController grabableObject, Vector3 sphereCenter, float radius, int index)
        {
            var angle = index * angleDistance;          /* i * distancia entre los angulos calculada en CreateVisualization */
            var position = RandomCircle(sphereCenter, radius, angle);

            grabableObject.transform.position = position;       /* le asigna la posición calculada al objeto */
            grabableObject.transform.LookAt(transform);     /*  Gira la transformación de manera que el vector hacia adelante señale la posición actual del objeto -> NO SE QUE HACE EXACTAMENTE pero si lo comento las imagenes se vuelven planas  y volteadas */
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
        private Vector3 RandomCircle(Vector3 center, float radius, float angle)
        {
            return new Vector3
            {
                x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad),       
                y = center.y,
                z = center.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad)
            };
        }

    }

    public abstract class VisualizationBehaviour : GLMonoBehaviour
    {
        public int elementsToDisplay = 1;
        public float rowHightDistance = 0.4f;
        public bool showDebugGizmo = true;
        public Color sphereDebugColor = Color.red;

        [HideInInspector]
        public List<DIOController> dioControllerList;
        [HideInInspector]
        public DIOManager dioManager;

        public int[] _elementsPerRow;

        public abstract void OnValidate();
        public abstract void OnDrawGizmos();

        public abstract void InitializeDioControllers(DIOManager fatherDioManager, int assignedVisualizationId, Vector3 visualizationCenter, int textureIndex, bool createNewObjects = false);
        public abstract void CreateVisualization(bool createNewObjects, Vector3 VisualizationCenter);
        public abstract void ChangeVisualizationConfiguration(Vector3 visualizationCenter, float espacing, float newAlpha);

        public abstract void SetGrabableObjectPosition(DIOController grabableObject, Vector3 visualizationCenter, float spacing, int index);
        public abstract void SetGrabableObjectConfiguration(DIOController grabableObject, int id);

    }
}