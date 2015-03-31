using System;

public class MeterChangeEventArgs : EventArgs
{
    public Team Team;
    public int Delta;
    public int Total;

    public MeterChangeEventArgs(Team _team, int _delta, int _total)
    {
        Team = _team;
        Delta = _delta;
        Total = _total;
    }
}