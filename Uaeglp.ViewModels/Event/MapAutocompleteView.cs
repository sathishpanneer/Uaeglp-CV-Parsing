namespace Uaeglp.ViewModels.Event
{
    public class MapAutocompleteView
    {
        public MapAutocompleteView()
        {
        }

        public MapAutocompleteView(string text)
        {
            this.Text = text;
            this.Latitude = string.Empty;
            this.Longitude = string.Empty;
            this.PlaceID = string.Empty;
        }

        public string Text { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string PlaceID { get; set; }

        public string GoogleMapsURL
        {
            get
            {
                if (string.IsNullOrEmpty(this.Latitude) || string.IsNullOrEmpty(this.Longitude))
                    return string.Empty;
                if (string.IsNullOrEmpty(this.PlaceID))
                    return "https://www.google.com/maps/?q=+" + this.Latitude + "," + this.Longitude ?? "";
                return "https://www.google.com/maps/search/?api=1&query=+" + this.Latitude + "," + this.Longitude + "&query_place_id=" + this.PlaceID ?? "";
            }
        }
    }
}