using System;

public class ScoredEventArgs : EventArgs
{
    public Team Team { get; private set; }

    public ScoredEventArgs(Team _team)
    {
        Team = _team;
    }
}