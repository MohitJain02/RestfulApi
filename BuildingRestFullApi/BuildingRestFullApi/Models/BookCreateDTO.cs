namespace BuildingRestFullApi.Models
{
    /// <summary>
    /// This model is used to receive the request from the client
    /// </summary>
    public class BookCreateDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
