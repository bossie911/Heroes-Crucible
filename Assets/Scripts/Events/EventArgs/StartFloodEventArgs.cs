using System;
public class StartFloodEventArgs : EventArgs
{
    public LTDescr FloodInfo { get; private set; }

    public StartFloodEventArgs(LTDescr floodInfo)
    {
        this.FloodInfo = floodInfo;
    }
}