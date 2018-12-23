﻿#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   2018/05/08 15:19
// Modified On:  2018/11/22 18:37
// Modified By:  Alexis

#endregion




using System.Threading;
using System.Windows;
using System.Windows.Threading;
using SuperMemoAssistant.Services.IO;
using SuperMemoAssistant.SuperMemo;

namespace SuperMemoAssistant
{
  /// <summary>Interaction logic for App.xaml</summary>
  public partial class App : Application
  {
    #region Properties & Fields - Non-Public

    private SynchronizationContext SyncContext { get; set; }

    #endregion




    #region Methods

    private void Application_Startup(object           sender,
                                     StartupEventArgs e)
    {
      SyncContext = new DispatcherSynchronizationContext();
      SynchronizationContext.SetSynchronizationContext(SyncContext);

      var selectionWdw = new CollectionSelectionWindow();

      selectionWdw.ShowDialog();

      var selectedCol = selectionWdw.Collection;

      if (selectionWdw.Collection != null)
      {
        if (SMA.Instance.Start(selectedCol))
          SMA.Instance.OnSMStoppedEvent += Instance_OnSMStoppedEvent;
      }
      else
      {
        Shutdown();
      }
    }

    private void Instance_OnSMStoppedEvent(object                               sender,
                                           Interop.SuperMemo.Core.SMProcessArgs e)
    {
      SyncContext.Send(
        delegate { Shutdown(); },
        null);
    }

    protected override void OnExit(ExitEventArgs e)
    {
      Logger.Instance.Shutdown();

      base.OnExit(e);
    }

    #endregion
  }
}
