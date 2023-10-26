using System;

public interface IKeyboardService
{
    void RegisterKeyPress(Action<string> keyPressCallback);
}
