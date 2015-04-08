using System;

public class MatchStartVoiceEventArgs : EventArgs
{
    public Team Team = Team.UNASSIGNED;

    public MatchStartVoiceEventArgs(Team _team)
    {
        Team = _team;
    }
}