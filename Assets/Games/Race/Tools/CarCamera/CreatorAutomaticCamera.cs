using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CreatorAutomaticCamera : CreatorCamera
{
    public override ICameraOperation FactoryMethod(GameObject car)
    {
        AutomaticCamera camera = car.AddComponent<AutomaticCamera>();
        return camera;
    }
}

