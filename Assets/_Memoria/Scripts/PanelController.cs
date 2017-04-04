using System;
using Gamelogic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Memoria
{
    public abstract class PanelController : GLMonoBehaviour
    {

        [Header("Inside")]
        public Button moveCameraInside3DButton;
        public EventTrigger moveCameraInsideEventTrigger;

        [Header("Outside")]
        public Button moveCameraOutside3DButton;
        public EventTrigger moveCameraOutsideEventTrigger;

        public DIOManager dioManager;




        #region Enable Disable


        public void DisableMoveCameraInside()
        {
            DisableButton(moveCameraInside3DButton, moveCameraInsideEventTrigger);
        }

        public void DisableMoveCameraOutside()
        {
            DisableButton(moveCameraOutside3DButton, moveCameraOutsideEventTrigger);
        }

        public void EnableMoveCameraInside()
        {
            if (!dioManager.InLastVisualization)
                EnableButton(moveCameraInside3DButton, moveCameraInsideEventTrigger);
            else
                DisableButton(moveCameraInside3DButton, moveCameraInsideEventTrigger);
        }

        public void EnableMoveCameraOutside()
        {
            if (!dioManager.InFirstVisualization)
                EnableButton(moveCameraOutside3DButton, moveCameraOutsideEventTrigger);
            else
                DisableButton(moveCameraOutside3DButton, moveCameraOutsideEventTrigger);
        }
        public void EnableButton(Button button, EventTrigger eventTrigger)
        {
            button.interactable = true;
            eventTrigger.enabled = true;
        }

        public void DisableButton(Button button, EventTrigger eventTrigger)
        {
            button.interactable = false;
            eventTrigger.enabled = false;
        }
        #endregion

        public abstract void Initialize(DIOManager dioManager);
        public abstract void Inside();
        public abstract void Outside();
    }
}
