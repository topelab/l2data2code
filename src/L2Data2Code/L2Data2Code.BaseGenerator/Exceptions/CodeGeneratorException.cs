using L2Data2Code.SharedLib.Helpers;
using System;

namespace L2Data2Code.BaseGenerator.Exceptions
{
    public class CodeGeneratorException : Exception
    {
        private const string UNDEFINED_ERROR = "Undefined error";
        public string LastError { get; private set; }
        public CodeGeneratorExceptionType LastExceptionType { get; private set; }

        public static Action<string> OnError { get; set; }

        public CodeGeneratorException() : base()
        {
            LastError = UNDEFINED_ERROR;
            LastExceptionType = CodeGeneratorExceptionType.General;
            LogService.Error(UNDEFINED_ERROR);
            OnError?.Invoke(UNDEFINED_ERROR);
        }

        public CodeGeneratorException(string message, CodeGeneratorExceptionType exceptionType = CodeGeneratorExceptionType.General) : base(message)
        {
            LastError = message;
            LastExceptionType = exceptionType;
            LogService.Error(message);
            OnError?.Invoke(message);
        }

        public CodeGeneratorException(string message,
                                      Exception innerException,
                                      CodeGeneratorExceptionType exceptionType = CodeGeneratorExceptionType.General) : base(message, innerException)
        {
            LastError = $"{message} ({innerException?.Message ?? string.Empty})";
            LastExceptionType = exceptionType;
            LogService.Error(LastError);
            OnError?.Invoke(message);
        }
    }

    public enum CodeGeneratorExceptionType
    {
        General,
        TemplateNotFound,
        ErrorLoadingTemplate,
        ErrorLoadingWordList,
    }

}
