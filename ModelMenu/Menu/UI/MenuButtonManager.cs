using BeatSaberMarkupLanguage.MenuButtons;
using System;
using Zenject;

namespace ModelMenu.Menu.UI;

internal class MenuButtonManager : IInitializable, IDisposable
{
    private readonly MainFlowCoordinator mainFlowCoordinator;
    private readonly ModelMenuFlowCoordinator modelMenuFlowCoordinator;

    private MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, ModelMenuFlowCoordinator modelMenuFlowCoordinator) =>
        (this.mainFlowCoordinator, this.modelMenuFlowCoordinator) = (mainFlowCoordinator, modelMenuFlowCoordinator);

    private MenuButton button;

    public void Initialize()
    {
        button = new("More Models", "View and download more custom models", PresentFlowCoordinator);
        MenuButtons.instance.RegisterButton(button);
    }

    private void PresentFlowCoordinator() =>
        mainFlowCoordinator.PresentFlowCoordinator(modelMenuFlowCoordinator);

    public void Dispose() =>
        MenuButtons.instance.UnregisterButton(button);
}
