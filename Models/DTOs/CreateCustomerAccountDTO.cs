namespace CrudBankApp.Models.DTOs
{
    public class CreateCustomerAccountDTO
    {
        public int CustomerProfileId { get; set; }
        public int AccountTypeId { get; set; }
        public decimal InitialBalance { get; set; }
    }
}