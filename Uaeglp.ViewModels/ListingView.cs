namespace Uaeglp.ViewModels
{
    public class ListingView<T> where T : class, new()
    {
        public ListingView()
        {
        }

        public ListingView(T data, PagingView pagingView)
        {
            this.Data = data;
            this.PagingView = pagingView;
        }

        public PagingView PagingView { get; set; }

        public T Data { get; set; }
    }
}
