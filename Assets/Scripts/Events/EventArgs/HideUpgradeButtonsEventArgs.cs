using System;

public class HideUpgradeButtonsEventArgs : EventArgs
{
    public bool isHero;

    public HideUpgradeButtonsEventArgs(bool isHero)
    {
        this.isHero = isHero;
    }
}
