using Newtonsoft.Json;
using ShoppingBackend.Models;
using System.Collections.ObjectModel;

namespace ShoppingMaui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadDataFromRestAPI();
        }

        // LISTAN HAKEMINEN BACKENDISTÄ
        async void LoadDataFromRestAPI()
        {
            
            // Latausilmoitus näkyviin
            Loading_label.IsVisible = true;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://shoppingbackendtony-cpcgcpfhgjb7a5eh.swedencentral-01.azurewebsites.net/");
            string json = await client.GetStringAsync("/api/shoplist/");

            // Deserialisoidaan json muodosta C# muotoon
            IEnumerable<Shoplist> ienumShoplist = JsonConvert.DeserializeObject<Shoplist[]>(json);

            ObservableCollection<Shoplist> Shoplist = new ObservableCollection<Shoplist>(ienumShoplist);

            itemList.ItemsSource = Shoplist;

            Loading_label.IsVisible = false;
            addPageBtn.IsVisible = true;
            kerätty_nappi.IsVisible = true;
        }

    }

}
