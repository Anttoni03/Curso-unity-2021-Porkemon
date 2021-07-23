using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private MoveBasic _base;
    private int _pp;

    public MoveBasic Base
    {
        get => _base;
        set => _base = value;
    }

    public int PP
    {
        get => _pp;
        set => _pp = value;
    }

    public Move(MoveBasic mBase)
    {
        Base = mBase;
        PP = mBase.PP;
    }
}