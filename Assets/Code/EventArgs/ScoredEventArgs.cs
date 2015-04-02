using System;

public class ScoredEventArgs : EventArgs
{
    public Team Team { get; private set; }
    public int Points { get; private set; }

    public ScoredEventArgs(Team _team, int _points)
    {
        Team = _team;
        Points = _points;
    }
}