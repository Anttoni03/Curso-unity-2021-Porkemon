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
        set => Base = _base;
    }

    public int PP
    {
        get => _pp;
        set => PP = _pp;
    }

    public Move(MoveBasic mBase)
    {
        Base = mBase;
        PP = mBase.PP;
    }



}
