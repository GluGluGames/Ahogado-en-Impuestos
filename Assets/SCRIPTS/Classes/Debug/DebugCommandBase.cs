using UnityEngine;

namespace GGG.Classes.Debug
{
    public class DebugCommandBase
    {
        private readonly string _commandId;
        private readonly string _commandDescription;
        private readonly string _commandFormat;

        public DebugCommandBase(string id, string description, string format)
        {
            _commandId = id;
            _commandDescription = description;
            _commandFormat = format;
        }

        public string GetId => _commandId;
        public string GetDescription => _commandDescription;
        public string GetFormat => _commandFormat;
    }
}
