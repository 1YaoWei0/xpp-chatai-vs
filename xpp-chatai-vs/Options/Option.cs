﻿using Community.VisualStudio.Toolkit;
using System.ComponentModel;
using System.Runtime.InteropServices;
using xpp_chatai_vs.Base;

namespace xpp_chatai_vs
{
    public partial class OptionsProvider
    {
        [ComVisible(true)]
        public class OptionOptions : BaseOptionPage<Option> { }
    }

    public class Option : BaseOptionModel<Option>
    {
        private CompletionModel completionModel = CompletionModel.GPT4o;

        [Category("AI")]
        [DisplayName("AI: Completion Model")]
        [Description("Configure which model to use for code completion.")]
        public CompletionModel CompletionModel { get => completionModel; set => completionModel = value; }
    }
}
