using System;
using System.Windows.Forms;
using System.Drawing;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal
{
    partial class Form1
    {
        private Label lblMessage;
        private ListBox lstPokemons;
        private Button btnSearchCat, btnClearSearch; 
        private Label lblCat; 
        private PictureBox pbCat; 
        private Button btnRefreshAll; 
        private PictureBox pbPokemon;
        private Panel panelInfo;
        private TextBox txtSearch;
        private Button btnSearch;
        private List<string> pokemonList = new List<string>();
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            SetupUI();
            this.Load += async (s, e) => await LoadPokemonList();
        }

        private void SetupUI()
        {
            this.Text = "Consumo de APIS";
            this.Size = new Size(900, 950);
            this.BackColor = Color.FromArgb(33, 37, 41);
             
            panelInfo = new Panel();
            panelInfo.Size = new Size(450, 520);
            panelInfo.Location = new Point(320, 50);
            panelInfo.BackColor = Color.FromArgb(52, 58, 64);
            panelInfo.BorderStyle = BorderStyle.FixedSingle;

            lblMessage = new Label();
            lblMessage.Text = "Selecciona un Pokémon";
            lblMessage.Location = new Point(20, 20);
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Arial", 14, FontStyle.Bold);
            lblMessage.ForeColor = Color.White;
            panelInfo.Controls.Add(lblMessage);

            pbPokemon = new PictureBox();
            pbPokemon.Location = new Point(120, 60);
            pbPokemon.Size = new Size(200, 200);
            pbPokemon.SizeMode = PictureBoxSizeMode.StretchImage;
            pbPokemon.BorderStyle = BorderStyle.FixedSingle;
            panelInfo.Controls.Add(pbPokemon);

            lblCat = new Label();
            lblCat.Text = "Gatito Aleatorio:";
            lblCat.Location = new Point(20, 280);
            lblCat.AutoSize = true;
            lblCat.Font = new Font("Arial", 12, FontStyle.Regular);
            lblCat.ForeColor = Color.LightGray;
            panelInfo.Controls.Add(lblCat);

            pbCat = new PictureBox();
            pbCat.Location = new Point(120, 310); // Ajusta la posición para que no se superponga
            pbCat.Size = new Size(200, 180); // Tamaño similar al de pbPokemon
            pbCat.SizeMode = PictureBoxSizeMode.StretchImage;
            pbCat.BorderStyle = BorderStyle.FixedSingle;
            panelInfo.Controls.Add(pbCat);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(50, 20);
            txtSearch.Size = new Size(150, 20);
            txtSearch.TextChanged += (s, e) => FilterList();
            this.Controls.Add(txtSearch);

            btnClearSearch = new Button();
            btnClearSearch.Text = "Limpiar";
            btnClearSearch.Location = new Point(210, 18); // Ajusta la posición según tu diseño
            btnClearSearch.Size = new Size(80, 25); // Tamaño del botón
            btnClearSearch.ForeColor = Color.White; // Color del texto
            btnClearSearch.BackColor = Color.FromArgb(52, 58, 64); // Fondo del botón
            btnClearSearch.Click += (s, e) =>
            {
                txtSearch.Text = ""; // Limpia el cuadro de búsqueda
                lstPokemons.Items.Clear(); // Limpia la lista
                lstPokemons.Items.AddRange(pokemonList.ToArray()); // Restaura la lista completa
            };
            this.Controls.Add(btnClearSearch);

            btnSearch = new Button();
            btnSearch.Text = "Buscar";
            btnSearch.Location = new Point(300, 18);
            btnSearch.ForeColor = Color.White; // Cambia el color del texto a blanco
            btnSearch.Click += async (s, e) => await SearchPokemon();
            this.Controls.Add(btnSearch);

            btnSearchCat = new Button();
            btnSearchCat.Text = "Mostrar Minino"; // Usa un salto de línea (\n) para dividir el texto
            btnSearchCat.Location = new Point(400, 18);
            btnSearchCat.Size = new Size(100, 25); // Ajusta el tamaño del botón
            btnSearchCat.ForeColor = Color.White; // Cambia el color del texto a blanco
            btnSearchCat.TextAlign = ContentAlignment.MiddleCenter; // Centra el texto
            btnSearchCat.Click += async (s, e) => await LoadCatData();
            this.Controls.Add(btnSearchCat);

            lstPokemons = new ListBox();
            lstPokemons.Location = new Point(50, 50);
            lstPokemons.Size = new Size(200, 500);
            lstPokemons.Font = new Font("Arial", 10);
            lstPokemons.BackColor = Color.FromArgb(52, 58, 64);
            lstPokemons.ForeColor = Color.White;
            lstPokemons.BorderStyle = BorderStyle.FixedSingle;
            lstPokemons.Items.Add("Cargando...");
            lstPokemons.SelectedIndexChanged += new EventHandler(this.LstPokemons_SelectedIndexChanged);

            this.Controls.Add(lstPokemons);
            this.Controls.Add(panelInfo);
        }
        private async void LstPokemons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPokemons.SelectedItem != null)
            {
                string selectedPokemon = lstPokemons.SelectedItem.ToString().ToLower();
                await LoadPokemonData(selectedPokemon);
            }
        }

        private async Task LoadPokemonData(string pokemonName)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://pokeapi.co/api/v2/pokemon/{pokemonName}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    string imageUrl = data.sprites.front_default;
                    lblMessage.Text = $"Seleccionaste: {pokemonName.ToUpper()}";

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        pbPokemon.Load(imageUrl);
                    }
                    else
                    {
                        pbPokemon.Image = null;
                        lblMessage.Text += " (Imagen no disponible)";
                    }
                }
                else
                {
                    lblMessage.Text = "Error al cargar datos del Pokémon.";
                    pbPokemon.Image = null;
                }
            }
        }

        private async Task LoadPokemonList()
        {
            lstPokemons.Items.Clear();
            lstPokemons.Items.Add("Cargando...");
            using (HttpClient client = new HttpClient())
            {
                string url = "https://pokeapi.co/api/v2/pokemon?limit=50";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    pokemonList = ((IEnumerable<dynamic>)data.results).Select(p => (string)p.name).ToList();

                    pokemonList.Sort();

                    lstPokemons.Items.Clear();
                    lstPokemons.Items.AddRange(pokemonList.ToArray());
                }
            }
        }

        private void FilterList()
        {
            string filter = txtSearch.Text.ToLower();
            var filteredList = pokemonList.Where(p => p.Contains(filter)).ToList();
            lstPokemons.Items.Clear();
            lstPokemons.Items.AddRange(filteredList.ToArray());
        }

        private async Task SearchPokemon()
        {
            string searchQuery = txtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(searchQuery) && searchQuery != "buscar pokémon...")
            {
                await LoadPokemonData(searchQuery);
            }
            else
            {
                lblMessage.Text = "Por favor, ingresa un nombre válido para buscar.";
            }
        }

        private async Task LoadCatData()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.thecatapi.com/v1/images/search";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    string imageUrl = data[0].url;

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        pbCat.Load(imageUrl); // Cambiado a pbCat para cargar la imagen del gatito
                    }
                    else
                    {
                        lblMessage.Text = "No se pudo cargar la imagen del gatito.";
                        pbCat.Image = null; // Limpia la imagen en pbCat si no hay URL
                    }
                }
                else
                {
                    lblMessage.Text = "Error al cargar datos del gatito.";
                    pbCat.Image = null; // Limpia la imagen en pbCat si hay un error
                }
            }
        }
        private async Task RefreshAllImages()
        {
            if (lstPokemons.SelectedItem != null)
            {
                string selectedPokemon = lstPokemons.SelectedItem.ToString().ToLower();
                await LoadPokemonData(selectedPokemon); // Actualiza solo la imagen del Pokémon
            }

            await LoadCatData(); // Actualiza solo la imagen del gatito
        }

        #endregion
    }
}
