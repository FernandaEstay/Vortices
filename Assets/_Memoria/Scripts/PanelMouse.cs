using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Memoria
{
    public class PanelMouse :  PanelController
    {
        public Text txtTime;
        private float time;
        private string text;
        float min;
        float seg;

        public override void Initialize(DIOManager dioManager)
        {
            base.dioManager = dioManager;
            EnableMoveCameraInside();
            EnableMoveCameraOutside();
        }
        public override void Inside()
        {
            Debug.Log("ocurre evento");
            dioManager.MovePlaneInside(1, dioManager.initialPlaneAction, dioManager.finalPlaneAction);
        }

        public override void Outside()
        {
            dioManager.MovePlaneOutside(1, dioManager.initialPlaneAction, dioManager.finalPlaneAction);
        }
        public void Update()
        {
            time += Time.deltaTime;
            min = Mathf.Floor(time / 60);
            seg = time % 60;
            if (min.ToString().Length == 1)
                text = "Tiempo: 0" + min.ToString();
            else
                text = "Tiempo: " + min.ToString();

            if (Mathf.RoundToInt(seg).ToString().Length == 1)
                text = text + ":0" + Mathf.RoundToInt(seg).ToString();
            else
                text = text + ":" + Mathf.RoundToInt(seg).ToString();
            txtTime.text = text;
        }
    }
}