using System.Collections.Generic;
using Gamelogic;
using UnityCallbacks;
using UnityEngine;

namespace Memoria
{
    public abstract class VisualizationController : GLMonoBehaviour
    {
        public int elementsToDisplay = 1;
        public int visualizationRow = 1;
        public float rowHightDistance = 0.4f;
        public bool debugGizmo = true;
        public Color debugColor = Color.red;

        public bool notInZero;
        public int id;
        public float alpha = 1.0f;

        [HideInInspector]
        public List<DIOController> dioControllerList;
        [HideInInspector]
        public DIOManager dioManager;

        public int[] elementsPerRow;

        public abstract void OnValidate();
        public abstract void OnDrawGizmos();

        public virtual void InitializeDioControllers(DIOManager fatherDioManager, int Id, Vector3 center, int textureIndex, bool createNewObjects = false)
        {
            dioManager = fatherDioManager;
            id = Id;
            notInZero = true;

            if (createNewObjects)
                dioControllerList = new List<DIOController>();

            elementsPerRow = new int[visualizationRow];

            var extraItems = elementsToDisplay % visualizationRow;    //elementos extras que sobran en las esferas   
            var rowElements = elementsToDisplay / visualizationRow;   //cantidad de esferas

            for (int i = 0; i < elementsPerRow.Length; i++)
            {
                elementsPerRow[i] = rowElements;           //cantidad de elementos que se crearan por fila

                if (i == 1)
                    elementsPerRow[i] += extraItems;
            }
            CreateVisualization(createNewObjects, center);
        }

        public abstract void CreateVisualization(bool createNewObjects, Vector3 VisualizationCenter);
        public abstract void ChangeVisualizationConfiguration(Vector3 visualizationCenter, float espacing, float newAlpha);

        public abstract void SetGrabableObjectPosition(DIOController grabableObject, Vector3 visualizationCenter, float spacing, int index);
        public abstract void SetGrabableObjectConfiguration(DIOController grabableObject, int id);

    }
}