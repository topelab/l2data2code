using System;
using System.Collections.Generic;
using System.Text;

namespace L2Data2CodeUI.Shared.Dto
{
    public enum MessageType
    {
        Info,
        Warning,
        Error
    }

    public class MessageCodes
    {
        public const string RUN_GENERATOR = "RUN";
        public const string RUN_COMMAND = "RCM";
        public const string CAN_GENERATE_CODE = "CGC";
        public const string CONNECTION = "CON";
        public const string READ_SCHEMA = "RDB";
        public const string FIND_SERVICE = "FSV";
        public const string LOADING_TEMPLATES = "LTP";
    }
}
