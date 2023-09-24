using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class CreatorCamera 
{
    public abstract ICameraOperation FactoryMethod(GameObject car);
}
