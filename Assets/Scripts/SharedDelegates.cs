using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedDelegates : MonoBehaviour
{
  	public delegate void AttackUnit(BoardUnit attackedUnit);
    public delegate void AttackEventHandler(ValueTuple<int, int> coordinate, PlayerType playerType);
}
