#region Globals

public static class Globals
{
    public static bool AllowNetworkPrediction = false;
}

#endregion

#region Tags

public static class Tags
{
    public const string TEAM_LEFT = "Team_Left";
    public const string TEAM_RIGHT = "Team_Right";
}

#endregion

#region Enums

public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public enum Team
{
    UNASSIGNED,
    LEFT,
    RIGHT
}

#endregion