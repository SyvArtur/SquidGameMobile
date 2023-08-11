using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CreatorThirdPersonCamera : CreatorCamera
{
    public override ICameraOperation FactoryMethod(GameObject car)
    {
        ThirdPersonCamera camera = car.AddComponent<ThirdPersonCamera>();
        return camera;
    }
}

