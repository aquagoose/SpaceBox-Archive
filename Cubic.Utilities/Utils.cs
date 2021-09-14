namespace Cubic.Utilities
{
    public static class Utils
    {
        public static float Lerp(float value1, float value2, float amount)
        {
            // I need this because the MathHelper lerp function clamps at 0 or 1 which for the menu system is not
            // the desired behaviour.
            return value1 + amount * (value2 - value1);
        }
    }
}