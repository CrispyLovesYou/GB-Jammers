#region Enums

public enum GameModes
{
    DEBUG,
    MAIN_MENU,
    LOCAL_MULTIPLAYER,
    ONLINE_MULTIPLAYER
}

public enum InputType{
	KEYBOARD,
	CONTROLLER
}

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

public enum CharacterID
{
    DR_TRACKSUIT = 0,
    V_BOMB = 1,
    DIRTY_DAN_RYCKERT = 2,
    METAL_GEAR_SCANLON = 3
}

public enum PlayerState
{
    IDLE = 0,
    WALK = 1,
    DASH = 2,
    AIM = 3,
    CHARGE = 4,
    KNOCKBACK = 5,
    RECOVERY = 6,
    STUN = 7,
    RESET = 8
}

#endregion