using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirefighterFighter.Camera
{
    public class CameraAutoSet : MonoBehaviour
    {
        public void Setup(Transform player)
        {
            CinemachineFreeLook camera = GetComponent<CinemachineFreeLook>();

            camera.Follow = player.transform;
            camera.LookAt = player.transform;
        }
    }
}