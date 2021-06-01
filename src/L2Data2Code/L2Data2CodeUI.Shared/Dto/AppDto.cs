using L2Data2CodeUI.Shared.Adapters;

namespace L2Data2CodeUI.Shared.Dto
{
    public enum AppType
    {
        VisualStudio,
        VisualStudioCode,
        ApacheNetBeans,
        Eclipse,
        IntelliJIdea,
        Undefined = 99
    }

    public enum AppTypeAbbr
    {
        vs,
        vsc,
        nb,
        ec,
        ij
    }

    public class AppDto
    {
        public AppType AppType { get; set; } = AppType.VisualStudio;
        public string SearchExpression { get; set; } = "*.sln";
        public string CommandLine { get; set; } = AppService.FILE_PATTERN;
        public string CommandArguments { get; set; } = string.Empty;
    }
}
