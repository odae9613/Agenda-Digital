namespace ImOdNotes.Models.ViewModels
{
    public class LoginViewModel
    {
        public string NombreUsuario { get; set; } = string.Empty;
        //public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } // opcional
    }
}
