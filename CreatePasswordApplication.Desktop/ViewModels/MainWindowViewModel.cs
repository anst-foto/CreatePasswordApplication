using System;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CreatePasswordApplication.PasswordCheckLib;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CreatePasswordApplication.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [Reactive] public string? InputPasswordText { get; set; }
    [Reactive] public string? InputRepeatPasswordText { get; set; }
    
    [Reactive] public bool IsErrorMinLenght { get; set; }
    [Reactive] public bool IsErrorMasterLetters { get; set; }
    [Reactive] public bool IsErrorSecondLetters { get; set; }
    [Reactive] public bool IsErrorDigits { get; set; }
    [Reactive] public bool IsErrorSymbols { get; set; }
    [Reactive] public bool IsErrorEqualPasswords { get; set; }
    
    
    public ReactiveCommand<Unit, Unit> CommandSave { get; }
    public ReactiveCommand<Unit, Unit> CommandClear { get; }

    public MainWindowViewModel()
    {
        IsErrorMinLenght = false;
        IsErrorMasterLetters = false;
        IsErrorSecondLetters = false;
        IsErrorDigits = false;
        IsErrorSymbols = false;
        IsErrorEqualPasswords = false;
        
        var observablePasswordAndRepeatPasswordWithEquals = this.WhenAnyValue(
            vm => vm.InputPasswordText,
            vm => vm.InputRepeatPasswordText,
            (p1, p2) => !string.IsNullOrWhiteSpace(p1) 
                        && !string.IsNullOrWhiteSpace(p2)
                        && p1 == p2);

        var canExecuteCommandClear = this.WhenAnyValue(
            vm => vm.InputPasswordText,
            vm => vm.InputRepeatPasswordText,
            (p1, p2) => !string.IsNullOrWhiteSpace(p1) || !string.IsNullOrWhiteSpace(p2));
        
        CommandSave = ReactiveCommand.Create(
            execute: Cancel,
            canExecute: observablePasswordAndRepeatPasswordWithEquals);
        
        CommandClear = ReactiveCommand.Create(
            execute: ClearInputs,
            canExecute: canExecuteCommandClear);

        var passwordCheck = new PasswordCheck()
        {
            MinLength = 8,
            MasterLetters = "QWERTYUIOPASDFGHJKLZXCVBNM",
            SecondLetters = "qwertyuiopasdfghjklzxcvbnm",
            Digits = "1234567890",
            Symbols = "!@#$%^&*_-+="
        };
        
        var observablePassword = this
            .WhenValueChanged(vm => vm.InputPasswordText)
            .WhereNotNull();
        observablePassword.Subscribe(p =>
        {
            passwordCheck.Password = p;
            
            IsErrorMinLenght = !passwordCheck.CheckMinLength;
            IsErrorMasterLetters = !passwordCheck.CheckMasterLetters;
            IsErrorSecondLetters = !(passwordCheck.CheckSecondLetters ?? false);
            IsErrorDigits = !(passwordCheck.CheckDigits ?? false);
            IsErrorSymbols = !(passwordCheck.CheckSymbols ?? false);
        });

        observablePasswordAndRepeatPasswordWithEquals
            .Subscribe(b => IsErrorEqualPasswords = !b);
    }

    private void ClearInputs()
    {
        InputPasswordText = null;
        InputRepeatPasswordText = null;
    }

    public void Cancel()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            lifetime.Shutdown();
    }
}