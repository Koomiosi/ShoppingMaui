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
            AutoUpdate();
        }

        public async Task AutoUpdate()
        {
            int nextUpdate = 80; //Aika sekunteina paivitysten valissa

            for (int i = 0; i < 1000; i++)
            {
                await Task.Delay(1000);
                nextUpdate--;
                if (nextUpdate <60)
                {   //Sekunti laskuri nakyy kun alle minuutti jaljella
                    autoupdateLabel.Text = "Seuraavaan päivitykseen" + nextUpdate + "sek.";

                    //Kun nollassa ladataan listaus ja laskuri nollautuu
                    if (nextUpdate == 0)
                    {
                        autoupdateLabel.Text = "";
                        nextUpdate = 80;
                        await LoadDataFromRestAPI();
                    }
                }
            }
        }

        // LISTAN HAKEMINEN BACKENDISTÄ
        public async Task LoadDataFromRestAPI()
        {
            
            // Latausilmoitus näkyviin
            Loading_label.IsVisible = true;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://shoppingbackendtony-cpcgcpfhgjb7a5eh.swedencentral-01.azurewebsites.net");
            string json = await client.GetStringAsync("/api/shoplist/");

            // Deserialisoidaan json muodosta C# muotoon
            IEnumerable<Shoplist> ienumShoplist = JsonConvert.DeserializeObject<Shoplist[]>(json);

            ObservableCollection<Shoplist> Shoplist = new ObservableCollection<Shoplist>(ienumShoplist);

            itemList.ItemsSource = Shoplist;

            Loading_label.IsVisible = false;
            addPageBtn.IsVisible = true;
            kerätty_nappi.IsVisible = true;
        }

        private async void addPageBtn_Clicked(object sender, EventArgs e)
        {
            var addingPage = new AddingPage(this);
            await Shell.Current.Navigation.PushModalAsync(addingPage);
        }

    }

}
