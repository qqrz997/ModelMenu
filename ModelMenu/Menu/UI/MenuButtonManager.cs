using BeatSaberMarkupLanguage.MenuButtons;
using System;
using Zenject;

namespace ModelMenu.Menu.UI;

internal class MenuButtonManager : IInitializable, IDisposable
{
    private readonly MainFlowCoordinator mainFlowCoordinator;
    private readonly ModelMenuFlowCoordinator modelMenuFlowCoordinator;

    private MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, ModelMenuFlowCoordinator modelMenuFlowCoordinator)
    {
        this.mainFlowCoordinator = mainFlowCoordinator;
        this.modelMenuFlowCoordinator = modelMenuFlowCoordinator;
    }

    private MenuButton button;

    public void Initialize()
    {
        button = new("More Models", "View and download more custom models", PresentFlowCoordinator);
        MenuButtons.Instance.RegisterButton(button);

        modelMenuFlowCoordinator.DidFinish += ModelMenuDidFinish;
    }

    private void ModelMenuDidFinish() => 
        mainFlowCoordinator.DismissFlowCoordinator(modelMenuFlowCoordinator);

    private void PresentFlowCoordinator() =>
        mainFlowCoordinator.PresentFlowCoordinator(modelMenuFlowCoordinator);

    public void Dispose()
    {
        MenuButtons.Instance.UnregisterButton(button);
        modelMenuFlowCoordinator.DidFinish -= ModelMenuDidFinish;
    }
}
