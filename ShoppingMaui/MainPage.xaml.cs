﻿using Newtonsoft.Json;
using ShoppingBackend.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
            refreshView.IsRefreshing = false;
        }

        private async void addPageBtn_Clicked(object sender, EventArgs e)
        {
            var addingPage = new AddingPage(this);
            await Shell.Current.Navigation.PushModalAsync(addingPage);
        }

        private void refreshView_Refreshing(object sender, EventArgs e)
        {
            LoadDataFromRestAPI();
        }

        private void itemList_ItemSelected(object sender, EventArgs e)
        {
            Shoplist? selectedItem = itemList.SelectedItem as Shoplist;
            kerätty_nappi.Text = "Poimi " + selectedItem?.Item;
        }

        async void kerätty_nappi_Clicked(object sender, EventArgs e)
        {
            Shoplist? selected = itemList.SelectedItem as Shoplist;

            if (selected == null)
            {
                await DisplayAlert("Valinta puuttuu", "Valitse ensin poimittava tuote", "ok");
                return;
            }

            bool answer = await DisplayAlert("Menikö oikein?", selected.Item + "kerätty?", "Yes! Kyllä meni!", "Ei, yritän uudestaan");
            if (answer == false)
            {
                return;
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://shoppingbackendtony-cpcgcpfhgjb7a5eh.swedencentral-01.azurewebsites.net");
            HttpResponseMessage res = await client.DeleteAsync("/api/shoplist/" + selected.Id);

            if (res.StatusCode ==System.Net.HttpStatusCode.OK)
            {
                await LoadDataFromRestAPI();
            }
            else
            {
                await DisplayAlert("Tilapäinen virhe", "Joku muu on saattanut poistaa tuotteen sen jalkeen kun listauksesi on viimeksi paivittynyt.",
                    "Lataa uudelleen.");
                await LoadDataFromRestAPI();
            }
        }
    }

}
