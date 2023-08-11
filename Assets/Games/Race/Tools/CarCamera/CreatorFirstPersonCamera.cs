using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CreatorFirstPersonCamera: CreatorCamera
{
        public override ICameraOperation FactoryMethod(GameObject car)
        {
            FirstPersonCamera camera = car.AddComponent<FirstPersonCamera>();
            return camera;
        }
}

