﻿using Eneca.SpacesManager.Commands;
using Nice3point.Revit.Toolkit.External;

namespace Eneca.SpacesManager;
[UsedImplicitly]
public class Application : ExternalApplication
{
    public static Application ThisApp;

    public Application()
    {
        ThisApp = this;
    }

    public override void OnStartup()
    {
        var panel = Application.CreatePanel("Manage", "Eneca");

        var showButton = panel.AddPushButton<EntryCommand>("Spaces\nManager");
        showButton.SetImage("/Eneca.SpacesManager;component/Resources/Icons/RibbonIcon16.png");
        showButton.SetLargeImage("/Eneca.SpacesManager;component/Resources/Icons/RibbonIcon32.png");
        showButton.ToolTip =
            "Размещает пространства на основе помещений связанных файлов, а также анализирует их изменения.";
    }

    public override void OnShutdown()
    {
    }
}