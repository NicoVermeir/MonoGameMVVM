using System;

namespace MonoGameMVVM
{
    public interface INavigationService
    {
        bool Navigate(Type target);
    }
}
