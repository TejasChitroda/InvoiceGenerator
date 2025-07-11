namespace Invoice_Generator.DTOs
{
    public class InvoiceCreateDto
    {
        public int CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
