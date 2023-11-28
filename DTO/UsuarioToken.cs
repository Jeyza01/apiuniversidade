namespace ApiUniversidade.DTO;
public class UsuarioToken
{
    public bool UseAuthentication {get; set; }
    public DateTime Expiration {get; set; }
    public string Token {get; set; }
    public string Message {get; set; }
    
}