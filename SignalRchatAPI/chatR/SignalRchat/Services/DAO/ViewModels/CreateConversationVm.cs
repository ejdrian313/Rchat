using System.ComponentModel.DataAnnotations;

namespace SignalRchat.Services.DAO.ViewModels
{
    public class CreateConversationVm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set;}
    }
}