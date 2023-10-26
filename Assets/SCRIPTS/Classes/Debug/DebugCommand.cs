using System;

namespace GGG.Classes.Debug
{
    public class DebugCommand : DebugCommandBase
    {
        private Action _command;
        
        public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
        {
            _command = command;
        }

        public void InvokeCommand()
        {
            _command?.Invoke();
        }
    }

    public class DebugCommand<T1> : DebugCommandBase
    {
        private Action<T1> _command;

        public DebugCommand(string id, string description, string format, Action<T1> command) : base(id, description, format)
        {
            _command = command;
        }

        public void InvokeCommand(T1 value)
        {
            _command?.Invoke(value);
        }
    }
    
    public class DebugCommand<T1, T2> : DebugCommandBase
    {
        private Action<T1, T2> _command;

        public DebugCommand(string id, string description, string format, Action<T1, T2> command) : base(id, description, format)
        {
            _command = command;
        }

        public void InvokeCommand(T1 value, T2 value2)
        {
            _command?.Invoke(value, value2);
        }
    }
}
