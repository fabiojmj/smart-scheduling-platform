using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface ITokenService
{
    string GerarToken(Usuario usuario);
}
