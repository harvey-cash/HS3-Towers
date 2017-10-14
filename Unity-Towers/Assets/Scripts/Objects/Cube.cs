using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Unit {
    public override UNIT_TYPES GetUnitType() {
        return UNIT_TYPES.CUBE;
    }
}
