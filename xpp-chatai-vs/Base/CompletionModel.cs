using System.ComponentModel;

namespace xpp_chatai_vs.Base
{
    /// <summary>
    /// Completion model enum
    /// Willie Yao - 04/14/2025
    /// </summary>
    public enum CompletionModel
    {
        [Description("Deepseek")]
        Deepseek,
        [Description("GPT-4o")]
        GPT4o,
        [Description("X++ assistant")]
        XppAssistant
    }
}
