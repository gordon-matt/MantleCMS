//No license, but thanks to Matteo Tontini
//http://ilmatte.wordpress.com/2010/11/16/asp-net-mvc-panel-htmlhelper-extension-methods-with-using-syntax/

namespace Mantle.Web.Mvc.Controls;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

/// <summary>
/// Represents a Panel with a title panel.
/// It's needed to support using syntax when invoking
/// helper methods rendering such panel.
/// </summary>
public class MvcPanel : IDisposable
{
    #region Fields

    private bool disposed;
    private readonly TextWriter writer;

    #endregion Fields

    #region CTOR

    public MvcPanel(TextWriter writer)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }
        this.writer = writer;
    }

    #endregion CTOR

    #region Methods

    [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
    public void Dispose()
    {
        Dispose(true /* disposing */);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.disposed = true;
            this.writer.Write("</div>");
        }
    }

    public void EndPanel()
    {
        Dispose(true);
    }

    #endregion Methods
}