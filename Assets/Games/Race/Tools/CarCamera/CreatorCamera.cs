using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class CreatorCamera 
{
    public abstract ICameraOperation FactoryMethod(GameObject car);

    public void Initialize(GameObject car)
    {
        var camera = FactoryMethod(car);
        camera.Initialize(car);
    }
}
