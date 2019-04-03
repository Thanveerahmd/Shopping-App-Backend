namespace Project.Entities
{
    public class Admin
    {
         public int Id { get; set; }
         public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool FirstLogin { get; set; }
        public System.Guid ActivationCode { get; set; }
        public bool IsEmailConfirmed { get; set; }

    }
}