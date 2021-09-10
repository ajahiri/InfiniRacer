/// <summary>
/// A selection of common pre-set vibration lengths
/// Short:      100 Milliseconds
/// Medium:     250 Milliseconds
/// Long:       500 Milliseconds
/// Very Long:  1000 Milliseconds
/// 
/// Use Vibrator.Vibrate(Vibration) to use these pre-sets
/// Use Vibrator.Vibrate(long) to specify custom vibration lengths
/// </summary>
public enum Vibration
{
    SHORT = 100,
    MEDIUM = 250,
    LONG = 500,
    VERY_LONG = 1000
}
