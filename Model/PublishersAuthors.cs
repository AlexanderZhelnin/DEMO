namespace Demo.Model;

public class PublishersAuthors
{
    public int AuthorId { get; set; }
    public Author Author {  get; set; }
    public int PublisherId { get; set; }
    public Publisher Publisher {  get; set; }
}
