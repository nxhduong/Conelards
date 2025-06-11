using System.ComponentModel.DataAnnotations;

namespace Conelards.Database;

public class AccountDbModel
{
    [Key]
    public string Username;
    public string HashPassword;
}