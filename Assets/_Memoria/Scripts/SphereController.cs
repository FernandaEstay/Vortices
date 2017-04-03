using System.Collections.Generic;
using Gamelogic;
using UnityCallbacks;
using UnityEngine;

namespace Memoria
{
    public class SphereController : VisualizationController, IOnValidate, IOnDrawGizmos
    {

        public float rowRadiusDifference = 0.15f;
        public Vector3 scaleFactor = Vector3.one;
        public float sphereRadius = 1.0f;

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
            sphereRadius = Mathf.Max(0.0f, sphereRadius);
            alpha = Mathf.Clamp(alpha, 0.0f, 1.0f);
            angleDistance = Mathf.Clamp(angleDistance, 0.0f, 360.0f);
        }

        public override void OnDrawGizmos()
        {
            if (!debugGizmo)
                return;

            var previousGizmoColor = Gizmos.color;
            Gizmos.color = debugColor;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
            Gizmos.color = previousGizmoColor;
        }



        public override void CreateVisualization(bool createNewObjects, Vector3 sphereCenter)
        {
            var center = sphereCenter;
            var radius = sphereRadius;

            for (var j = 0; j < elementsPerRow.Length; j++)
            {
                if (autoAngleDistance)
                    angleDistance = 360.0f / elementsPerRow[j];

                if (elementsPerRow.Length > 1)
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

                for (var i = 0; i < elementsPerRow[j]; i++)
                {
                    if (createNewObjects)
                    {
                        var grabableObject = Instantiate(dioManager.informationPrefab, gameObject);
                        grabableObject.pitchGrabObject.transform.localScale = scaleFactor;

                        SetGrabableObjectPosition(grabableObject, center, radius, i);
                        SetGrabableObjectConfiguration(grabableObject, i);

                        var grabableObjectMeshRender = grabableObject.pitchGrabObject.GetComponent<MeshRenderer>();
                        var grabableObjectColor = grabableObjectMeshRender.material.color;
                        grabableObjectColor.a = alpha;
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
            alpha = newAlpha;

            var center = sphereCenter;
            var radius = sphereRadius;
            var globalElementIndex = 0;

            for (var j = 0; j < elementsPerRow.Length; j++)
            {
                if (elementsPerRow.Length > 1)
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
                for (var i = 0; i < elementsPerRow[j]; i++)
                {
                    var grabableObject = dioControllerList[globalElementIndex];
                    var grabableObjectMeshRender = grabableObject.pitchGrabObject.GetComponent<MeshRenderer>();
                    var grabableObjectColor = grabableObjectMeshRender.material.color;
                    grabableObjectColor.a = alpha;
                    grabableObjectMeshRender.material.color = grabableObjectColor;

                    SetGrabableObjectPosition(grabableObject, center, radius, i);
                    globalElementIndex++;
                }
            }
        }

        public override void SetGrabableObjectPosition(DIOController grabableObject, Vector3 sphereCenter, float radius, int index)
        {
            var angle = index * angleDistance;
            var position = RandomCircle(sphereCenter, radius, angle);

            grabableObject.transform.position = position;
            grabableObject.transform.LookAt(transform);
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


}