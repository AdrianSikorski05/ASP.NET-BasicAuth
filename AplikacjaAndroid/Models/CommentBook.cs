using CommunityToolkit.Mvvm.ComponentModel;

namespace AplikacjaAndroid
{
    public partial class CommentBook : ObservableObject
    {       
        public int id { get; set; }       
        public string? Author { get; set; }       
        public string? Content { get; set; }      
        public short? Rate { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
