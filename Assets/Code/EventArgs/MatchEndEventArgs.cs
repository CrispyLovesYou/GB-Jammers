using System;

public class MatchEndEventArgs : EventArgs
{
    public Team Winner;
    public int L_Sets;
    public int R_Sets;

    public MatchEndEventArgs(Team _winner, int _lsets, int _rsets)
    {
        Winner = _winner;
        L_Sets = _lsets;
        R_Sets = _rsets;
    }
}