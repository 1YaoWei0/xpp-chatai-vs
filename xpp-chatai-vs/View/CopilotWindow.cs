using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace xpp_chatai_vs.View
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// Willie Yao - 04/14/2025
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("51ee489a-a5b9-4af1-9d72-943b636aced7")]
    public class CopilotWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopilotWindow"/> class.
        /// Willie Yao - 04/14/2025
        /// </summary>
        public CopilotWindow() : base(null)
        {
            this.Caption = "Copilot";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new CopilotWindowControl();
        }
    }
}
