namespace Octamino.Constant
{
    public static class Text
    {
        public static readonly string GameFinished = "GAME FINISHED";
        public static readonly string GamePaused = "GAME PAUSED";
        public static readonly string Settings = "SETTINGS";
        public static readonly string HighScore = "HIGH SCORE";
        public static readonly string Music = "MUSIC";
    }

    public static class ScoreFormat
    {
        public static readonly int Length = 9;
        public static readonly char PadCharacter = '0';
    }

    public static class Input
    {
        public static readonly float KeyRepeatDelay = 0.18f;
        public static readonly float KeyRepeatInterval = 0.07f;
    }
}
