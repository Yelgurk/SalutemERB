namespace SalutemES.Engineer.Avalonia.Extra;

public enum KeyCombination { NONE, CTRL_F };

public interface IHotKeyHandler
{
    public void HotkeyWorked(KeyCombination combination);
}