namespace Uaeglp.ViewModels
{
    public class PagingView
	{
        public int TotalCount { get; set; }

        public int PageCount { get; set; }

        public int CurrentPageNumber { get; set; }

        public bool IsFilterUpdated { get; set; }

        public int Skip
        {
            get
            {
                return this.CurrentPageNumber == -1 ? 0 : this.PageCount * this.CurrentPageNumber + this.AddedItemsCount;
            }
        }

        public int Take
        {
            get
            {
                return this.PageCount;
            }
        }

        public bool DevMode { get; set; }

        public int AddedItemsCount { get; set; }
    }
}
